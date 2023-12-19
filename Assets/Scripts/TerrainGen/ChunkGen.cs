using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class ChunkGen : MonoBehaviour
{
    [SerializeField] int renderDistance;
    [SerializeField] NoiseSettings noiseSettings;
    [SerializeField] List<TerrainLevel> colorList = new();
    Hashtable chunksHash = new();
    List<GameObject> chunksList = new();

    void Start()
    {
        noiseSettings = NoiseSettings.CreateDefault();
    }

    void Update()
    {
        GenerateChunks();
    }

    void GenerateChunks()
    {
        foreach (Vector2 pos in GetChunksWithinRadius(Camera.main.transform.position, renderDistance))
        {
            if (!chunksHash.ContainsKey(pos))
            {
                string chunkName = "Terrain Chunk: (" + (int)pos.x / ChunkGlobals.worldSpaceChunkSize + ", " + (int)pos.y / ChunkGlobals.worldSpaceChunkSize + ")";
                GameObject terrainChunk = new(chunkName);
                terrainChunk.transform.parent = transform;

                Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
                chunkComponent.Initialize(terrainChunk, noiseSettings, pos, 20, colorList);

                chunksHash.Add(pos, terrainChunk);
                chunksList.Add(terrainChunk);
            }
        }
    }

    public static List<Vector2> GetChunksWithinRadius(Vector3 currentPositionVec3, int renderDistance)
    {
        List<Vector2> chunksWithinRadius = new();
        float radius = renderDistance * ChunkGlobals.worldSpaceChunkSize;
        Vector2 currentPosition = new(currentPositionVec3.x, currentPositionVec3.z);
        int xPos = (int)(Mathf.Round(currentPosition.x / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize);
        int yPos = (int)(Mathf.Round(currentPosition.y / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize);

        // Assuming each chunk is worldSpaceChunkSize units in size
        for (float x = xPos - radius; x <= xPos + radius; x += ChunkGlobals.worldSpaceChunkSize)
        {
            for (float y = yPos - radius; y <= yPos + radius; y += ChunkGlobals.worldSpaceChunkSize)
            {
                if (IsWithinRadius(currentPosition, x, y, radius))
                {
                    chunksWithinRadius.Add(new Vector2(x, y));
                }
            }
        }

        return chunksWithinRadius;
    }

    static bool IsWithinRadius(Vector2 currentPosition, float x, float y, float radius)
    {
        // Calculate Euclidean distance from the center
        float dx = x - currentPosition.x;
        float dy = y - currentPosition.y;
        return dx * dx + dy * dy <= radius * radius;
    }

    public void CalcUVs(int width, int height, GameObject terrain)
    {
        Mesh terrainMesh = terrain.GetComponent<MeshFilter>().sharedMesh;
        // Generate the UV coordinates for the mesh
        Vector2[] uvs = new Vector2[width * height];
        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                uvs[index] = new Vector2((float)x / (float)width, (float)y / (float)height);
                index++;
            }
        }
        // Set the UV coordinates of the mesh
        terrainMesh.SetUVs(0, uvs);
    }

    public void ClearCachedChunks()
    {
        Debug.Log("Clearing cached chunks");
        foreach (GameObject item in chunksList)
        {
            GameObject.Destroy(item);
        }
        chunksHash.Clear();
        chunksList.Clear();
    }


}
