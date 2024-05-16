using UnityEngine;
using Unity.Collections;
using System;

public class ChunkConstructor
{
    private JobData<Vector3> noiseJobData;
    private JobData<Color> textureJobData;
    private NativeArray<Vector3> vertices;
    private Texture2D texture;
    private Mesh[] meshes;
    private Vector2 worldSpacePosition;

    public void StartNoiseJob(Vector2 position)
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
        // Ensure the noise job is complete before starting the texture job
        if (!noiseJobData.jobHandle.IsCompleted)
        {
            throw new InvalidOperationException("Noise job must be completed before starting the texture job.");
        }

        CompleteNoiseJob(); // Ensure vertices are populated before starting the texture job
        textureJobData = TextureGen.ScheduleTextureGenJob(vertices);
    }

    public void CompleteTextureJob()
    {
        texture = TextureGen.CompleteTextureGenJob(textureJobData);
    }

    public void CreateMesh()
    {
        // Ensure the noise job is complete before creating the mesh
        if (!noiseJobData.jobHandle.IsCompleted)
        {
            throw new InvalidOperationException("Noise job must be completed before creating the mesh.");
        }

        meshes = MeshGen.GetMeshes(vertices);
        vertices.Dispose(); // Dispose of vertices as they are no longer needed
    }

    public NativeArray<Vector3> GetVertices() => vertices;

    public Texture2D GetTexture() => texture;

    public Mesh[] GetMeshes() => meshes;

    public bool IsNoiseJobComplete() => noiseJobData.jobHandle.IsCompleted;

    public bool IsTextureJobComplete() => textureJobData.jobHandle.IsCompleted;

    public Vector2 GetWorldSpacePosition() => worldSpacePosition;

    private void DisposeNoiseJobData()
    {
        if (noiseJobData.data.IsCreated)
        {
            noiseJobData.Dispose();
        }
    }
}
