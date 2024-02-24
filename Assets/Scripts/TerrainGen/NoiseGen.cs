using UnityEngine;

public class NoiseGenerator
{
    // Generates a 2D array of Perlin noise values with the given settings and detail level
    public static float[] GeneratePerlinNoise(NoiseSettings settings, int detailLevel)
    {
        int adjustedChunkSize = CalculateAdjustedChunkSize(detailLevel);
        float[] noiseMap = new float[adjustedChunkSize * adjustedChunkSize];

        // Generate and apply Perlin noise
        float maxPossibleHeight = ApplyPerlinNoise(settings, adjustedChunkSize, ref noiseMap);

        // Normalize noise map values
        NormalizeNoiseMap(adjustedChunkSize, ref noiseMap, maxPossibleHeight);

        return noiseMap;
    }

    private static int CalculateAdjustedChunkSize(int detailLevel)
    {
        return Mathf.Max((ChunkGlobals.ChunkSize + 1) / detailLevel, 1);
    }

    private static float ApplyPerlinNoise(NoiseSettings settings, int chunkSize, ref float[] noiseMap)
    {
        System.Random random = new System.Random(settings.Seed);
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int octave = 0; octave < settings.Octaves; octave++)
        {
            int randomX = random.Next(-100, 100);
            int randomY = random.Next(-100, 100);

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float sampleX = (x / (float)chunkSize * settings.Scale + settings.XOffset + randomX * amplitude) * frequency;
                    float sampleY = (y / (float)chunkSize * settings.Scale + settings.YOffset + randomY * amplitude) * frequency;

                    noiseMap[y * chunkSize + x] += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;
                }
            }

            maxPossibleHeight += amplitude;
            amplitude *= settings.Persistence;
            frequency *= settings.Lacunarity;
        }

        return maxPossibleHeight;
    }

    private static void NormalizeNoiseMap(int chunkSize, ref float[] noiseMap, float maxPossibleHeight)
    {
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                noiseMap[y * chunkSize + x] = Mathf.Clamp(noiseMap[y * chunkSize + x] / maxPossibleHeight, 0f, 1f);
            }
        }
    }
}

[System.Serializable]
public struct NoiseSettings
{
    [HideInInspector] public int ChunkSize;
    public float XOffset;
    public float YOffset;
    [Space]
    [Range(0.01f, 20f)] public float Scale;
    [Range(1f, 5f)] public float Lacunarity;
    [Range(0.01f, 1f)] public float Persistence;
    [Range(1, 10)] public int Octaves;
    public int Seed;

    // Initialize fields with default values
    public NoiseSettings(int dummy)
    {
        ChunkSize = ChunkGlobals.ChunkSize;
        XOffset = 0f;
        YOffset = 0f;
        Scale = 0.3f;
        Lacunarity = 2f;
        Persistence = 0.3f;
        Octaves = 7;
        Seed = 0;
    }

    // Static factory method for default settings
    public static NoiseSettings CreateDefault()
    {
        return new NoiseSettings(0);
    }
}
