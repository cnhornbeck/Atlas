using UnityEngine;
using System.Collections.Generic;

public struct ChunkRegistry
{
    private static Dictionary<Vector2, Chunk> generatedChunks = new();
    public static Dictionary<Vector2, Chunk> GetGeneratedChunksDictionary() => generatedChunks;

    public void AddChunk(Vector2 position, Chunk chunk)
    {
        generatedChunks[position] = chunk;
    }

    public bool HasChunkAt(Vector2 position)
    {
        return generatedChunks.ContainsKey(position);
    }

    public Chunk GetChunkAt(Vector2 position)
    {
        return generatedChunks[position];
    }

    // public void UpdateVisibleLastFrame(HashSet<Vector2> newVisibleChunks)
    // {
    //     chunksVisibleLastFrame = newVisibleChunks;
    // }
}
