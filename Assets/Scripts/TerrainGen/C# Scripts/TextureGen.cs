using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureGen
{
    static Dictionary<int, Color> lookupTable = new();
    static int numberOfColors = 100;

    public static void PreprocessColors(List<TerrainLevel> terrainLevels)
    {
        // Assuming heights are between 0 and 1, and we're truncating to 1 decimal place
        for (int i = 0; i <= numberOfColors; i += 1)
        {
            float previousMaxHeight = 0f;
            // Find the first color that matches or exceeds each truncated value
            foreach (TerrainLevel level in terrainLevels)
            {
                if (level.MaxHeight >= i / (float)numberOfColors)
                {
                    lookupTable[i] = Color.Lerp(level.ColorStart, level.ColorEnd, level.Gradient.Evaluate((i / (float)numberOfColors - previousMaxHeight) / (level.MaxHeight - previousMaxHeight)));
                    break;
                }
                previousMaxHeight = level.MaxHeight;
            }
        }
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

        texture.SetPixels(MapColorsToHeight(heightArray, textureSize));
        texture.Apply(); // Apply changes to the texture

        // If you want to save the texture to a file, use the following code:
        // string path = Application.persistentDataPath + "/mySavedTexture.png";
        // SaveTextureToPNG(texture, path);

        return texture;
    }

    public static Color GetColorForHeight(float height)
    {
        // Scale the height (between 0 and 1) to a range of 0 to numberOfColors
        int scaledHeight = Mathf.RoundToInt(height * numberOfColors);
        if (lookupTable.TryGetValue(scaledHeight, out Color color))
        {
            return color;
        }
        return Color.white; // Default color if none found, adjust as needed
    }

    private static Color[] MapColorsToHeight(float[] heightArray, int textureSize)
    {
        Color[] colorMap = new Color[textureSize * textureSize];

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // Summed height of the four corners of the square
                float summedHeight =
                    heightArray[x * (textureSize + 1) + y] +
                    heightArray[x * (textureSize + 1) + y + 1] +
                    heightArray[(x + 1) * (textureSize + 1) + y] +
                    heightArray[(x + 1) * (textureSize + 1) + y + 1];

                // Average height of the four corners of the square
                float smoothedHeight = summedHeight / 4f;

                colorMap[x * textureSize + y] = GetColorForHeight(smoothedHeight);
            }
        }

        return colorMap;
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