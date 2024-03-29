public class NoiseSettings
{
    public int ChunkSize = ChunkGlobals.meshSpaceChunkSize;
    // Ratio between world space and mesh space
    public float Scale = 0.04f;
    public float Lacunarity = 2f;
    public float Persistence = 0.5f;
    public int Octaves = 15;
    public int Seed = 1337;
}
