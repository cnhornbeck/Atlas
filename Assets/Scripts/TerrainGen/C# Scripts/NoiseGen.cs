public class NoiseSettings
{
    public int ChunkSize = ChunkGlobals.meshSpaceChunkSize;
    // Ratio between world space and mesh space
    public float Scale = 0.05f;
    public float Lacunarity = 2.5f;
    public float Persistence = 0.4f;
    public int Octaves = 6;
    public int Seed = 1337;
}
