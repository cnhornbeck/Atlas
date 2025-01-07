using Unity.Mathematics;

public struct ChunkGlobals
{
    public const int lodCount = 1;

    // Size of chunk is measured by number of edges per chunk at highest detail LOD
    public static readonly int meshSpaceChunkSize = 32;

    // Size of the actual mesh in the scene
    public const float WorldSpaceChunkSize = 100;
    public const int heightMultiplier = 500;
    public static byte renderDistance = 20;
    public static int[][] triangleArrays;
    public static float2[][] uvArrays;
}
