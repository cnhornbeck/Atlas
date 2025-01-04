using Unity.Mathematics;

public struct ChunkGlobals
{
    public const int lodCount = 1;

    // Size of chunk is measured by number of edges per chunk at highest detail LOD
    public static readonly int meshSpaceChunkSize = 64;

    // Size of the actual mesh in the scene
    public const float WorldSpaceChunkSize = 10;
    public const int heightMultiplier = 35;
    public static byte renderDistance = 10;
    public static int[][] triangleArrays;
    public static float2[][] uvArrays;
}
