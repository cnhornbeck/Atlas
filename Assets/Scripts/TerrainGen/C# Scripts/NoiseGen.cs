using UnityEngine;

// public class NoiseGen
// {

//     private static NoiseSettings settings = NoiseSettings.CreateDefault();
//     // private static readonly System.Random random = new(settings.Seed);

//     // Generates a 2D array of Perlin noise values with the given settings
//     public static float[] GeneratePerlinNoise(Vector2 worldSpacePosition)
//     {
//         // Length of the mesh measured in vertices
//         int vertexNum = ChunkGlobals.meshSpaceChunkSize + 1;

//         float[] noiseMap = new float[vertexNum * vertexNum];

//         // Generate and apply Perlin noise
//         ApplyPerlinNoise(worldSpacePosition, vertexNum, ref noiseMap);

//         // Normalize noise map values
//         // NormalizeNoiseMap(vertexNum, ref noiseMap);

//         return noiseMap;
//     }

//     private static void ApplyPerlinNoise(Vector2 worldSpacePosition, int vertexNum, ref float[] noiseMap)
//     {
//         FastNoiseLite noise = new();
//         noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
//         noise.SetFractalType(FastNoiseLite.FractalType.FBm);
//         noise.SetFractalOctaves(settings.Octaves);
//         noise.SetFrequency(settings.Scale);
//         noise.SetFractalLacunarity(settings.Lacunarity);
//         noise.SetFractalGain(settings.Persistence);

//         System.Random random = new(settings.Seed);

//         int randomX = random.Next(-1000, 1000);
//         int randomY = random.Next(-1000, 1000);

//         for (int y = 0; y < vertexNum; y++)
//         {
//             for (int x = 0; x < vertexNum; x++)
//             {
//                 // Represents the bottom left corner of the mesh in world space. Ex: (initialCoord, initialCoord) = (-5, -5)
//                 float initialCoord = -ChunkGlobals.worldSpaceChunkSize / 2f;

//                 // This gives the step size in world space. Ex: 1*10/2 = 5. This means 5 world space units between vertices.
//                 float stepSize = ChunkGlobals.worldSpaceChunkSize / (vertexNum - 1);

//                 // Takes the starting point (bottom left corner of the mesh in world space) and adds the step size times which vertex we are on, thereby moving through each vertex position. 
//                 // Then we add a random offset to x and y to prevent tiling effects.
//                 // Finally multiply by the frequency so sample points are appropriately spread out.
//                 float sampleX = initialCoord + x * stepSize + worldSpacePosition.x + randomX;
//                 float sampleY = initialCoord + y * stepSize + worldSpacePosition.y + randomY;


//                 noiseMap[y * vertexNum + x] = noise.GetNoise(sampleX, sampleY);
//                 noiseMap[y * vertexNum + x] = Mathf.Clamp((noiseMap[y * vertexNum + x] + 0.825f) / 1.65f, 0f, 1f);
//             }
//         }
//     }
// }

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
        Scale = 0.01f;
        Lacunarity = 2f;
        Persistence = 0.5f;
        Octaves = 5;
        Seed = 1337;
    }

    // Static factory method for default settings
    public static NoiseSettings CreateDefault()
    {
        return new NoiseSettings(0);
    }
}
