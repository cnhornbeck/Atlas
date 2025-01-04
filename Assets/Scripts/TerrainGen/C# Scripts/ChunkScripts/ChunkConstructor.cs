using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class ChunkConstructor
{
    private JobData<float3> noiseJobData;
    private JobData<Color> textureJobData;
    private NativeArray<float3> vertices;
    private Texture2D texture;
    private Mesh[] meshes;
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
        meshes = MeshGen.GenerateMeshes(vertices);
        vertices.Dispose(); // Dispose of vertices as they are no longer needed
    }

    public NativeArray<float3> GetVertices() => vertices;

    public Texture2D GetTexture() => texture;

    public Mesh[] GetMeshes() => meshes;

    public bool IsNoiseJobComplete() => noiseJobData.JobHandle.IsCompleted;

    public bool IsTextureJobComplete() => textureJobData.JobHandle.IsCompleted;

    public float2 GetWorldSpacePosition() => worldSpacePosition;

    private void DisposeNoiseJobData()
    {
        if (noiseJobData.Data.IsCreated)
        {
            noiseJobData.Dispose();
        }
    }
}
