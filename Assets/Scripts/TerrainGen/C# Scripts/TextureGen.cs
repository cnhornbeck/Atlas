using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using System.Threading.Tasks;

public class TextureGen
{
    static readonly int numberOfColors = 60;
    static NativeArray<Color> lookupTable = new();

    public static void PreprocessColors(List<TerrainLevel> terrainLevels)
    {
        List<Color> lookupTableList = new();
        for (int i = 0; i <= numberOfColors; i++)
        {
            float previousMaxHeight = 0f;
            foreach (TerrainLevel level in terrainLevels)
            {
                if (level.MaxHeight >= i / (float)numberOfColors)
                {
                    float lerpFactor = (i / (float)numberOfColors - previousMaxHeight) / (level.MaxHeight - previousMaxHeight);
                    lookupTableList.Add(Color.Lerp(level.ColorStart, level.ColorEnd, level.Gradient.Evaluate(lerpFactor)));
                    break;
                }
                previousMaxHeight = level.MaxHeight;
            }
        }
        lookupTable = new NativeArray<Color>(lookupTableList.ToArray(), Allocator.Persistent);
    }

    [BurstCompile]
    public struct TextureGenJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> vertexArray;
        [ReadOnly] public NativeArray<Color> lookupTable;
        public NativeArray<Color> colorData;
        public float numberOfColors;
        public void Execute(int index)
        {
            int textureSize = ChunkGlobals.meshSpaceChunkSize;
            int meshSize = textureSize + 1;

            int x = index % textureSize;
            int y = index / textureSize;

            // Calculate the indices of the vertices
            int bottomLeft = y * meshSize + x;
            int bottomRight = bottomLeft + 1;
            int topLeft = bottomLeft + meshSize;
            int topRight = topLeft + 1;

            float summedHeight =
                vertexArray[bottomLeft].y +
                vertexArray[bottomRight].y +
                vertexArray[topLeft].y +
                vertexArray[topRight].y;

            float smoothedHeight = ((summedHeight / (4 * ChunkGlobals.heightMultiplier)) + 1) / 2;
            int colorIndex = (int)(smoothedHeight * numberOfColors);
            colorData[index] = lookupTable[colorIndex];
        }
    }

    public static async Task<Texture2D> ScheduleTextureGenJob(NativeArray<Vector3> vertexArray)
    {
        int textureSize = ChunkGlobals.meshSpaceChunkSize;

        NativeArray<Color> colorData = new(textureSize * textureSize, Allocator.TempJob);

        TextureGenJob job = new()
        {
            vertexArray = vertexArray,
            lookupTable = lookupTable,
            colorData = colorData,
            numberOfColors = numberOfColors
        };

        JobHandle jobHandle = job.Schedule(colorData.Length, 64);

        while (!jobHandle.IsCompleted)
        {
            await Task.Yield(); // Yield the task back to the Unity main loop until the job is complete
        }

        jobHandle.Complete();

        Texture2D textureData = new(textureSize, textureSize)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents texture from tiling
        };

        textureData.SetPixels(colorData.ToArray());
        textureData.Apply();

        colorData.Dispose();

        return textureData;
    }

    public static Texture2D GenerateTexture(float[] heightArray)
    {
        int textureSize = (int)Mathf.Sqrt(heightArray.Length) - 1;

        Texture2D texture = new(textureSize, textureSize)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents texture from tiling
        };

        // texture.SetPixels(MapColorsToHeight(heightArray, textureSize));
        texture.Apply(); // Apply changes to the texture

        return texture;
    }

    public static Texture2D GenerateTexture(Color[] colorData)
    {
        int textureSize = (int)Mathf.Sqrt(colorData.Length);

        Texture2D texture = new(textureSize, textureSize)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents texture from tiling
        };

        texture.SetPixels(colorData);
        texture.Apply(); // Apply changes to the texture

        return texture;
    }

    public static void SaveTextureToPNG(Texture2D texture, string filePath)
    {
        // Encode the texture into PNG format
        byte[] bytes = texture.EncodeToPNG();
        if (bytes != null && bytes.Length > 0)
        {
            // Write the bytes to a file
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Texture saved to {filePath}");
        }
        else
        {
            Debug.LogError("Failed to encode texture to PNG.");
        }
    }

    [Serializable]
    public struct TerrainLevel
    {
        public string Name;
        public Color ColorStart;
        public Color ColorEnd;
        public AnimationCurve Gradient;
        [Range(0f, 1f)] public float MaxHeight;
    }
}