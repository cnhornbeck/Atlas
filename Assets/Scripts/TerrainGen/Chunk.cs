using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public int id = -1;
    public Vector2 Position { get; set; } = Vector2.zero;
    public Bounds Bounds { get; set; } = new();
    public Mesh[] LodMeshList { get; set; } = new Mesh[ChunkGlobals.lodNumSize];
    public Texture2D[] LodTextureList { get; set; } = new Texture2D[ChunkGlobals.lodNumSize];
    public MeshCollider Hitbox { get; set; } = new();
    public enum ChunkState
    {
        Visible,
        NotVisible
    }

    public ChunkState chunkState { get; set; } = ChunkState.Visible;
    MeshFilter meshFilter;
    Mesh currentMesh;
    MeshRenderer meshRenderer;
    Texture2D currentTexture;
    float centerVertHeight = 0;
    GameObject parent;
    float distanceFromNearestPoint;
    bool visible;

    public void UpdateChunk()
    {
        distanceFromNearestPoint = Mathf.Sqrt(Bounds.SqrDistance(Camera.main.transform.position));
        visible = distanceFromNearestPoint <= ChunkGlobals.renderDistance * ChunkGlobals.worldSpaceChunkSize;
        SetVisible(visible);
    }

    public void Initialize(GameObject parent, NoiseSettings noiseSettings, Vector2 position, float heightMultiplier, List<TerrainLevel> colorList)
    {
        // Set Pos
        Position = position * ChunkGlobals.worldSpaceChunkSize;
        this.parent = parent;
        Bounds = new Bounds(new Vector3(position.x, 0, position.y), Vector3.one * ChunkGlobals.ChunkSize);

        visible = false;
        SetVisible(visible);

        float xTerm = noiseSettings.Scale * (position.x / ChunkGlobals.worldSpaceChunkSize);
        float yTerm = noiseSettings.Scale * (position.y / ChunkGlobals.worldSpaceChunkSize);
        noiseSettings.XOffset = xTerm * (1f - (1f / (ChunkGlobals.ChunkSize + 1)));
        noiseSettings.YOffset = yTerm * (1f - (1f / (ChunkGlobals.ChunkSize + 1)));

        // Set Textures and LODs
        for (int i = 0; i < ChunkGlobals.lodNumArray.Length; i++)
        {
            // Generate height values for chunk for this level of detail
            float[] heightArray = NoiseGenerator.GeneratePerlinNoise(noiseSettings, ChunkGlobals.lodNumArray[i]);

            centerVertHeight = heightArray[heightArray.Length / 2] * heightMultiplier;

            // Set meshes and textures for this level of detail
            LodMeshList[i] = MeshGen.GenerateMesh(heightArray, centerVertHeight, heightMultiplier);
            CalcUVs(LodMeshList[i], heightArray);

            LodTextureList[i] = TextureGenerator.GenerateTexture(heightArray, colorList);
        }

        Vector3 worldPosition = new(position.x, 0, position.y);

        // Now, adjust the parent's position.
        // We are setting the parent's position directly to the world position we calculated.
        parent.transform.position = worldPosition;

        meshFilter = GetComponent<MeshFilter>();
        currentMesh = LodMeshList[0];
        SetMesh(meshFilter, currentMesh);

        meshRenderer = GetComponent<MeshRenderer>();
        currentTexture = LodTextureList[0];
        SetTexture(currentTexture);
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

    public void SetTexture(Texture2D texture)
    {
        meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"))
        {
            mainTexture = texture
        };
        meshRenderer.sharedMaterial.SetFloat("_Smoothness", 0.0f);
    }
}
