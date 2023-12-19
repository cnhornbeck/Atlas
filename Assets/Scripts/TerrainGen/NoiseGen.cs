using UnityEngine;

public class NoiseGen
{
    // Generates a 2D array of Perlin noise values with the given parameters
    public static float[] GeneratePerlinNoise(NoiseSettings noiseSettings, int levelOfDetail)
    {
        // Set level of detail corrected chunkSize
        int chunkSize = (ChunkGlobals.chunkSize + 1) / levelOfDetail;

        // Create a new 2D array to hold the output noise values
        float[] output = new float[chunkSize * chunkSize];

        System.Random random = new(noiseSettings.seed);

        // Initialize variables for tracking the amplitude and current amplitude and frequency for each octave
        float maxAmplitude = 0;
        float amplitude = 1;
        float frequency = 1;

        // Generate Perlin noise for each octave
        for (int i = 0; i < noiseSettings.octaves; i++)
        {
            int randomIntX = random.Next(-1000, 1000);
            int randomIntY = random.Next(-1000, 1000);

            // Loop over each pixel in the output array
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float xCoord = ((float)x / chunkSize * noiseSettings.scale + noiseSettings.xOffset + randomIntX * amplitude) * frequency;
                    float yCoord = ((float)y / chunkSize * noiseSettings.scale + noiseSettings.yOffset + randomIntY * amplitude) * frequency;

                    // Calculate the Perlin noise value for the current pixel at the current octave
                    output[y * chunkSize + x] += Mathf.PerlinNoise(xCoord, yCoord) * amplitude;
                }
            }


            // Update the maximum amplitude and current amplitude and frequency for the next octave
            maxAmplitude += amplitude;
            amplitude *= noiseSettings.persistence;
            frequency *= noiseSettings.lacunarity;
        }


        // Normalize the output values so they range from 0 to 1
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                output[y * chunkSize + x] = Mathf.Clamp(output[y * chunkSize + x] / maxAmplitude, 0f, 1f);
                // output[y * chunkSize + x] = (output[y * chunkSize + x] - minAmplitude) / (maxAmplitude - minAmplitude);
            }
        }

        // Return the completed Perlin noise array
        return output;
    }
}


[System.Serializable]
public struct NoiseSettings
{
    [HideInInspector] public int chunkSize;
    public float xOffset;
    public float yOffset;
    [Space]
    [Range(0.01f, 20f)] public float scale;
    [Range(1f, 5f)] public float lacunarity;
    [Range(0.01f, 1f)] public float persistence;
    [Range(1f, 10f)] public int octaves;
    public int seed;

    // Initialize fields with default values
    public NoiseSettings(int dummy)
    {
        chunkSize = ChunkGlobals.chunkSize;
        xOffset = 0f;
        yOffset = 0f;
        scale = 0.3f;
        lacunarity = 2f;
        persistence = 0.3f;
        octaves = 7;
        seed = 0;
    }

    // Static factory method
    public static NoiseSettings CreateDefault()
    {
        return new NoiseSettings(0);
    }
}