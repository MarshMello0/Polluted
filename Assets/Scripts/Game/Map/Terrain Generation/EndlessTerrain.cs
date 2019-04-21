using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndlessTerrain : MonoBehaviour {

	const float scale = 2.5f;

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

	public LODInfo[] detailLevels;
	public static float maxViewDst;

	public Transform viewer;
	public Material mapMaterial;

	public static Vector2 viewerPosition;
	Vector2 viewerPositionOld;
	static MapGenerator mapGenerator;
	int chunkSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();


	private static float waterHeight = 5f;
	private static float grassHeight = 20f;

	[SerializeField] private RoadGenerator roadGenerator;
	[HideInInspector]
	public Loading loading;
	public bool isLoading;
	private bool hasStarted;
	[SerializeField] private GameObject player;
	private void Awake()
	{
		isLoading = true;
		int i_viewDistance = FileManager.GetIntValueFromConfig("i_viewdistance");
		detailLevels = new LODInfo[3];
		detailLevels[0] = new LODInfo(0,i_viewDistance * 100,true);
		detailLevels[1] = new LODInfo(1,i_viewDistance * 200,false);
		detailLevels[2] = new LODInfo(2,i_viewDistance * 300,false);
		loading = FindObjectOfType<Loading>();
	}

	private void Start()
	{
		mapGenerator = FindObjectOfType<MapGenerator> ();

		maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
		StartCoroutine(UpdateVisibleChunks(Vector3.zero,chunksVisibleInViewDst, terrainChunksVisibleLastUpdate));
	}
	[ContextMenu("StartTerrainGeneration")]
	public void StartTerrainGeneration() 
	{
		
	}

	private void Update()
	{
		if (isLoading && SceneManager.sceneCount.Equals(1))
			isLoading = false;
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z) / scale;

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			StartCoroutine(UpdateVisibleChunks(viewerPosition,chunksVisibleInViewDst,terrainChunksVisibleLastUpdate));
		}
		
		if (!isLoading && !player.activeInHierarchy)
		{
			player.SetActive(true);
		}
	}
		
	public IEnumerator UpdateVisibleChunks(Vector2 position, int viewDistance, List<TerrainChunk> list, Action callback = null) 
	{
		/*
		for (int i = 0; i < list.Count; i++) 
		{
			list [i].SetVisible (false);
		}
		
		*/	
		list.Clear ();
		int currentChunkCoordX = Mathf.RoundToInt (position.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (position.y / chunkSize);

		for (int yOffset = -viewDistance; yOffset <= viewDistance; yOffset++) {
			for (int xOffset = -viewDistance; xOffset <= viewDistance; xOffset++) 
			{
				
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				
				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) 
				{
					terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk (position,list);
					FinishedLoadingChunk();
				} 
				else 
				{
					terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial,
						this, position, list,roadGenerator.cities,  callback));
					yield return new WaitForEndOfFrame();
				}
			}
		}
	}

	public void FinishedLoadingChunk()
	{
		//Finished Loading chunk here
		if (isLoading)
		{
			loading.numberOfActionsCompleted++;
		}
	}

	[Serializable]
	public class TerrainChunk 
	{

		GameObject meshObject;
		Vector2 position;
		Bounds bounds;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;

		LODInfo[] detailLevels;
		LODMesh[] lodMeshes;
		LODMesh collisionLODMesh;

		MapData mapData;
		bool mapDataReceived;
		int previousLODIndex = -1;

		private EndlessTerrain endlessTerrain;
		private Vector2 viewer;
		private List<TerrainChunk> chunks = new List<TerrainChunk>();
		private Action callback;
		public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, EndlessTerrain endlessTerrain, Vector2 viewer, List<TerrainChunk> list, List<City> cities, Action callback = null) {
			this.detailLevels = detailLevels;
			this.endlessTerrain = endlessTerrain;
			this.viewer = viewer;
			this.chunks = list;
			
			position = coord * size;
			bounds = new Bounds(position,Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x,0,position.y);

			meshObject = new GameObject("Chunk " + positionV3);
			meshObject.tag = "Terrain";
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider = meshObject.AddComponent<MeshCollider>();
			meshRenderer.material = material;

			meshObject.transform.position = positionV3 * scale;
			meshObject.transform.parent = parent;
			meshObject.transform.localScale = Vector3.one * scale;
			SetVisible(false);

			lodMeshes = new LODMesh[detailLevels.Length];
			
			for (int i = 0; i < detailLevels.Length; i++) {
				lodMeshes[i] = new LODMesh(detailLevels[i].lod, delegate { UpdateTerrainChunk(viewer, list); });
				if (detailLevels[i].useForCollider) {
					collisionLODMesh = lodMeshes[i];
				}
			}

			if (callback != null)
				this.callback = callback;
			mapGenerator.RequestMapData(position, cities,OnMapDataReceived);
		}

		void OnMapDataReceived(MapData mapData) {
			this.mapData = mapData;
			mapDataReceived = true;

			Texture2D texture = TextureGenerator.TextureFromColourMap (mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
			meshRenderer.material.mainTexture = texture;
			endlessTerrain.FinishedLoadingChunk();
			
			if (callback != null)
				callback.Invoke();
			callback = null;
			
			UpdateTerrainChunk (viewer,chunks);
		}

	

		public void UpdateTerrainChunk(Vector2 viewer, List<TerrainChunk> chunks) {
			if (mapDataReceived) 
			{
				float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewer));
				//bool visible = viewerDstFromNearestEdge <= maxViewDst;
				bool visible = true;
				if (visible) {
					int lodIndex = 0;

					for (int i = 0; i < detailLevels.Length - 1; i++) {
						if (viewerDstFromNearestEdge > detailLevels [i].visibleDstThreshold) {
							lodIndex = i + 1;
						} else {
							break;
						}
					}

					if (lodIndex != previousLODIndex) {
						LODMesh lodMesh = lodMeshes [lodIndex];
						if (lodMesh.hasMesh) {
							previousLODIndex = lodIndex;
							meshFilter.mesh = lodMesh.mesh;
						} else if (!lodMesh.hasRequestedMesh) {
							lodMesh.RequestMesh (mapData);
						}
					}

					if (lodIndex == 0) {
						if (collisionLODMesh.hasMesh) {
							meshCollider.sharedMesh = collisionLODMesh.mesh;
						} else if (!collisionLODMesh.hasRequestedMesh) {
							collisionLODMesh.RequestMesh (mapData);
						}
					}

					chunks.Add (this);
				}

				SetVisible (visible);
			}
		}

		public void SetVisible(bool visible) {
			meshObject.SetActive (visible);
		}

		public bool IsVisible() {
			return meshObject.activeSelf;
		}

	}

	class LODMesh {

		public Mesh mesh;
		public bool hasRequestedMesh;
		public bool hasMesh;
		int lod;
		System.Action updateCallback;

		public LODMesh(int lod, System.Action updateCallback) {
			this.lod = lod;
			this.updateCallback = updateCallback;
		}

		void OnMeshDataReceived(MeshData meshData) {
			mesh = meshData.CreateMesh ();
			hasMesh = true;
			
			//Add the colours for the shader graph
				
			Vector3[] vertices = mesh.vertices;
			List<Color> colours = new List<Color>();

			Color currentColour = new Color(0, 0, 0, 0); //Just setting it at first
			for (int j = 0; j < vertices.Length; j++)
			{
				//If i is can be divided by 6, then we want to change the colour
				if (j % 6 == 0)
				{
					if (vertices[j].y < waterHeight)
						currentColour = new Color(1, 0, 0, 0);
					else if (vertices[j].y < grassHeight)
						currentColour = new Color(0, 1, 0, 0);
					else
						currentColour = new Color(0, 0, 1, 0);
				}

				colours.Add(currentColour);
			}
			mesh.SetColors(colours);

			updateCallback ();
		}

		public void RequestMesh(MapData mapData) {
			hasRequestedMesh = true;
			mapGenerator.RequestMeshData (mapData, lod, OnMeshDataReceived);
		}

	}

	[System.Serializable]
	public struct LODInfo {
		public int lod;
		public float visibleDstThreshold;
		public bool useForCollider;

		public LODInfo(int lod, float visibleDstThreshold, bool useForCollider)
		{
			this.lod = lod;
			this.visibleDstThreshold = visibleDstThreshold;
			this.useForCollider = useForCollider;
		}
	}

}
