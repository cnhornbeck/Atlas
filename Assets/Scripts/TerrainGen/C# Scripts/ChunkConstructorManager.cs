using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class ChunkConstructorManager : MonoBehaviour
{
    private static List<ChunkConstructor> noiseJobs = new List<ChunkConstructor>();
    private static List<ChunkConstructor> textureJobs = new List<ChunkConstructor>();
    private const int MaxJobsPerFrame = 1;

    /// <summary>
    /// Initiates the chunk generation process for a given position.
    /// </summary>
    /// <param name="position">The position of the chunk to generate.</param>
    public static void StartChunkGeneration(Vector2 position)
    {
        var newChunkConstructor = new ChunkConstructor();
        newChunkConstructor.StartNoiseJob(position);
        noiseJobs.Add(newChunkConstructor);
    }

    /// <summary>
    /// Processes completed noise jobs and starts texture generation for them.
    /// </summary>
    public static void StartTextureGeneration()
    {
        var finishedNoiseJobs = new List<ChunkConstructor>();

        foreach (var job in noiseJobs)
        {
            if (job.IsNoiseJobComplete())
            {
                job.CompleteNoiseJob();
                job.StartTextureJob();
                finishedNoiseJobs.Add(job);
                textureJobs.Add(job);
            }
        }

        // Remove completed noise jobs
        foreach (var finishedJob in finishedNoiseJobs)
        {
            noiseJobs.Remove(finishedJob);
        }
    }

    /// <summary>
    /// Retrieves and processes finished chunks, limited by MaxJobsPerFrame.
    /// </summary>
    /// <returns>A list of finished chunk constructors.</returns>
    public static List<ChunkConstructor> GetFinishedChunks()
    {
        var finishedChunks = new List<ChunkConstructor>();
        var finishedTextureJobs = new List<ChunkConstructor>();
        int completedJobs = 0;

        foreach (var job in textureJobs)
        {
            if (job.IsTextureJobComplete() && completedJobs < MaxJobsPerFrame)
            {
                job.CompleteTextureJob();
                job.CreateMesh();
                finishedTextureJobs.Add(job);
                finishedChunks.Add(job);
                completedJobs++;
            }
        }

        // Remove completed texture jobs
        foreach (var finishedJob in finishedTextureJobs)
        {
            textureJobs.Remove(finishedJob);
        }

        return finishedChunks;
    }
}

/// <summary>
/// Struct to manage job data and its disposal.
/// </summary>
/// <typeparam name="T">The type of data in the NativeArray.</typeparam>
public readonly struct JobData<T> : IDisposable where T : struct
{
    public JobHandle JobHandle { get; }
    public NativeArray<T> Data { get; }

    public JobData(JobHandle jobHandle, NativeArray<T> data)
    {
        JobHandle = jobHandle;
        Data = data;
    }

    public void Dispose()
    {
        if (Data.IsCreated)
        {
            Data.Dispose();
        }
    }
}