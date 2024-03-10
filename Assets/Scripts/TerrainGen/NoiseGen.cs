using UnityEngine;

public class NoiseGen
{

    private static NoiseSettings settings = NoiseSettings.CreateDefault();
    // private static readonly System.Random random = new(settings.Seed);

    // Generates a 2D array of Perlin noise values with the given settings and detail level
    public static float[] GeneratePerlinNoise(Vector2 worldSpacePosition, int detailLevel)
    {
        // Length of the mesh measured in vertices
        int vertexNum = CalculateVertexNum(detailLevel);

        float[] noiseMap = new float[vertexNum * vertexNum];

        // Debug.Log($"Chunk size: {vertexNum}");

        // Generate and apply Perlin noise
        float maxPossibleHeight = ApplyPerlinNoise(worldSpacePosition, vertexNum, ref noiseMap);

        // Normalize noise map values
        NormalizeNoiseMap(vertexNum, ref noiseMap, maxPossibleHeight);

        return noiseMap;
    }

    private static int CalculateVertexNum(int detailLevel)
    {
        return Mathf.Max((ChunkGlobals.meshSpaceChunkSize / detailLevel) + 1, 1);
    }

    private static float ApplyPerlinNoise(Vector2 worldSpacePosition, int vertexNum, ref float[] noiseMap)
    {
        System.Random random = new(settings.Seed);
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int octave = 0; octave < settings.Octaves; octave++)
        {
            int randomX = random.Next(-1000, 1000);
            int randomY = random.Next(-1000, 1000);

            for (int y = 0; y < vertexNum; y++)
            {
                for (int x = 0; x < vertexNum; x++)
                {
                    // Represents the bottom left corner of the mesh in world space. Ex: (initialCoord, initialCoord) = (-5, -5)
                    float initialCoord = -ChunkGlobals.worldSpaceChunkSize / 2f;

                    // This gives the step size in world space. Ex: 1*10/2 = 5. This means 5 world space units between vertices.
                    float stepSize = ChunkGlobals.worldSpaceChunkSize / (vertexNum - 1);

                    // Takes the starting point (bottom left corner of the mesh in world space) and adds the step size times which vertex we are on, thereby moving through each vertex position. 
                    // Then we add a random offset to x and y to prevent tiling effects.
                    // Finally multiply by the frequency so sample points are appropriately spread out.
                    float sampleX = (initialCoord + x * stepSize + worldSpacePosition.x + randomX) * settings.Scale * frequency;
                    float sampleY = (initialCoord + y * stepSize + worldSpacePosition.y + randomY) * settings.Scale * frequency;


                    noiseMap[y * vertexNum + x] += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;
                }
            }

            maxPossibleHeight += amplitude;
            amplitude *= settings.Persistence;
            frequency *= settings.Lacunarity;
        }

        // float summedHeight = noiseMap[0] +
        //                 noiseMap[vertexNum - 1] +
        //                 noiseMap[(vertexNum - 1) * vertexNum] +
        //                 noiseMap[(vertexNum - 1) * vertexNum + vertexNum - 1];

        // averageHeight = summedHeight * ChunkGlobals.heightMultiplier / (4 * maxPossibleHeight);

        return maxPossibleHeight;
    }

    private static void NormalizeNoiseMap(int vertexNum, ref float[] noiseMap, float maxPossibleHeight)
    {
        for (int y = 0; y < vertexNum; y++)
        {
            for (int x = 0; x < vertexNum; x++)
            {
                noiseMap[y * vertexNum + x] = Mathf.Clamp(noiseMap[y * vertexNum + x] / maxPossibleHeight, 0f, 1f);
            }
        }
    }

    // public static float GetAverageHeight(Vector2 worldSpacePosition, NoiseSettings settings)
    // {
    //     System.Random random = new(settings.Seed);
    //     float amplitude = 1;
    //     float frequency = 1;
    //     float averageHeight = 0;
    //     float maxPossibleHeight = 0;

    //     for (int octave = 0; octave < settings.Octaves; octave++)
    //     {
    //         int randomX = random.Next(-1000, 1000);
    //         int randomY = random.Next(-1000, 1000);

    //         // Takes the starting point (bottom left corner of the mesh in world space) and adds the step size times which vertex we are on, thereby moving through each vertex position. 
    //         // Then we add a random offset to x and y to prevent tiling effects.
    //         // Finally multiply by the frequency so sample points are appropriately spread out.
    //         float sampleX = (worldSpacePosition.x + randomX) * settings.Scale * frequency;
    //         float sampleY = (worldSpacePosition.y + randomY) * settings.Scale * frequency;

    //         averageHeight += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;

    //         maxPossibleHeight += amplitude;
    //         amplitude *= settings.Persistence;
    //         frequency *= settings.Lacunarity;
    //     }

    //     return averageHeight / maxPossibleHeight;
    // }
}

[System.Serializable]
public struct NoiseSettings
{
    [HideInInspector] public int ChunkSize;
    [Space]

    // Ratio between world space and mesh space
    [Range(0.01f, 20f)] public float Scale;
    [Range(1f, 5f)] public float Lacunarity;
    [Range(0.01f, 1f)] public float Persistence;
    [Range(1, 10)] public int Octaves;
    public int Seed;

    // Initialize fields with default values
    public NoiseSettings(int dummy)
    {
        ChunkSize = ChunkGlobals.meshSpaceChunkSize;
        Scale = 0.005f;
        Lacunarity = 2f;
        Persistence = 0.4f;
        Octaves = 10;
        Seed = 0;
    }

    // Static factory method for default settings
    public static NoiseSettings CreateDefault()
    {
        return new NoiseSettings(0);
    }
}
