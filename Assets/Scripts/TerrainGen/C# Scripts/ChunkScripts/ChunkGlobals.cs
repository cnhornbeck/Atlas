using Unity.Mathematics;

public struct ChunkGlobals
{
    public const int lodCount = 1;

    // Size of chunk is measured by number of edges per chunk at highest detail LOD
    public static readonly int meshSpaceChunkSize = 64;

    // Size of the actual mesh in the scene
    public const float WorldSpaceChunkSize = 1000;
    public const int heightMultiplier = 700;
    public static byte renderDistance = 20;
    public static int[][] triangleArrays;
    public static float2[][] uvArrays;
}
