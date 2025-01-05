using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class ChunkConstructor
{
    private JobData<float3> noiseJobData;
    private JobData<Color> textureJobData;
    // private JobData<Mesh> meshJobData;
    private NativeArray<float3> vertices;
    private Texture2D texture;
    private Mesh mesh;
    private float2 worldSpacePosition;

    public void StartNoiseJob(float2 position)
    {
        DisposeNoiseJobData(); // Ensure any previous noise job data is disposed of
        worldSpacePosition = position;
        noiseJobData = NoiseGen.ScheduleNoiseGenJob(worldSpacePosition);
    }

    public void CompleteNoiseJob()
    {
        // Ensure the noise job is completed and vertex array is correctly assigned
        vertices = NoiseGen.CompleteNoiseGenJob(noiseJobData);
    }

    public void StartTextureJob()
    {
        CompleteNoiseJob(); // Ensure vertices are populated before starting the texture job
        textureJobData = TextureGen.ScheduleTextureGenJob(vertices);
    }

    public void CompleteTextureJob()
    {
        texture = TextureGen.CompleteTextureGenJob(textureJobData);
    }

    public void CreateMesh()
    {
        mesh = MeshGen.GenerateMesh(vertices, 0);
        vertices.Dispose(); // Dispose of vertices as they are no longer needed
    }

    public void StartMeshJob()
    {
        // meshJobData = MeshGen.ScheduleMeshGenJob(vertices);
    }

    public NativeArray<float3> GetVertices() => vertices;

    public Texture2D GetTexture() => texture;

    public Mesh GetMeshes() => mesh;

    public bool IsNoiseJobComplete() => noiseJobData.JobHandle.IsCompleted;

    // Create a public bool with getter setter for if we are ready to start the texture job, because just because the noise job is complete doesn't mean we are ready to start the texture job
    // public bool IsReadyToStartJob { get; set; } = false;

    public bool IsTextureJobComplete() => textureJobData.JobHandle.IsCompleted;
    // public bool IsMeshJobComplete() => meshJobData.JobHandle.IsCompleted;

    public float2 GetWorldSpacePosition() => worldSpacePosition;

    private void DisposeNoiseJobData()
    {
        if (noiseJobData.Data.IsCreated)
        {
            noiseJobData.Dispose();
        }
    }
}
