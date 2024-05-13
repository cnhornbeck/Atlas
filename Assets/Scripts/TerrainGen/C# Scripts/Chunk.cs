using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;


// A chunk is a single unit of terrain in the world.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(LODManager))]
public class Chunk : MonoBehaviour
{
    public Vector2 WorldSpacePosition { get; private set; }
    public Vector3 WorldSpaceChunkCenter { get; private set; }
    private GameObject parent;
    private NativeArray<Vector3> vertexArray;
    Texture textureData;
    Mesh meshData;
    private JobHandle noiseGenJobHandle;
    private bool jobStarted = false;

    // Initialize the chunk with its basic properties and generate its initial content.
    public void Initialize(GameObject parent, Vector2 worldSpacePosition)
    {
        WorldSpacePosition = worldSpacePosition;
        this.parent = parent;

        // Set the parent's position based on the world space position.
        Vector3 worldPosition = new Vector3(worldSpacePosition.x, 0, worldSpacePosition.y);
        parent.transform.position = worldPosition;

        SetVisible(true);

        // Allocate the vertex array for job
        vertexArray = new NativeArray<Vector3>((ChunkGlobals.meshSpaceChunkSize + 1) * (ChunkGlobals.meshSpaceChunkSize + 1), Allocator.Persistent);

        // Schedule the noise generation job
        noiseGenJobHandle = ScheduleNoiseGenJob();
        jobStarted = true;
    }

    void Update()
    {
        if (jobStarted && noiseGenJobHandle.IsCompleted)
        {
            // Complete the job handle
            noiseGenJobHandle.Complete();

            textureData = new Texture2D(1, 1);
            meshData = MeshGen.GenerateMesh(vertexArray.ToArray());

            WorldSpaceChunkCenter = GetWorldSpaceChunkCenter(vertexArray);

            // LODManager lodManager = GetComponent<LODManager>();
            // lodManager.worldSpaceChunkCenter = WorldSpaceChunkCenter;

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            SetMesh(meshFilter, meshData);
            SetTexture(meshRenderer, textureData);

            // Dispose of the heights array
            vertexArray.Dispose();

            jobStarted = false;
        }
    }

    JobHandle ScheduleNoiseGenJob()
    {
        // Setup the job
        NoiseGenJob job = new()
        {
            vertexArray = vertexArray,
            worldSpaceChunkSize = ChunkGlobals.worldSpaceChunkSize,
            meshLengthInVertices = ChunkGlobals.meshSpaceChunkSize + 1,
            heightMultiplier = ChunkGlobals.heightMultiplier,
            worldSpaceChunkCenterX = WorldSpacePosition.x,
            worldSpaceChunkCenterZ = WorldSpacePosition.y
        };

        // Schedule the job
        return job.Schedule(vertexArray.Length, 64);
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
