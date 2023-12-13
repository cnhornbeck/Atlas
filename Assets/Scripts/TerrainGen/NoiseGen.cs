using UnityEngine;

public class NoiseGen
{
    // Generates a 2D array of Perlin noise values with the given parameters
    public static float[] GeneratePerlinNoise(int chunkSize, float xOffset, float yOffset, float scale, float lacunarity, float persistence, int octaves, int seed)
    {
        // Create a new 2D array to hold the output noise values
        float[] output = new float[chunkSize * chunkSize];

        System.Random random = new(seed);

        // Initialize variables for tracking the maximum amplitude and current amplitude and frequency for each octave
        float maxAmplitude = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        // Generate Perlin noise for each octave
        for (int i = 0; i < octaves; i++)
        {
            int randomIntX = random.Next(-1000, 1000);
            int randomIntY = random.Next(-1000, 1000);

            // Loop over each pixel in the output array
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float xCoord = ((float)x / chunkSize * scale + xOffset + randomIntX * amplitude * amplitude) * frequency;
                    float yCoord = ((float)y / chunkSize * scale + yOffset + randomIntY * amplitude * amplitude) * frequency;

                    // float xCoord = ((float)x / width * scale + xOffset) * frequency;
                    // float yCoord = ((float)y / height * scale + yOffset) * frequency;

                    // Calculate the Perlin noise value for the current pixel at the current octave
                    output[y * chunkSize + x] += Mathf.PerlinNoise(xCoord, yCoord) * amplitude;
                }
            }


            // Update the maximum amplitude and current amplitude and frequency for the next octave
            maxAmplitude += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        // Normalize the output values so they range from 0 to 1
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                output[y * chunkSize + x] = Mathf.Clamp(output[y * chunkSize + x] / maxAmplitude, 0f, 1f);
            }
        }

        // Return the completed Perlin noise array
        return output;
    }
}

[System.Serializable]
public struct NoiseSettings
{
    public int chunkSize;
    public float xOffset;
    public float yOffset;
    public float scale;
    public float lacunarity;
    public float persistence;
    public int octaves;
    public int seed;
}