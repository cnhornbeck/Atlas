public class ChunkGlobals
{
    public const int lodCount = 4;

    // Size of chunk is measured by number of edges per chunk
    public const int meshSpaceChunkSize = 64;

    // Size of the actual mesh in the scene
    public const float worldSpaceChunkSize = 10;
    public const float heightMultiplier = 50;
    public static int renderDistance = 8;
}
