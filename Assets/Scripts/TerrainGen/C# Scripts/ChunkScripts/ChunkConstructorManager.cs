using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class ChunkConstructorManager
{
    // TODO: Maybe make these arrays with a reasonable size. Somehow get the users cpu specs and adjust the size accordingly.
    private static readonly int chunksInRenderDistance = Mathf.CeilToInt(ChunkGlobals.renderDistance * ChunkGlobals.renderDistance * math.PI);
    private static readonly List<ChunkConstructor> NoiseJobs = new();
    private static readonly List<ChunkConstructor> TextureJobs = new();
    private static readonly List<ChunkConstructor> FinishedConstructors = new();
    private static FixedSizeDeque<ChunkConstructionData> ChunksToGenerate = new(chunksInRenderDistance);
    private static int MaxMeshTextureJobs = 15;
    private static int MaxNoiseJobs = MaxMeshTextureJobs * 2;
    private static HashSet<float2> chunksStartedThisFrame = new();
    public static Transform ParentTransform { get; set; }

    public static void AddChunkToQueue(float2 position)
    {
        ChunksToGenerate.AddFront(new ChunkConstructionData { position = position, lod = 0 });
    }

    public static HashSet<float2> StartChunkConstructionJobs()
    {

        if (NoiseJobs.Count >= MaxNoiseJobs)
        {
            return chunksStartedThisFrame;
        }

        // int count = 0;
        while (NoiseJobs.Count < MaxNoiseJobs && ChunksToGenerate.Count > 0)
        {
            ChunkConstructionData chunkConstructionData = ChunksToGenerate.PopFront();

            // Debug.Log("Count: " + count);
            // Debug.Log("NoiseJobs Count: " + NoiseJobs.Count);
            // Debug.Log("Chunks To Generate Count: " + ChunksToGenerate.Count);

            float2 position = chunkConstructionData.position;

            int chunkX = (int)(position.x / ChunkGlobals.WorldSpaceChunkSize);
            int chunkY = (int)(position.y / ChunkGlobals.WorldSpaceChunkSize);
            string chunkName = $"Terrain Chunk: ({chunkX}, {chunkY})";

            GameObject terrainChunk = new(chunkName)
            {
                transform = { parent = ParentTransform }
            };

            Chunk chunkComponent = terrainChunk.AddComponent<Chunk>();
            chunkComponent.SetParent(terrainChunk);
            ChunkRegistry.GetGeneratedChunksDictionary().Add(chunkConstructionData.position, chunkComponent);

            var chunkConstructor = new ChunkConstructor();
            chunkConstructor.StartNoiseJob(chunkConstructionData);
            NoiseJobs.Add(chunkConstructor);

            chunksStartedThisFrame.Add(chunkConstructionData.position);
            // count++;
        }

        return chunksStartedThisFrame;
    }

    public static bool IsQueuedForConstruction(float2 position)
    {
        if (ChunksToGenerate.Contains(new ChunkConstructionData { position = position, lod = 0 }))
        {
            return true;
        }
        return false;
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

        int count = 0;
        foreach (var job in TextureJobs)
        {
            if (count >= MaxMeshTextureJobs)
            {
                break;
            }

            if (job.IsTextureJobComplete())
            {
                job.CompleteTextureJob();
                job.CreateMesh();
                FinishedConstructors.Add(job);
                completedTextureJobs.Add(job);
            }
            count++;
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

public struct ChunkConstructionData
{
    public float2 position;
    public int lod;
}
