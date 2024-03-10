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
    // [SerializeField] private int lod;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private LOD[] lodList = new LOD[ChunkGlobals.lodNumArray.Length];

    // private Mesh[] LodMeshList = new Mesh[ChunkGlobals.lodNumArray.Length];
    // private Texture2D[] LodTextureList = new Texture2D[ChunkGlobals.lodNumArray.Length];

    // Initialize the chunk with its basic properties and generate its initial content.
    public void Initialize(GameObject parent, Vector2 worldSpacePosition)
    {
        WorldSpacePosition = worldSpacePosition;
        this.parent = parent;
        // this.lod = lod;

        // Set the parent's position based on the world space position.
        Vector3 worldPosition = new(worldSpacePosition.x, 0, worldSpacePosition.y);
        parent.transform.position = worldPosition;

        SetVisible(true);

        // Setup components
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        LODGroup lodGroup = GetComponent<LODGroup>();

        SetLODList(lodList);

        lodGroup.SetLODs(lodList);
        lodGroup.RecalculateBounds();

        // GenerateTerrainContent();
    }

    // void SetLODList(LOD[] lodList)
    // {
    //     LOD[] lods = new LOD[4];
    //     for (int i = 0; i < 4; i++)
    //     {
    //         PrimitiveType primType = PrimitiveType.Cube;
    //         switch (i)
    //         {
    //             case 1:
    //                 primType = PrimitiveType.Capsule;
    //                 break;
    //             case 2:
    //                 primType = PrimitiveType.Sphere;
    //                 break;
    //             case 3:
    //                 primType = PrimitiveType.Cylinder;
    //                 break;
    //         }
    //         GameObject go = GameObject.CreatePrimitive(primType);
    //         go.transform.parent = gameObject.transform;
    //         go.transform.position = go.transform.parent.position;
    //         Renderer[] renderers = new Renderer[1];
    //         renderers[0] = go.GetComponent<Renderer>();
    //         lods[i] = new LOD((3 - i) / 4f, renderers);
    //         lodList[i] = lods[i];
    //     }
    // }

    void SetLODList(LOD[] lodList)
    {
        int lodCount = ChunkGlobals.lodNumArray.Length;

        for (int i = 0; i < lodCount; i++)
        {
            // Create a new GameObject for this LOD level
            GameObject go = new("LOD_" + i);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;

            // Add required components
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();

            // Generate terrain content (mesh and texture) for this LOD level
            Renderer terrainRenderer = GenerateTerrainContent(i, meshFilter, meshRenderer); // Adjusted to pass MeshFilter and MeshRenderer

            Renderer[] renderers = new Renderer[1];
            renderers[0] = terrainRenderer;

            // Set the LOD
            if (i == lodCount - 1)
            {
                lodList[i] = new LOD(0, renderers);
            }
            else
            {
                lodList[i] = new LOD(1f / Mathf.Pow(2f, i + 1), renderers);
            }
        }
    }

    private Renderer GenerateTerrainContent(int levelOfDetail, MeshFilter meshFilter, MeshRenderer meshRenderer)
    {
        // Generate mesh and texture based on levelOfDetail
        float[] heightArray = NoiseGen.GeneratePerlinNoise(WorldSpacePosition, levelOfDetail);
        Mesh mesh = MeshGen.GenerateMesh(heightArray);
        Texture2D texture = TextureGen.GenerateTexture(heightArray);

        // Assign the generated mesh and texture
        SetMesh(meshFilter, mesh);
        SetTexture(meshRenderer, texture);

        // Calculate and set UVs - assuming CalcUVs modifies the mesh directly
        CalcUVs(mesh, heightArray);

        return meshRenderer;
    }

    // Sets the Level of Detail (LOD) for the chunk, regenerating content if necessary.
    // public void GetLODContent(int levelOfDetail)
    // {
    //     // No change needed if the LOD is already set to the desired level.
    //     if (lod != newLOD)
    //     {
    //         lod = newLOD;
    //         if (LodMeshList[lod] == null)
    //         {
    //             GenerateContentForLOD(lod);
    //         }
    //         else
    //         {
    //             // Apply existing LOD content without regenerating.
    //             ApplyLODContent(lod);
    //         }
    //     }
    // }

    // // Generates or applies content for a specific LOD.
    // private Renderer GenerateTerrainContent(int levelOfDetail)
    // {
    //     float[] heightArray = NoiseGen.GeneratePerlinNoise(WorldSpacePosition, levelOfDetail - 1);

    //     Mesh mesh = MeshGen.GenerateMesh(heightArray);
    //     Texture2D texture = TextureGen.GenerateTexture(heightArray);

    //     SetMesh(meshFilter, mesh);
    //     SetTexture(meshRenderer, texture);
    //     CalcUVs(mesh, heightArray);

    //     return meshRenderer;
    // }

    // // Applies mesh and texture for the current LOD.
    // private void ApplyLODContent(int lod)
    // {

    // }

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
