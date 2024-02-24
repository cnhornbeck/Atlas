using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator
{
    public static Texture2D GenerateTexture(float[] heightArray, List<TerrainLevel> terrainLevels)
    {
        int textureSize = (int)Mathf.Sqrt(heightArray.Length);

        Texture2D texture = new Texture2D(textureSize, textureSize)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents texture from tiling
        };

        texture.SetPixels(MapColorsToHeight(heightArray, terrainLevels));
        texture.Apply(); // Apply changes to the texture

        return texture;
    }

    private static Color[] MapColorsToHeight(float[] heightArray, List<TerrainLevel> terrainLevels)
    {
        Color[] colorMap = new Color[heightArray.Length];

        for (int i = 0; i < heightArray.Length; i++)
        {
            float previousHeight = 0f;

            foreach (TerrainLevel level in terrainLevels)
            {
                if (heightArray[i] <= level.MaxHeight)
                {
                    float heightDifference = level.MaxHeight - previousHeight;
                    float normalizedHeight = (heightArray[i] - previousHeight) / heightDifference;

                    colorMap[i] = Color.Lerp(level.ColorStart, level.ColorEnd, level.Gradient.Evaluate(normalizedHeight));
                    break;
                }

                previousHeight = level.MaxHeight;
            }
        }

        return colorMap;
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
