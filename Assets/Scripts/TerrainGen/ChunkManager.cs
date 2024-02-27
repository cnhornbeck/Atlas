using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    int renderDistance;
    NoiseSettings noiseSettings;
    [SerializeField] List<TextureGen.TerrainLevel> colorList = new();
    static Dictionary<Vector2, Chunk> generatedChunks = new();
    static HashSet<Vector2> chunksVisibleLastFrame = new();

    void Start()
    {
        noiseSettings = NoiseSettings.CreateDefault();
        renderDistance = ChunkGlobals.renderDistance;
        TextureGen.PreprocessColors(colorList);
    }

    void Update()
    {
        renderDistance = ChunkGlobals.renderDistance;
        Vector3 cameraPos = Camera.main.transform.position;
        UpdateVisibleChunks(cameraPos);
    }

    void UpdateVisibleChunks(Vector3 cameraPos)
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
            }
            else
            {
                // If chunk has not been generated, generate it
                Chunk newChunk = GenerateChunk(position);
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

    Chunk GenerateChunk(Vector2 position)
    {
        string chunkName = "Terrain Chunk: (" + (int)position.x / ChunkGlobals.worldSpaceChunkSize + ", " + (int)position.y / ChunkGlobals.worldSpaceChunkSize + ")";
        GameObject terrainChunk = new(chunkName);
        terrainChunk.transform.parent = transform;

        Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
        chunkComponent.Initialize(terrainChunk, noiseSettings, position, 60, colorList);

        return chunkComponent;
    }

    public static HashSet<Vector2> GetChunkPositionsWithinRadius(Vector3 currentPositionVec3, int renderDistance)
    {
        HashSet<Vector2> chunkPositionsWithinRadius = new();
        int chunksVisibleInRenderDist = renderDistance;
        float xPos = Mathf.RoundToInt(currentPositionVec3.x / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;
        float yPos = Mathf.RoundToInt(currentPositionVec3.z / ChunkGlobals.worldSpaceChunkSize) * ChunkGlobals.worldSpaceChunkSize;

        for (int xOffset = -chunksVisibleInRenderDist; xOffset <= chunksVisibleInRenderDist; xOffset++)
        {
            for (int yOffset = -chunksVisibleInRenderDist; yOffset <= chunksVisibleInRenderDist; yOffset++)
            {

                if (InCircleOfRadius(currentPositionVec3, new Vector3(xPos + xOffset * ChunkGlobals.worldSpaceChunkSize, 20, yPos + yOffset * ChunkGlobals.worldSpaceChunkSize), renderDistance * ChunkGlobals.worldSpaceChunkSize))
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
