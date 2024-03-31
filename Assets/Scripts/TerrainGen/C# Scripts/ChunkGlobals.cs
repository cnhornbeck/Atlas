using System;

public class ChunkGlobals
{
    public const int lodCount = 6;

    // Size of chunk is measured by number of edges per chunk
    public static readonly int meshSpaceChunkSize = (int)Math.Pow(2, lodCount - 1);

    // Size of the actual mesh in the scene
    public const float worldSpaceChunkSize = 10;
    public const int heightMultiplier = 15;
    public static int renderDistance = 15;
}
