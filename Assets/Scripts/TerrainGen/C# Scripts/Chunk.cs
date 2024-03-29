using UnityEngine;
using System.Collections.Generic;

// A chunk is a single unit of terrain in the world.
[RequireComponent(typeof(LODGroup))]
public class Chunk : MonoBehaviour
{
    public Vector2 WorldSpacePosition { get; private set; }

    private GameObject parent;

    private LOD[] lodList = new LOD[ChunkGlobals.lodCount];

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

        // Setup component
        LODGroup lodGroup = GetComponent<LODGroup>();

        SetLODList(lodList);

        lodGroup.SetLODs(lodList);
        lodGroup.RecalculateBounds();

    }

    void SetLODList(LOD[] lodList)
    {
        int lodCount = ChunkGlobals.lodCount;
        Mesh[] meshArray = new Mesh[lodCount];
        Texture[] textureArray = new Texture[lodCount];

        GenerateTerrainContent(ref meshArray, ref textureArray, lodCount);

        for (int i = 0; i < lodCount; i++)
        {
            GameObject go = InitializeLODGameObject(i, meshArray[i], textureArray[i]);
            Renderer renderer = go.GetComponent<MeshRenderer>();

            // Simplify LOD level calculation and setting
            float screenRelativeTransitionHeight = (i == lodCount - 1) ? 0f : 1f / Mathf.Pow(2f, i + 1);
            lodList[i] = new LOD(screenRelativeTransitionHeight, new Renderer[] { renderer });
        }
    }

    GameObject InitializeLODGameObject(int lod, Mesh mesh, Texture texture)
    {
        GameObject go = new GameObject($"LOD_{lod}");
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;

        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();

        SetMesh(meshFilter, mesh);
        SetTexture(meshRenderer, texture);

        return go;
    }

    private void GenerateTerrainContent(ref Mesh[] meshes, ref Texture[] textures, int lodCount)
    {
        GetDataFromCompute getDataFromCompute = transform.parent.GetComponent<GetDataFromCompute>();
        getDataFromCompute.CalculateMeshData(WorldSpacePosition);

        Vector3[] vertexArray = getDataFromCompute.GetHeightData();
        Texture textureData = getDataFromCompute.GetTextureData();
        Mesh[] lodMeshArray = MeshGen.GenerateMeshes(vertexArray);

        // float[][] heightValueArrays = MeshPrune.GetHeightValueArrays(heightArray, lodCount);

        for (int i = 0; i < lodCount; i++)
        {
            // Use the original heightArray for the highest LOD, then use the pruned arrays for lower LODs
            // float[] currentHeightArray = (i == 0) ? heightArray : heightValueArrays[i - 1];
            // Vector3[] currentHeightArray = heightArray;

            meshes[i] = lodMeshArray[i];
            textures[i] = textureData;
        }
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

    // Sets the mesh for the chunk.
    private void SetMesh(MeshFilter meshFilter, Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }

    // Sets the texture for the chunk.
    private void SetTexture(MeshRenderer meshRenderer, Texture texture)
    {
        // meshRenderer.material = new Material(Shader.Find("Unlit/Texture"))
        meshRenderer.material = new Material(Shader.Find("Standard"))
        {
            mainTexture = texture
        };
        meshRenderer.material.SetFloat("_Glossiness", 0.0f);
    }
}
