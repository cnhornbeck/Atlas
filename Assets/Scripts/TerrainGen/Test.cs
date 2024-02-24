using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;

public class Test : MonoBehaviour
{

    #region Noise Variables

    [SerializeField] NoiseSettings noiseSettings;
    [SerializeField] List<TerrainLevel> colorList = new();
    #endregion

    MeshRenderer planeTex;
    float[] heightArray;
    [SerializeField] int levelOfDetail;

    void Start()
    {
        noiseSettings = NoiseSettings.CreateDefault();
        planeTex = GetComponent<MeshRenderer>();
        StartCoroutine(SetHeightArray());
    }

    // void Update()
    // {

    // }

    void UpdateTexture()
    {
        Texture2D terrainTex = TextureGenerator.GenerateTexture(heightArray, colorList);

        planeTex.material.mainTexture = terrainTex;
        planeTex.material.SetFloat("_Glossiness", 0.0f);

        terrainTex.filterMode = FilterMode.Point;
        terrainTex.wrapMode = TextureWrapMode.Clamp;
        terrainTex.Apply();
    }

    IEnumerator SetHeightArray()
    {
        while (true)
        {
            heightArray = NoiseGenerator.GeneratePerlinNoise(noiseSettings, ChunkGlobals.lodNumArray[levelOfDetail]);
            UpdateTexture(); // Update the texture after heightArray is generated
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator CaptureTextureProcess()
    {
        // Capture the texture
        Texture2D terrainTex = TextureGenerator.GenerateTexture(heightArray, colorList);

        // Apply texture settings
        terrainTex.filterMode = FilterMode.Point;
        terrainTex.wrapMode = TextureWrapMode.Clamp;
        terrainTex.Apply();

        // Wait for a frame to ensure all operations are completed
        yield return null;

        // Convert the texture to a PNG
        byte[] bytes = terrainTex.EncodeToPNG();

        // Choose a directory and file name
        string directoryPath = Application.persistentDataPath;
        string fileName = "exportedTexture.png";
        string fullPath = Path.Combine(directoryPath, fileName);

        // Write to a file
        File.WriteAllBytes(fullPath, bytes);

        Debug.Log("Texture saved to " + fullPath);
        yield return null;

        // If you need to perform actions after saving is done, place them here
    }

}