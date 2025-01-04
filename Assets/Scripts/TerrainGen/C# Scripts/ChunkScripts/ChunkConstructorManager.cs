using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class ChunkConstructorManager
{
    // TODO: Maybe make these arrays with a reasonable size. Somehow get the users cpu specs and adjust the size accordingly.
    private static readonly List<ChunkConstructor> NoiseJobs = new();
    private static readonly List<ChunkConstructor> TextureJobs = new();
    private static readonly List<ChunkConstructor> FinishedConstructors = new();

    public static Transform ParentTransform { get; set; }

    public static void QueueChunkGeneration(float2 position)
    {
        int chunkX = (int)(position.x / ChunkGlobals.WorldSpaceChunkSize);
        int chunkY = (int)(position.y / ChunkGlobals.WorldSpaceChunkSize);
        string chunkName = $"Terrain Chunk: ({chunkX}, {chunkY})";

        GameObject terrainChunk = new(chunkName)
        {
            transform = { parent = ParentTransform }
        };

        Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
        chunkComponent.SetParent(terrainChunk);
        ChunkRegistry.GetGeneratedChunksDictionary().Add(position, chunkComponent);

        StartNoiseJob(position);
    }

    private static void StartNoiseJob(float2 position)
    {
        var chunkConstructor = new ChunkConstructor();
        chunkConstructor.StartNoiseJob(position);
        NoiseJobs.Add(chunkConstructor);
    }

    public static void HandleChunkGeneration()
    {
        ProcessNoiseJobs();
        ProcessTextureJobs();
        InitializeFinishedChunks();
    }

    private static void ProcessNoiseJobs()
    {
        var completedNoiseJobs = new List<ChunkConstructor>();

        foreach (var job in NoiseJobs)
        {
            if (job.IsNoiseJobComplete())
            {
                job.CompleteNoiseJob();
                job.StartTextureJob();
                TextureJobs.Add(job);
                completedNoiseJobs.Add(job);
            }
        }

        foreach (var job in completedNoiseJobs)
        {
            NoiseJobs.Remove(job);
        }
    }

    private static void ProcessTextureJobs()
    {
        var completedTextureJobs = new List<ChunkConstructor>();

        foreach (var job in TextureJobs)
        {
            if (job.IsTextureJobComplete())
            {
                job.CompleteTextureJob();
                job.CreateMesh();
                FinishedConstructors.Add(job);
                completedTextureJobs.Add(job);
            }
        }

        foreach (var job in completedTextureJobs)
        {
            TextureJobs.Remove(job);
        }
    }

    private static void InitializeFinishedChunks()
    {
        foreach (var constructor in FinishedConstructors)
        {
            float2 position = constructor.GetWorldSpacePosition();
            if (ChunkRegistry.GetGeneratedChunksDictionary().TryGetValue(position, out var chunk))
            {
                chunk.Initialize(constructor.GetMeshes(), constructor.GetTexture(), position);
            }
        }

        FinishedConstructors.Clear();
    }
}
