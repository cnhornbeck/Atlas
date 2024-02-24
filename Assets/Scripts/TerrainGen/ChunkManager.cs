using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    int renderDistance;
    NoiseSettings noiseSettings;
    [SerializeField] List<TerrainLevel> colorList = new();
    static Dictionary<Vector2, GameObject> chunkDict = new();
    public static List<GameObject> chunkList = new();
    public static List<Chunk> chunksVisibleLastFrame = new();

    void Start()
    {
        noiseSettings = NoiseSettings.CreateDefault();
        renderDistance = ChunkGlobals.renderDistance;
    }

    void Update()
    {
        renderDistance = ChunkGlobals.renderDistance;
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        foreach (Chunk chunk in chunksVisibleLastFrame)
        {
            chunk.SetVisible(false);
        }
        chunksVisibleLastFrame.Clear();

        foreach (Vector2 pos in GetChunkPositionsWithinRadius(Camera.main.transform.position, renderDistance))
        {
            if (chunkDict.ContainsKey(pos))
            {
                chunkDict[pos].GetComponent<Chunk>().UpdateChunk();
                if (chunkDict[pos].GetComponent<Chunk>().IsVisible())
                {
                    chunksVisibleLastFrame.Add(chunkDict[pos].GetComponent<Chunk>());
                }
            }
            else
            {
                string chunkName = "Terrain Chunk: (" + (int)pos.x / ChunkGlobals.worldSpaceChunkSize + ", " + (int)pos.y / ChunkGlobals.worldSpaceChunkSize + ")";
                GameObject terrainChunk = new(chunkName);
                terrainChunk.transform.parent = transform;

                Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
                chunkComponent.Initialize(terrainChunk, noiseSettings, pos, 60, colorList);

                chunkDict.Add(pos, terrainChunk);
                chunkList.Add(terrainChunk);
            }
        }
    }

    public static List<Vector2> GetChunkPositionsWithinRadius(Vector3 currentPositionVec3, int renderDistance)
    {
        List<Vector2> chunkPositionsWithinRadius = new();
        int chunksVisibleInRenderDist = renderDistance;
        Vector2 currentPosition = new(currentPositionVec3.x, currentPositionVec3.z);
        float xPos = Mathf.RoundToInt(currentPosition.x / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;
        float yPos = Mathf.RoundToInt(currentPosition.y / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;

        for (int xOffset = -chunksVisibleInRenderDist; xOffset <= chunksVisibleInRenderDist; xOffset++)
        {
            for (int yOffset = -chunksVisibleInRenderDist; yOffset <= chunksVisibleInRenderDist; yOffset++)
            {
                Vector2 viewedChunkCoord = new(xPos + xOffset * ChunkGlobals.worldSpaceChunkSize, yPos + yOffset * ChunkGlobals.worldSpaceChunkSize);
                chunkPositionsWithinRadius.Add(viewedChunkCoord);
            }
        }

        return chunkPositionsWithinRadius;
    }

    // static bool IsWithinRadius(Vector2 currentPosition, Vector2 position, float radius)
    // {
    //     float x = position.x;
    //     float y = position.y;
    //     // Calculate Euclidean distance from the center
    //     float dx = x - currentPosition.x;
    //     float dy = y - currentPosition.y;
    //     return dx * dx + dy * dy < radius * radius;
    // }

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
                uvs[index] = new Vector2(x / (float)width, y / (float)height);
                index++;
            }
        }
        // Set the UV coordinates of the mesh
        terrainMesh.SetUVs(0, uvs);
    }

    public void ClearCachedChunks()
    {
        Debug.Log("Clearing cached chunks");
        foreach (GameObject item in chunkList)
        {
            Destroy(item);
        }
        chunkDict.Clear();
        chunkList.Clear();
    }
}
