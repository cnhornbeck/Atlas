using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public int id = -1;
    public Vector2 position { get; set; } = Vector2.zero;
    public Mesh[] lodMeshList { get; set; } = new Mesh[ChunkGlobals.lodNumSize];
    public Texture2D[] lodTextureList { get; set; } = new Texture2D[ChunkGlobals.lodNumSize];
    public MeshCollider hitbox { get; set; } = new();
    public enum ChunkState
    {
        NotGenerated,
        Generated,
        Generating,
        NeedsUpdating
    }

    public ChunkState chunkState { get; set; } = ChunkState.NotGenerated;

    MeshFilter meshFilter;
    Mesh currentMesh;
    MeshRenderer meshRenderer;
    Texture2D currentTexture;
    float centerVertHeight = 0;

    public void Initialize(GameObject parent, NoiseSettings noiseSettings, Vector2 position, float heightMultiplier, List<TerrainLevel> colorList)
    {
        // Set Pos
        this.position = position;

        // int chunkSize = Mathf.Max((ChunkGlobals.chunkSize + 1) / levelOfDetail, 1);

        float xTerm = noiseSettings.scale * (position.x / ChunkGlobals.worldSpaceChunkSize);
        float yTerm = noiseSettings.scale * (position.y / ChunkGlobals.worldSpaceChunkSize);
        noiseSettings.xOffset = xTerm * (1f - (1f / (ChunkGlobals.chunkSize + 1)));
        noiseSettings.yOffset = yTerm * (1f - (1f / (ChunkGlobals.chunkSize + 1)));

        // Set Textures and LODs
        for (int i = 0; i < ChunkGlobals.lodNumArray.Length; i++)
        {
            // Generate height values for chunk for this level of detail
            float[] heightArray = NoiseGen.GeneratePerlinNoise(noiseSettings, ChunkGlobals.lodNumArray[i]);

            centerVertHeight = heightArray[heightArray.Length / 2] * heightMultiplier;

            // Set meshes and textures for this level of detail
            lodMeshList[i] = MeshGen.GenerateMesh(heightArray, centerVertHeight, heightMultiplier);
            CalcUVs(lodMeshList[i], heightArray);

            lodTextureList[i] = TextureGen.GenerateTexture(heightArray, colorList);
        }

        Vector3 worldPosition = new(position.x, 0, position.y);

        // Now, adjust the parent's position.
        // We are setting the parent's position directly to the world position we calculated.
        parent.transform.position = worldPosition;

        meshFilter = GetComponent<MeshFilter>();
        currentMesh = lodMeshList[0];
        SetMesh(meshFilter, currentMesh);

        meshRenderer = GetComponent<MeshRenderer>();
        currentTexture = lodTextureList[0];
        SetTexture(currentTexture);
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
        meshRenderer.material.mainTexture = texture;
        meshRenderer.material.SetFloat("_Glossiness", 0.0f);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
    }
}
