using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Noise 
{

	public enum NormalizeMode {Local, Global};

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode, List<City> cities) 
	{
		//Center * 2.5f = The world position
		float[,] noiseMap = new float[mapWidth,mapHeight];

		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < octaves; i++) 
		{
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) - offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if (scale <= 0) 
		{
			scale = 0.0001f;
		}
		
		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) 
		{
			for (int x = 0; x < mapWidth; x++) 
			{

				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight) 
				{
					maxLocalNoiseHeight = noiseHeight;
				} 
				else if (noiseHeight < minLocalNoiseHeight) 
				{
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap [x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapHeight; y++) 
		{
			for (int x = 0; x < mapWidth; x++) 
			{
				if (normalizeMode == NormalizeMode.Local) 
				{
					noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]);
				} 
				else 
				{
					float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight/0.9f);
					noiseMap [x, y] = Mathf.Clamp(normalizedHeight,0, int.MaxValue);
				}
			}
		}
		
		//Flattening the mesh for the cities
		Vector3 worldPosition = new Vector3(offset.x * 2.5f,0,offset.y * 2.5f);
		City cityInChunk = null;
		
		//Finding out which city would be in this chunk
		foreach (City city in cities)
		{
//			CustomDebug.LogColour(Color.grey, string.Format("{0} X:{1} Y:{2} World X:{3} Y:{4}",
//				city.name, city.position.x, city.position.z, worldPosition.x - 50, worldPosition.z - 50));


			//Finds if there is a city inside of the float array we are generating
			//If the chunk is within a distance of the city
			if (Vector3.Distance((city.position + (new Vector3(50, 0, 50) * RoadGenerator.cityScale)), worldPosition) < 1000)
			{
				cityInChunk = city;
				//CustomDebug.LogColour(Color.yellow, string.Format("{0} will changed a chunk at {1}", city.name, worldPosition));
				break;
			}
		}
		if (cityInChunk != null)
		{
			//Need to work out where the city starts inside the chunk
			float cityStartX = 0;
			float cityStartZ = 0;
			int amountX = 97;
			int amountZ = 97;

			CustomDebug.LogColour(Color.green, string.Format("CityStart X:{0} Z:{1}  Amount X:{2} Z:{3} City Position X:{4} Z:{5} Chunk Position X:{6} Z:{7} NoiseMap Length {8}",
				cityStartX,cityStartZ,amountX,amountZ, cityInChunk.position.x, cityInChunk.position.z, worldPosition.x, worldPosition.z, noiseMap.GetLength(0)));

			for (int xIndex = 0; xIndex < amountX; xIndex++)
			{
				for (int zIndex = 0; zIndex < amountZ; zIndex++)
				{
					//This is where we set the height of the mesh for the city
					//Later this needs to find the average instead of just being at 50% of the max height
					noiseMap[xIndex + Mathf.RoundToInt(cityStartX),zIndex + Mathf.RoundToInt(cityStartZ)] = 1;
				}
			}
		}

		return noiseMap;
	}

}
