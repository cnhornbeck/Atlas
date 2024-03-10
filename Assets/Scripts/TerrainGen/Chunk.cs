using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(LODGroup))]

public class Chunk : MonoBehaviour
{
    public Vector2 WorldSpacePosition { get; private set; }
    public float AverageHeight { get; private set; }

    private GameObject parent;
    [SerializeField] private int lod;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private Mesh[] LodMeshList = new Mesh[ChunkGlobals.lodNumArray.Length];
    private Texture2D[] LodTextureList = new Texture2D[ChunkGlobals.lodNumArray.Length];

    // Initialize the chunk with its basic properties and generate its initial content.
    public void Initialize(GameObject parent, Vector2 worldSpacePosition, int lod)
    {
        WorldSpacePosition = worldSpacePosition;
        this.parent = parent;
        this.lod = lod;

        // Set the parent's position based on the world space position.
        Vector3 worldPosition = new(worldSpacePosition.x, 0, worldSpacePosition.y);
        parent.transform.position = worldPosition;

        SetVisible(true);

        // Setup components
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        GenerateContentForLOD(lod);
    }

    // Sets the Level of Detail (LOD) for the chunk, regenerating content if necessary.
    public void SetLOD(int newLOD)
    {
        // No change needed if the LOD is already set to the desired level.
        if (lod != newLOD)
        {
            lod = newLOD;
            if (LodMeshList[lod] == null)
            {
                GenerateContentForLOD(lod);
            }
            else
            {
                // Apply existing LOD content without regenerating.
                ApplyLODContent(lod);
            }
        }
    }

    // Generates or applies content for a specific LOD.
    private void GenerateContentForLOD(int lod)
    {
        float[] heightArray = NoiseGen.GeneratePerlinNoise(WorldSpacePosition, ChunkGlobals.lodNumArray[lod]);
        if (LodMeshList[lod] == null)
        {
            LodMeshList[lod] = MeshGen.GenerateMesh(heightArray);
            LodTextureList[lod] = TextureGen.GenerateTexture(heightArray);
        }

        CalcUVs(LodMeshList[lod], heightArray);
        ApplyLODContent(lod);
    }

    // Applies mesh and texture for the current LOD.
    private void ApplyLODContent(int lod)
    {
        SetMesh(meshFilter, LodMeshList[lod]);
        SetTexture(meshRenderer, LodTextureList[lod]);
    }

    // Sets the visibility of the chunk.
    public void SetVisible(bool visible)
    {
        parent.SetActive(visible);
    }

    // Returns visibility status of the chunk.
    public bool IsVisible()
    {
        return parent.activeSelf;
    }

    // Calculates and sets UVs based on height array.
    private void CalcUVs(Mesh mesh, float[] heightArray)
    {
        int size = (int)Mathf.Sqrt(heightArray.Length);
        Vector2[] uvs = new Vector2[size * size];
        for (int i = 0, y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++, i++)
            {
                uvs[i] = new Vector2(x / (float)(size - 1), y / (float)(size - 1));
            }
        }
        mesh.SetUVs(0, new List<Vector2>(uvs));
    }

    // Sets the mesh for the chunk.
    private void SetMesh(MeshFilter meshFilter, Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }

    // Sets the texture for the chunk.
    private void SetTexture(MeshRenderer meshRenderer, Texture2D texture)
    {
        if (meshRenderer.material == null)
        {
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
        meshRenderer.material.mainTexture = texture;
        meshRenderer.material.SetFloat("_Glossiness", 0.0f);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
    }
}
