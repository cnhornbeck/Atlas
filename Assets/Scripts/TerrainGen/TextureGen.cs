using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureGen
{
    static Dictionary<int, Color> lookupTable = new();

    public static Texture2D GenerateTexture(float[] heightArray, List<TerrainLevel> terrainLevels)
    {
        int textureSize = (int)Mathf.Sqrt(heightArray.Length);

        Texture2D texture = new Texture2D(textureSize, textureSize)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents texture from tiling
        };

        texture.SetPixels(MapColorsToHeight(heightArray));
        texture.Apply(); // Apply changes to the texture

        return texture;
    }

    public static void PreprocessColors(List<TerrainLevel> terrainLevels)
    {
        int numberOfColors = 1000;
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
                    // Debug.Log((i / 100f - previousMaxHeight) + " divided by " + (level.MaxHeight - previousMaxHeight) + " equals " + (i / 100f - previousMaxHeight) / (level.MaxHeight - previousMaxHeight));
                    // Debug.Log();
                    break;
                }
                previousMaxHeight = level.MaxHeight;
            }
        }
    }


    public static Color GetColorForHeight(float height)
    {
        int truncatedHeight = Mathf.RoundToInt(height * 1000f);
        // Debug.Log(truncatedHeight);
        if (lookupTable.TryGetValue(truncatedHeight, out Color color))
        {
            return color;
        }
        return Color.white; // Default color if none found, adjust as needed
    }

    private static Color[] MapColorsToHeight(float[] heightArray)
    {
        Color[] colorMap = new Color[heightArray.Length];

        for (int i = 0; i < heightArray.Length; i++)
        {
            colorMap[i] = GetColorForHeight(heightArray[i]);
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