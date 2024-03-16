using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureGen
{
    public static Texture2D lookupTexture;
    static int numberOfColors = 400;

    public static void PreprocessColors(List<TerrainLevel> terrainLevels)
    {
        List<Color> lookupTable = new List<Color>();
        for (int i = 0; i <= numberOfColors; i++)
        {
            float previousMaxHeight = 0f;
            foreach (TerrainLevel level in terrainLevels)
            {
                if (level.MaxHeight >= i / (float)numberOfColors)
                {
                    float lerpFactor = (i / (float)numberOfColors - previousMaxHeight) / (level.MaxHeight - previousMaxHeight);
                    lookupTable.Add(Color.Lerp(level.ColorStart, level.ColorEnd, level.Gradient.Evaluate(lerpFactor)));
                    break;
                }
                previousMaxHeight = level.MaxHeight;
            }
        }

        lookupTexture = new Texture2D(lookupTable.Count, 1, TextureFormat.RGBA32, false, true); // Ensure linear color space for color calculations
        lookupTexture.SetPixels(lookupTable.ToArray());
        lookupTexture.Apply();

        lookupTexture.filterMode = FilterMode.Point;
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

    // public static Color GetColorForHeight(float height)
    // {
    //     // Scale the height (between 0 and 1) to a range of 0 to numberOfColors
    //     int scaledHeight = Mathf.RoundToInt(height * numberOfColors);
    //     if (lookupTable.TryGetValue(scaledHeight, out Color color))
    //     {
    //         return color;
    //     }
    //     return Color.white; // Default color if none found, adjust as needed
    // }

    // private static Color[] MapColorsToHeight(float[] heightArray, int textureSize)
    // {
    //     Color[] colorMap = new Color[textureSize * textureSize];

    //     for (int x = 0; x < textureSize; x++)
    //     {
    //         for (int y = 0; y < textureSize; y++)
    //         {
    //             // Summed height of the four corners of the square
    //             float summedHeight =
    //                 heightArray[x * (textureSize + 1) + y] +
    //                 heightArray[x * (textureSize + 1) + y + 1] +
    //                 heightArray[(x + 1) * (textureSize + 1) + y] +
    //                 heightArray[(x + 1) * (textureSize + 1) + y + 1];

    //             // Average height of the four corners of the square
    //             float smoothedHeight = summedHeight / 4f;



    //             colorMap[x * textureSize + y] = GetColorForHeight(smoothedHeight);
    //         }
    //     }

    //     return colorMap;
    // }


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