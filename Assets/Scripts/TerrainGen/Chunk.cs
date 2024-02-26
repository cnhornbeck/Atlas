using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public int id = -1;
    public Vector2 WorldSpacePosition { get; set; } = Vector2.zero;
    public Bounds Bounds { get; set; } = new();
    public Mesh[] LodMeshList { get; set; } = new Mesh[ChunkGlobals.lodNumSize];
    public Texture2D[] LodTextureList { get; set; } = new Texture2D[ChunkGlobals.lodNumSize];
    public MeshCollider Hitbox { get; set; } = new();
    MeshFilter meshFilter;
    Mesh currentMesh;
    MeshRenderer meshRenderer;
    Texture2D currentTexture;
    GameObject parent;



    public void Initialize(GameObject parent, NoiseSettings noiseSettings, Vector2 worldSpacePosition, float heightMultiplier, List<TextureGen.TerrainLevel> colorList)
    {
        // Set Pos
        WorldSpacePosition = worldSpacePosition;
        this.parent = parent;
        Bounds = new Bounds(new Vector3(worldSpacePosition.x, 0, worldSpacePosition.y), Vector3.one * ChunkGlobals.meshSpaceChunkSize);

        SetVisible(true);

        // float xTerm = noiseSettings.Scale * (worldSpacePosition.x / ChunkGlobals.worldSpaceChunkSize);
        // float yTerm = noiseSettings.Scale * (worldSpacePosition.y / ChunkGlobals.worldSpaceChunkSize);
        // noiseSettings.XOffset = xTerm * (1f - (1f / (ChunkGlobals.ChunkSize + 1)));
        // noiseSettings.YOffset = yTerm * (1f - (1f / (ChunkGlobals.ChunkSize + 1)));

        // Set Textures and LODs
        // for (int i = 0; i < ChunkGlobals.lodNumArray.Length; i++)
        // {
        //     // Generate height values for chunk for this level of detail
        //     float[] heightArray = NoiseGenerator.GeneratePerlinNoise(worldSpacePosition, noiseSettings, ChunkGlobals.lodNumArray[i]);

        //     // Set meshes and textures for this level of detail
        //     LodMeshList[i] = MeshGen.GenerateMesh(heightArray, heightMultiplier);
        //     CalcUVs(LodMeshList[i], heightArray);

        //     LodTextureList[i] = TextureGenerator.GenerateTexture(heightArray, colorList);
        // }


        // Generate height values for chunk for this level of detail
        float[] heightArray = NoiseGenerator.GeneratePerlinNoise(worldSpacePosition, noiseSettings, ChunkGlobals.lodNumArray[0]);

        // Set meshes and textures for this level of detail
        LodMeshList[0] = MeshGen.GenerateMesh(heightArray, heightMultiplier);
        CalcUVs(LodMeshList[0], heightArray);

        LodTextureList[0] = TextureGen.GenerateTexture(heightArray, colorList);

        Vector3 worldPosition = new(worldSpacePosition.x, 0, worldSpacePosition.y);

        // Now, adjust the parent's position.
        // We are setting the parent's position directly to the world position we calculated.
        parent.transform.position = worldPosition;

        meshFilter = GetComponent<MeshFilter>();
        currentMesh = LodMeshList[0];
        SetMesh(meshFilter, currentMesh);

        meshRenderer = GetComponent<MeshRenderer>();

        currentTexture = LodTextureList[0];

        SetTexture(meshRenderer, currentTexture);
    }

    public void SetVisible(bool visible)
    {
        parent.SetActive(visible);
    }

    public bool IsVisible()
    {
        return parent.activeSelf;
    }

    public void CalcUVs(Mesh mesh, float[] heightArray)
    {
        int size = (int)Mathf.Sqrt(heightArray.Length);
        // Generate the UV coordinates for the mesh
        Vector2[] uvs = new Vector2[size * size];
        int index = 0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                uvs[index] = new Vector2(x / (float)size, y / (float)size);
                index++;
            }
        }
        // Set the UV coordinates of the mesh
        mesh.SetUVs(0, uvs);
    }

    public void SetMesh(MeshFilter meshFilter, Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }

    public void SetTexture(MeshRenderer meshRenderer, Texture2D texture)
    {
        meshRenderer.material = new Material(Shader.Find("Standard"))
        {
            mainTexture = texture
        };
        meshRenderer.material.SetFloat("_Glossiness", 0.0f);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
    }
}
