using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public int id = -1;
    public int lod = ChunkGlobals.lodNumSize - 1;
    public float averageHeight;
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


    public void Initialize(GameObject parent, Vector2 worldSpacePosition, int lod)
    {
        // Set Pos
        WorldSpacePosition = worldSpacePosition;
        this.parent = parent;
        // Bounds = new Bounds(new Vector3(worldSpacePosition.x, 0, worldSpacePosition.y), Vector3.one * ChunkGlobals.meshSpaceChunkSize);
        this.lod = lod;

        SetVisible(true);

        // print("Initializing chunk at: " + worldSpacePosition / ChunkGlobals.worldSpaceChunkSize + " with LOD: " + lod);

        // Generate height values for chunk for this level of detail
        float[] heightArray = NoiseGen.GeneratePerlinNoise(worldSpacePosition, ChunkGlobals.lodNumArray[lod], ref averageHeight);

        // Set meshes and textures for this level of detail
        LodMeshList[lod] = MeshGen.GenerateMesh(heightArray);
        CalcUVs(LodMeshList[lod], heightArray);

        LodTextureList[lod] = TextureGen.GenerateTexture(heightArray);

        Vector3 worldPosition = new(worldSpacePosition.x, 0, worldSpacePosition.y);

        // Now, adjust the parent's position.
        // We are setting the parent's position directly to the world position we calculated.
        parent.transform.position = worldPosition;

        meshFilter = GetComponent<MeshFilter>();
        currentMesh = LodMeshList[lod];
        SetMesh(meshFilter, currentMesh);

        meshRenderer = GetComponent<MeshRenderer>();

        currentTexture = LodTextureList[lod];

        SetTexture(meshRenderer, currentTexture);
    }

    public void SetLOD(int lod)
    {
        float dummy = 0;
        // Check is the mesh is already set to the correct level of detail
        if (currentMesh != LodMeshList[lod])
        {
            // Check if the mesh has not been generated yet (this also means the texture has not been generated)
            if (LodMeshList[lod] == null)
            {
                // Generate height values for chunk for this level of detail
                float[] heightArray = NoiseGen.GeneratePerlinNoise(WorldSpacePosition, ChunkGlobals.lodNumArray[lod], ref dummy);

                // Generate the mesh for this level of detail and set the UVs
                LodMeshList[lod] = MeshGen.GenerateMesh(heightArray);
                CalcUVs(LodMeshList[lod], heightArray);

                // Generate the texture for this level of detail
                LodTextureList[lod] = TextureGen.GenerateTexture(heightArray);
            }

            // Set the mesh this level of detail
            currentMesh = LodMeshList[lod];
            SetMesh(meshFilter, currentMesh);

            // Set the texture this level of detail
            currentTexture = LodTextureList[lod];
            SetTexture(meshRenderer, currentTexture);
        }

        this.lod = lod;
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
                // Size - 1 is used because the UV coordinates are in the range [0, 1] and size increments from 0 to size - 1 in the for loop.
                uvs[index] = new Vector2(x / ((float)size - 1), y / ((float)size - 1));
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
