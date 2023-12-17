using UnityEngine;

public class NoiseGen
{
    // Generates a 2D array of Perlin noise values with the given parameters
    public static float[] GeneratePerlinNoise(NoiseSettings noiseSettings)
    {
        // Create a new 2D array to hold the output noise values
        float[] output = new float[noiseSettings.chunkSize * noiseSettings.chunkSize];

        System.Random random = new(noiseSettings.seed);

        // Initialize variables for tracking the maximum amplitude and current amplitude and frequency for each octave
        float maxAmplitude = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        // Generate Perlin noise for each octave
        for (int i = 0; i < noiseSettings.octaves; i++)
        {
            int randomIntX = random.Next(-1000, 1000);
            int randomIntY = random.Next(-1000, 1000);

            // Loop over each pixel in the output array
            for (int y = 0; y < noiseSettings.chunkSize; y++)
            {
                for (int x = 0; x < noiseSettings.chunkSize; x++)
                {
                    float xCoord = ((float)x / noiseSettings.chunkSize * noiseSettings.scale + noiseSettings.xOffset + randomIntX * amplitude * amplitude) * frequency;
                    float yCoord = ((float)y / noiseSettings.chunkSize * noiseSettings.scale + noiseSettings.yOffset + randomIntY * amplitude * amplitude) * frequency;

                    // float xCoord = ((float)x / width * scale + xOffset) * frequency;
                    // float yCoord = ((float)y / height * scale + yOffset) * frequency;

                    // Calculate the Perlin noise value for the current pixel at the current octave
                    output[y * noiseSettings.chunkSize + x] += Mathf.PerlinNoise(xCoord, yCoord) * amplitude;
                }
            }


            // Update the maximum amplitude and current amplitude and frequency for the next octave
            maxAmplitude += amplitude;
            amplitude *= noiseSettings.persistence;
            frequency *= noiseSettings.lacunarity;
        }

        // Normalize the output values so they range from 0 to 1
        for (int y = 0; y < noiseSettings.chunkSize; y++)
        {
            for (int x = 0; x < noiseSettings.chunkSize; x++)
            {
                output[y * noiseSettings.chunkSize + x] = Mathf.Clamp(output[y * noiseSettings.chunkSize + x] / maxAmplitude, 0f, 1f);
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
        chunkSize = ChunkConstants.chunkSize;
        xOffset = 0f;
        yOffset = 0f;
        scale = 10f;
        lacunarity = 2f;
        persistence = 0.5f;
        octaves = 4;
        seed = 0;
    }

    // Static factory method
    public static NoiseSettings CreateDefault()
    {
        return new NoiseSettings(0);
    }

}