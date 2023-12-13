using System.Collections.Generic;
using UnityEngine;

public class TextureGen
{

    public static Texture2D GenerateTexture(int chunkSize, float xOffset, float yOffset, float scale, float lacunarity, float persistence, int octaves, int seed, List<TerrainLevel> colorList)
    {
        Texture2D output = new(chunkSize, chunkSize);
        output.SetPixels(ColorMapper(NoiseGen.GeneratePerlinNoise(chunkSize, xOffset, yOffset, scale, lacunarity, persistence, octaves, seed), colorList));
        return output;
    }


    public static Color[] ColorMapper(float[] height, List<TerrainLevel> colorList)
    {
        Color[] output = new Color[height.Length];

        for (int i = 0; i < height.Length; i++)
        {
            foreach (TerrainLevel lvl in colorList)
            {
                if (height[i] < lvl.maxHeight)
                {
                    output[i] = Color.Lerp(lvl.colorStart, lvl.colorStop, lvl.gradient.Evaluate((height[i] - lvl.previousHeight) / (lvl.maxHeight - lvl.previousHeight)));
                    break;
                }
            }
        }

        return output;
    }

}

[System.Serializable]
public struct TerrainLevel
{
    public string name;
    public Color colorStart;
    public Color colorStop;
    public AnimationCurve gradient;
    [Range(0f, 1f)] public float maxHeight;
    [HideInInspector] public float previousHeight;
}

