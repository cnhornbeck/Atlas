using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureGen
{

    // GenerateTexture(int chunkSize, float xOffset, float yOffset, float scale, float lacunarity, float persistence, int octaves, int seed, List<TerrainLevel> colorList)

    public static Texture2D GenerateTexture(float[] heightArray, List<TerrainLevel> colorList)
    {
        int outputSize = (int)Mathf.Sqrt(heightArray.Length);

        Texture2D output = new(outputSize, outputSize);
        output.SetPixels(ColorMapper(heightArray, colorList));
        return output;
    }


    public static Color[] ColorMapper(float[] heightArray, List<TerrainLevel> colorList)
    {
        Color[] output = new Color[heightArray.Length];

        for (int i = 0; i < heightArray.Length; i++)
        {
            float previousHeight = 0;

            foreach (TerrainLevel lvl in colorList)
            {
                if (heightArray[i] <= lvl.maxHeight)
                {
                    // output[i] = new Color(heightArray[i], heightArray[i], heightArray[i]);
                    output[i] = Color.Lerp(lvl.colorStart, lvl.colorStop, lvl.gradient.Evaluate((heightArray[i] - previousHeight) / (lvl.maxHeight - previousHeight)));
                    break;
                }

                previousHeight = lvl.maxHeight;
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
}

