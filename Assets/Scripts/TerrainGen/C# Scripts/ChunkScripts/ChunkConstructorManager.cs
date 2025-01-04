using System.Collections.Generic;
using UnityEngine;

public class ChunkConstructorManager
{
    private static List<ChunkConstructor> noiseJobs = new();
    private static List<ChunkConstructor> textureJobs = new();
    private static List<ChunkConstructor> finishedChunkConstructors = new();
    public static Transform parentTransform;

    public static void QueueChunkGeneration(Vector2 position)
    {
        string chunkName = "Terrain Chunk: (" + (int)position.x / ChunkGlobals.worldSpaceChunkSize + ", " + (int)position.y / ChunkGlobals.worldSpaceChunkSize + ")";
        GameObject terrainChunk = new(chunkName);
        terrainChunk.transform.parent = parentTransform;

        Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
        chunkComponent.SetParent(terrainChunk);
        StartChunkGeneration(position);

        ChunkRegistry.GetGeneratedChunksDictionary().Add(position, chunkComponent);
    }


    /// <summary>
    /// Initiates the chunk generation process for a given position.
    /// </summary>
    /// <param name="position">The position of the chunk to generate.</param>
    private static void StartChunkGeneration(Vector2 position)
    {
        var newChunkConstructor = new ChunkConstructor();
        newChunkConstructor.StartNoiseJob(position);
        noiseJobs.Add(newChunkConstructor);
    }

    /// <summary>
    /// Processes completed noise jobs and starts texture generation for them.
    /// </summary>
    private static void StartMeshAndTextureGeneration()
    {
        var finishedNoiseJobs = new List<ChunkConstructor>();

        foreach (var job in noiseJobs)
        {
            if (job.IsNoiseJobComplete())
            {
                job.CompleteNoiseJob();

                job.StartTextureJob();
                textureJobs.Add(job);

                // job.StartMeshJob();
                // meshJobs.Add(job);

                finishedNoiseJobs.Add(job);
            }
        }

        // Remove completed noise jobs
        foreach (var finishedJob in finishedNoiseJobs)
        {
            noiseJobs.Remove(finishedJob);
        }
    }

    /// <summary>
    /// Retrieves and processes finished chunks.
    /// </summary>
    /// <returns>A list of finished chunk constructors.</returns>
    private static List<ChunkConstructor> GetFinishedChunks()
    {
        var finishedChunks = new List<ChunkConstructor>();
        var finishedTextureJobs = new List<ChunkConstructor>();
        int completedJobs = 0;

        foreach (var job in textureJobs)
        {
            if (job.IsTextureJobComplete())
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

    public static void HandleChunkGeneration(){
        StartMeshAndTextureGeneration();
        HandleFinishedChunks();
    }

    private static void HandleFinishedChunks()
    {
        finishedChunkConstructors = GetFinishedChunks();
        foreach (ChunkConstructor chunkConstructor in finishedChunkConstructors)
        {
            Vector2 chunkPosition = chunkConstructor.GetWorldSpacePosition();
            Chunk chunk = ChunkRegistry.GetGeneratedChunksDictionary()[chunkPosition];
            chunk.Initialize(chunkConstructor.GetMeshes(), chunkConstructor.GetTexture(), chunkPosition);
            // chunksVisibleLastFrame.Add(chunkPosition);
        }
        finishedChunkConstructors.Clear();
    }
}