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
        HashSet<Vector2> visibleChunkPositions = GetChunkPositionsWithinRadius(cameraPos, renderDistance);

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
                UpdateChunkLOD(cameraPos, chunk);
            }
            else
            {
                // If chunk has not been generated, generate it with an lod of 3 and add it to the generatedChunks dictionary
                // lod ranges from 0 to 3, 3 being the lowest resolution
                Chunk newChunk = GenerateChunk(position, ChunkGlobals.lodNumSize - 1);
                generatedChunks.Add(position, newChunk);
                UpdateChunkLOD(cameraPos, newChunk);
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

    void UpdateChunkLOD(Vector3 cameraPos, Chunk chunk)
    {
        int lod = CalculateLOD(cameraPos, chunk.WorldSpacePosition, chunk.averageHeight);
        chunk.SetLOD(lod);
    }

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
        chunkComponent.Initialize(terrainChunk, position, lod);

        return chunkComponent;
    }

    public static HashSet<Vector2> GetChunkPositionsWithinRadius(Vector3 currentPositionVec3, int renderDistance)
    {
        HashSet<Vector2> chunkPositionsWithinRadius = new();
        int chunksVisibleInRenderDist = renderDistance;
        float averageHeight;

        float xPos = Mathf.RoundToInt(currentPositionVec3.x / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;
        float yPos = Mathf.RoundToInt(currentPositionVec3.z / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;

        for (int xOffset = -chunksVisibleInRenderDist; xOffset <= chunksVisibleInRenderDist; xOffset++)
        {
            for (int yOffset = -chunksVisibleInRenderDist; yOffset <= chunksVisibleInRenderDist; yOffset++)
            {
                float xCoord = xPos + xOffset * ChunkGlobals.worldSpaceChunkSize;
                float zCoord = yPos + yOffset * ChunkGlobals.worldSpaceChunkSize;

                averageHeight = generatedChunks.TryGetValue(new Vector2(xCoord, zCoord), out Chunk chunk) ? chunk.averageHeight : ChunkGlobals.heightMultiplier * 0.6f;

                if (InCircleOfRadius(currentPositionVec3, new Vector3(xCoord, averageHeight, zCoord), renderDistance * ChunkGlobals.worldSpaceChunkSize))
                {
                    Vector2 chunkPosition = new(xPos + xOffset * ChunkGlobals.worldSpaceChunkSize, yPos + yOffset * ChunkGlobals.worldSpaceChunkSize);
                    chunkPositionsWithinRadius.Add(chunkPosition);
                }
            }
        }

        return chunkPositionsWithinRadius;
    }

    static bool InCircleOfRadius(Vector3 currentPosition, Vector3 chunkPosition, float radius)
    {
        return (currentPosition - chunkPosition).sqrMagnitude <= radius * radius;
    }
}
