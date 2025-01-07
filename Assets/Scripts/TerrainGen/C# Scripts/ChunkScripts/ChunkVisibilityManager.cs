using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class ChunkVisibilityManager : MonoBehaviour
{
    static HashSet<float2> chunksVisibleLastFrame = new();
    static HashSet<float2> chunksVisibleThisFrame = new();
    static HashSet<float2> chunksStartedThisFrame = new();

    public static void UpdateChunkVisibility(float3 cameraPos)
    {
        byte renderDistance = ChunkGlobals.renderDistance;

        chunksVisibleThisFrame = GetVisibleChunkPositionsWithinRadius(cameraPos, renderDistance);
        // Loops over the position of each chunk that should be visible
        foreach (float2 position in chunksVisibleThisFrame)
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
            else if (!ChunkConstructorManager.IsQueuedForConstruction(position))
            {
                ChunkConstructorManager.AddChunkToQueue(position);
                // print("Requesting chunk generation at " + position);
            }
        }

        chunksStartedThisFrame = ChunkConstructorManager.StartChunkConstructionJobs();
        // This gets all chunks that are in chunksVisibleLastFrame that are not in visibleChunkPositions and stores that value in chunksVisibleLastFrame
        chunksVisibleLastFrame.ExceptWith(chunksVisibleThisFrame);
        chunksVisibleLastFrame.IntersectWith(chunksStartedThisFrame);
        // Goes through every position in chunksVisibleLastFrame, which now contains only chunks that were visible last frame that should NOT be visible this frame, and disables every chunk at each position
        foreach (float2 position in chunksVisibleLastFrame)
        {
            ChunkRegistry.GetGeneratedChunksDictionary()[position].SetVisible(false);
        }
        // This is that last thing to happen before the next frame so now all chunks that are visible this frame will be visible in the last frame one frame from now
        chunksVisibleLastFrame = chunksVisibleThisFrame;
    }

    private static HashSet<float2> GetVisibleChunkPositionsWithinRadius(float3 currentPositionVec3, byte renderDistance)
    {
        ushort estimatedCapacity = (ushort)(Mathf.PI * renderDistance * renderDistance);
        HashSet<float2> visibleChunkPositionsWithinRadius = new HashSet<float2>(estimatedCapacity);

        // Get current chunk position in "chunk space"
        int currentChunkX = Mathf.RoundToInt(currentPositionVec3.x / ChunkGlobals.WorldSpaceChunkSize);
        int currentChunkZ = Mathf.RoundToInt(currentPositionVec3.z / ChunkGlobals.WorldSpaceChunkSize);

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
                    float xCoord = chunkX * ChunkGlobals.WorldSpaceChunkSize;
                    float zCoord = chunkZ * ChunkGlobals.WorldSpaceChunkSize;

                    visibleChunkPositionsWithinRadius.Add(new float2(xCoord, zCoord));
                }
            }
        }

        return visibleChunkPositionsWithinRadius;
    }
}

