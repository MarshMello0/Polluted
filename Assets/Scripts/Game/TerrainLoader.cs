using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainLoader : MonoBehaviour
{
    private EndlessTerrain endlessTerrain;
    private Loading loading;
    public int viewDistance = 1;
    public List<EndlessTerrain.TerrainChunk> chunks = new List<EndlessTerrain.TerrainChunk>();

    public int loopAmount;
    private void Start()
    {
        endlessTerrain = FindObjectOfType<EndlessTerrain>();
        if (SceneManager.sceneCount == 2)
        {
            loading = FindObjectOfType<Loading>();
            CalActions();
        }
        StartCoroutine(endlessTerrain.UpdateVisibleChunks(new Vector2(transform.position.x, transform.position.z) / 2.5f, viewDistance, chunks, DoneOneChunk));
    }

    private void CalActions()
    {
        loopAmount = viewDistance + 1 + viewDistance;
        loopAmount *= loopAmount;
        loading.totalNumberOfActions += loopAmount;
        //loading.totalLoaderChunkActions += loopAmount;
        //if (loading.numberOfChunkActionsCompleted == -1)
        //    loading.numberOfChunkActionsCompleted = 0;
    }

    public void DoneOneChunk()
    {
        //loading.numberOfChunkActionsCompleted++;
    }
}
