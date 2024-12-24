using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    int renderDistance;
    static Dictionary<Vector2, Chunk> generatedChunks = new();
    static HashSet<Vector2> chunksVisibleLastFrame = new();
    static List<ChunkConstructor> finishedChunkConstructors = new();

    void Start()
    {
        renderDistance = ChunkGlobals.renderDistance;
    }

    void Update()
    {
        renderDistance = ChunkGlobals.renderDistance;
        Vector3 cameraPos = Camera.main.transform.position;
        UpdateChunkVisibility(cameraPos);
        ChunkConstructorManager.StartTextureGeneration();
        finishedChunkConstructors = ChunkConstructorManager.GetFinishedChunks();
        HandleFinishedChunks(finishedChunkConstructors);
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
            }
            else
            {
                Chunk newChunk = QueueChunkGeneration(position);
                generatedChunks.Add(position, newChunk);
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

    Chunk QueueChunkGeneration(Vector2 position)
    {
        string chunkName = "Terrain Chunk: (" + (int)position.x / ChunkGlobals.worldSpaceChunkSize + ", " + (int)position.y / ChunkGlobals.worldSpaceChunkSize + ")";
        GameObject terrainChunk = new(chunkName);
        terrainChunk.transform.parent = transform;

        Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
        chunkComponent.SetParent(terrainChunk);
        ChunkConstructorManager.StartChunkGeneration(position);

        return chunkComponent;
    }

    void HandleFinishedChunks(List<ChunkConstructor> finishedChunkConstructors)
    {
        foreach (ChunkConstructor chunkConstructor in finishedChunkConstructors)
        {
            Vector2 chunkPosition = chunkConstructor.GetWorldSpacePosition();
            Chunk chunk = generatedChunks[chunkPosition];
            chunk.Initialize(chunkConstructor.GetMeshes(), chunkConstructor.GetTexture(), chunkPosition);
            chunksVisibleLastFrame.Add(chunkPosition);
        }
        finishedChunkConstructors.Clear();
    }

    public static HashSet<Vector2> GetVisibleChunkPositionsWithinRadius(Vector3 currentPositionVec3, int renderDistance)
    {
        int estimatedCapacity = (int)(Mathf.PI * renderDistance * renderDistance);
        HashSet<Vector2> visibleChunkPositionsWithinRadius = new HashSet<Vector2>(estimatedCapacity);

        // Get current chunk position in "chunk space"
        int currentChunkX = Mathf.RoundToInt(currentPositionVec3.x / ChunkGlobals.worldSpaceChunkSize);
        int currentChunkZ = Mathf.RoundToInt(currentPositionVec3.z / ChunkGlobals.worldSpaceChunkSize);

        float squaredRenderDistance = renderDistance * renderDistance;

        // Loop over chunk offsets in a square
        for (int xOffset = -renderDistance; xOffset <= renderDistance; xOffset++)
        {
            for (int zOffset = -renderDistance; zOffset <= renderDistance; zOffset++)
            {
                // Compute squared distance in chunk space
                float distanceSquared = xOffset * xOffset + zOffset * zOffset;

                // Check if within the circular render distance
                if (distanceSquared <= squaredRenderDistance)
                {
                    int chunkX = currentChunkX + xOffset;
                    int chunkZ = currentChunkZ + zOffset;

                    // Convert back to world space
                    float xCoord = chunkX * ChunkGlobals.worldSpaceChunkSize;
                    float zCoord = chunkZ * ChunkGlobals.worldSpaceChunkSize;

                    visibleChunkPositionsWithinRadius.Add(new Vector2(xCoord, zCoord));
                }
            }
        }

        return visibleChunkPositionsWithinRadius;
    }
}

