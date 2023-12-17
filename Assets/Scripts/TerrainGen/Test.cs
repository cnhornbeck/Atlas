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

    void Start()
    {
        noiseSettings = NoiseSettings.CreateDefault();
        planeTex = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        Texture2D terrainTex = TextureGen.GenerateTexture(noiseSettings, colorList);

        if (Input.GetKeyDown(KeyCode.P)) // Example: Press P to start capture
        {
            StartCoroutine(CaptureTextureProcess());
        }

        planeTex.material.mainTexture = terrainTex;
        planeTex.material.SetFloat("_Glossiness", 0.0f);

        terrainTex.filterMode = FilterMode.Point;
        terrainTex.wrapMode = TextureWrapMode.Clamp;
        terrainTex.Apply();

    }

    IEnumerator CaptureTextureProcess()
    {
        // Capture the texture
        Texture2D terrainTex = TextureGen.GenerateTexture(noiseSettings, colorList);

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