using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    int renderDistance;
    [SerializeField] List<TextureGen.TerrainLevel> colorList = new();
    static Dictionary<Vector2, Chunk> generatedChunks = new();
    static HashSet<Vector2> chunksVisibleLastFrame = new();

    void Start()
    {
        renderDistance = ChunkGlobals.renderDistance;
        TextureGen.PreprocessColors(colorList);
    }

    void Update()
    {
        renderDistance = ChunkGlobals.renderDistance;
        Vector3 cameraPos = Camera.main.transform.position;
        UpdateChunkVisibility(cameraPos);
    }

    void UpdateChunkVisibility(Vector3 cameraPos)
    {
        HashSet<Vector2> visibleChunkPositions = GetVisibleChunkPositionsWithinRadius(cameraPos, renderDistance);

        // Loops over the position of each chunk that should be visible
        foreach (Vector2 position in visibleChunkPositions)
        {
            // Checks if a chunk has been generated at position
            if (generatedChunks.TryGetValue(position, out Chunk chunk))
            {
                // Checks if chunk is not visible
                if (!chunk.IsVisible())
                {
                    // Enables the chunk
                    chunk.SetVisible(true);
                }
                // UpdateChunkLOD(cameraPos, chunk);
            }
            else
            {
                // If chunk has not been generated, generate it with an lod of 3 and add it to the generatedChunks dictionary
                // lod ranges from 0 to 3, 3 being the lowest resolution
                Chunk newChunk = GenerateChunk(position, ChunkGlobals.lodNumSize - 1);
                generatedChunks.Add(position, newChunk);
                // UpdateChunkLOD(cameraPos, newChunk);
            }
        }

        // This gets all chunks that are in chunksVisibleLastFrame that are not in visibleChunkPositions and stores that value in chunksVisibleLastFrame
        chunksVisibleLastFrame.ExceptWith(visibleChunkPositions);

        // Goes through every position in chunksVisibleLastFrame, which now contains only chunks that were visible last frame that should NOT be visible this frame, and disables every chunk at each position
        foreach (Vector2 position in chunksVisibleLastFrame)
        {
            generatedChunks[position].SetVisible(false);
        }

        // This is that last thing to happen before the next frame so now all chunks that are visible this frame will be visible in the last frame one frame from now
        chunksVisibleLastFrame = visibleChunkPositions;
    }

    // void UpdateChunkLOD(Vector3 cameraPos, Chunk chunk)
    // {
    //     int lod = CalculateLOD(cameraPos, chunk.WorldSpacePosition, chunk.AverageHeight);
    //     chunk.SetLOD(lod);
    // }

    int CalculateLOD(Vector3 cameraPos, Vector2 chunkPosition, float averageHeight)
    {
        float squaredDistance = (cameraPos - new Vector3(chunkPosition.x, averageHeight, chunkPosition.y)).sqrMagnitude;
        int lod = 0;

        if (squaredDistance > ChunkGlobals.worldSpaceChunkSize * ChunkGlobals.renderDistance * ChunkGlobals.worldSpaceChunkSize * ChunkGlobals.renderDistance)
        {
            lod = ChunkGlobals.lodNumSize - 1;
            return lod;
        }

        for (int i = 0; i < ChunkGlobals.lodCutoffArray.Length; i++)
        {
            float lodCutoff = ChunkGlobals.lodCutoffArray[i] * ChunkGlobals.worldSpaceChunkSize * ChunkGlobals.renderDistance;

            if (squaredDistance < lodCutoff * lodCutoff)
            {
                lod = i;
                break;
            }
        }
        return lod;
    }

    Chunk GenerateChunk(Vector2 position, int lod)
    {
        string chunkName = "Terrain Chunk: (" + (int)position.x / ChunkGlobals.worldSpaceChunkSize + ", " + (int)position.y / ChunkGlobals.worldSpaceChunkSize + ")";
        GameObject terrainChunk = new(chunkName);
        terrainChunk.transform.parent = transform;

        Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
        chunkComponent.Initialize(terrainChunk, position);

        return chunkComponent;
    }

    public static HashSet<Vector2> GetVisibleChunkPositionsWithinRadius(Vector3 currentPositionVec3, int renderDistance)
    {
        HashSet<Vector2> visibleChunkPositionsWithinRadius = new();
        // int chunksVisibleInRenderDist = renderDistance;
        float squaredRenderDistance = renderDistance * ChunkGlobals.worldSpaceChunkSize * (renderDistance * ChunkGlobals.worldSpaceChunkSize);

        float xPos = Mathf.RoundToInt(currentPositionVec3.x / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;
        float yPos = Mathf.RoundToInt(currentPositionVec3.z / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;
        // float averageHeight = ChunkGlobals.heightMultiplier * 0.6f;

        for (int xOffset = -renderDistance; xOffset <= renderDistance; xOffset++)
        {
            float xCoord = xPos + xOffset * ChunkGlobals.worldSpaceChunkSize;
            // Calculate the span of the circle at this xOffset
            int ySpan = Mathf.RoundToInt(Mathf.Sqrt(squaredRenderDistance - Mathf.Pow(xOffset * ChunkGlobals.worldSpaceChunkSize, 2)) / ChunkGlobals.worldSpaceChunkSize);

            for (int yOffset = -ySpan; yOffset <= ySpan; yOffset++)
            {
                float zCoord = yPos + yOffset * ChunkGlobals.worldSpaceChunkSize;
                Vector2 chunkPosition = new(xCoord, zCoord);
                visibleChunkPositionsWithinRadius.Add(chunkPosition);
            }
        }

        return visibleChunkPositionsWithinRadius;
    }

}

