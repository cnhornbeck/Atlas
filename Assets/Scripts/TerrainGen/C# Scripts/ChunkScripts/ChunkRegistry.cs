using System.Collections.Generic;
using Unity.Mathematics;

public struct ChunkRegistry
{
    private static Dictionary<float2, Chunk> generatedChunks = new();
    public static Dictionary<float2, Chunk> GetGeneratedChunksDictionary() => generatedChunks;
}
