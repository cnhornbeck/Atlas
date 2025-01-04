using UnityEngine;
using System.Collections.Generic;

public class ChunkVisibilityManager : MonoBehaviour
{
    static HashSet<Vector2> chunksVisibleLastFrame = new();

    public static void UpdateChunkVisibility(Vector3 cameraPos)
    {
        // TODO: This can be smaller type, such as byte
        int renderDistance = ChunkGlobals.renderDistance;

        HashSet<Vector2> visibleChunkPositions = GetVisibleChunkPositionsWithinRadius(cameraPos, renderDistance);
        // Loops over the position of each chunk that should be visible
        foreach (Vector2 position in visibleChunkPositions)
        {
            // Checks if a chunk has been generated at position
            if (ChunkRegistry.GetGeneratedChunksDictionary().TryGetValue(position, out Chunk chunk))
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
                // TODO: This functionality should be moved elsewhere. This class should not be responsible for generating chunks

                ChunkConstructorManager.QueueChunkGeneration(position);
                // ChunkRegistry.generatedChunks.Add(position, newChunk);
            }
        }
        // This gets all chunks that are in chunksVisibleLastFrame that are not in visibleChunkPositions and stores that value in chunksVisibleLastFrame
        chunksVisibleLastFrame.ExceptWith(visibleChunkPositions);
        // Goes through every position in chunksVisibleLastFrame, which now contains only chunks that were visible last frame that should NOT be visible this frame, and disables every chunk at each position
        foreach (Vector2 position in chunksVisibleLastFrame)
        {
            ChunkRegistry.GetGeneratedChunksDictionary()[position].SetVisible(false);
        }
        // This is that last thing to happen before the next frame so now all chunks that are visible this frame will be visible in the last frame one frame from now
        chunksVisibleLastFrame = visibleChunkPositions;
    }

    private static HashSet<Vector2> GetVisibleChunkPositionsWithinRadius(Vector3 currentPositionVec3, int renderDistance)
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

