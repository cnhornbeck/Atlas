using UnityEngine;
using Unity.Collections;

// A chunk is a single unit of terrain in the world.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(LODManager))]
public class Chunk : MonoBehaviour
{
    public Vector2 WorldSpacePosition { get; private set; }
    public Vector3 WorldSpaceChunkCenter { get; private set; }
    private GameObject parent;


    // Initialize the chunk with its basic properties and generate its initial content.
    public async void Initialize(GameObject parent, Vector2 worldSpacePosition)
    {
        WorldSpacePosition = worldSpacePosition;
        this.parent = parent;

        // Set the parent's position based on the world space position.
        Vector3 worldPosition = new(worldSpacePosition.x, 0, worldSpacePosition.y);
        parent.transform.position = worldPosition;

        SetVisible(true);

        // Schedule the noise generation job
        NativeArray<Vector3> vertexArray = await NoiseGen.ScheduleNoiseGenJob(WorldSpacePosition);

        // Schedule the texture generation job
        Texture2D textureData = await TextureGen.ScheduleTextureGenJob(vertexArray);

        Mesh[] meshData = MeshGen.GetMeshes(vertexArray);

        WorldSpaceChunkCenter = GetWorldSpaceChunkCenter(vertexArray);

        LODManager lodManager = GetComponent<LODManager>();
        lodManager.worldSpaceChunkCenter = WorldSpaceChunkCenter;
        lodManager.meshes = meshData;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        SetMesh(meshFilter, meshData[ChunkGlobals.lodCount - 1]);
        SetTexture(meshRenderer, textureData);

        // Dispose of the heights array
        vertexArray.Dispose();
    }

    private Vector3 GetWorldSpaceChunkCenter(NativeArray<Vector3> vertexArray)
    {
        int meshLengthInVertices = ChunkGlobals.meshSpaceChunkSize + 1;

        // Average the height of the four corners of the chunk to get the center height
        float summedHeight = (
            vertexArray[0].y +
            vertexArray[meshLengthInVertices - 1].y +
            vertexArray[(meshLengthInVertices * meshLengthInVertices) - 1].y +
            vertexArray[(meshLengthInVertices * meshLengthInVertices) - meshLengthInVertices].y
            );

        float centerHeight = summedHeight / 4;

        Vector3 worldSpaceChunkCenter = new(WorldSpacePosition.x, centerHeight, WorldSpacePosition.y);
        return worldSpaceChunkCenter;
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
        meshRenderer.material = new Material(Shader.Find("Unlit/Texture"))
        // meshRenderer.material = new Material(Shader.Find("Standard"))
        {
            mainTexture = texture
        };
        meshRenderer.material.SetFloat("_Glossiness", 0.0f);
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
