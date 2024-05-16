using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class ChunkConstructorManager : MonoBehaviour
{
    static List<ChunkConstructor> noiseJobs = new();
    static List<ChunkConstructor> textureJobs = new();

    public static void StartChunkGeneration(Vector2 position)
    {
        ChunkConstructor newChunkConstructor = new();
        newChunkConstructor.StartNoiseJob(position);
        noiseJobs.Add(newChunkConstructor);
    }

    public static void StartTextureGeneration()
    {
        List<ChunkConstructor> finishedNoiseJobs = new();
        foreach (ChunkConstructor noiseJob in noiseJobs)
        {
            if (noiseJob.GetNoiseJobStatus())
            {
                noiseJob.CompleteNoiseJob();
                noiseJob.StartTextureJob();
                finishedNoiseJobs.Add(noiseJob);
                textureJobs.Add(noiseJob);
            }
        }

        foreach (ChunkConstructor finishedNoiseJob in finishedNoiseJobs)
        {
            noiseJobs.Remove(finishedNoiseJob);
        }
    }

    public static List<ChunkConstructor> GetFinishedChunks()
    {
        List<ChunkConstructor> finishedChunks = new();
        List<ChunkConstructor> finishedTextureJobs = new();
        foreach (ChunkConstructor textureJob in textureJobs)
        {
            if (textureJob.GetTextureJobStatus())
            {
                textureJob.CompleteTextureJob();
                textureJob.CreateMesh();
                finishedTextureJobs.Add(textureJob);
                finishedChunks.Add(textureJob);
            }
        }

        foreach (ChunkConstructor finishedTextureJob in finishedTextureJobs)
        {
            textureJobs.Remove(finishedTextureJob);
        }
        return finishedChunks;
    }
}

public struct JobData<T> where T : struct
{
    public JobHandle jobHandle;
    public NativeArray<T> data;

    public JobData(JobHandle jobHandle, NativeArray<T> data)
    {
        this.jobHandle = jobHandle;
        this.data = data;
    }
}