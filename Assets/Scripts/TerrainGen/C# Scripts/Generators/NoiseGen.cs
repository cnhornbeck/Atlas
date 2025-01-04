using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
public struct NoiseSettings
{
    // Ratio between world space and mesh space
    public float Scale => 0.005f;
    public float Lacunarity => 2.5f;
    public float Persistence => 0.3f;
    public int Octaves => 5;
    public int Seed => 1;
}

[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
public struct NoiseGenJob : IJobParallelFor
{
    [WriteOnly] public NativeArray<float3> vertexArray;
    [ReadOnly] public float scale;
    [ReadOnly] public float lacunarity;
    [ReadOnly] public float persistence;
    [ReadOnly] public int octaves;
    [ReadOnly] public int seed;
    [ReadOnly] public float worldSpaceChunkSize;
    [ReadOnly] public int meshLengthInVertices;
    [ReadOnly] public float heightMultiplier;
    [ReadOnly] public float worldSpaceChunkCenterX;
    [ReadOnly] public float worldSpaceChunkCenterZ;

    public void Execute(int index)
    {
        // Ensure index is within bounds before accessing the array
        if (index < 0 || index >= vertexArray.Length)
        {
            return; // Or handle this case appropriately
        }

        float stepSize = worldSpaceChunkSize / (meshLengthInVertices - 1);
        float initialCoord = -worldSpaceChunkSize / 2 + worldSpaceChunkCenterX;
        float zPosInitialCoord = -worldSpaceChunkSize / 2 + worldSpaceChunkCenterZ;

        float xPos = initialCoord + index % meshLengthInVertices * stepSize;
        float zPos = zPosInitialCoord + index / meshLengthInVertices * stepSize;

        float noiseValue = GenerateNoise(xPos, zPos);
        vertexArray[index] = new float3(xPos - worldSpaceChunkCenterX, noiseValue * heightMultiplier, zPos - worldSpaceChunkCenterZ);
    }

    private readonly float GenerateNoise(float x, float z)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0; // Used for normalizing result to 0.0 - 1.0
        Unity.Mathematics.Random random = new((uint)seed);

        for (int i = 0; i < octaves; i++)
        {
            float randomValue = 1000 * ((random.NextFloat() * 2) - 1);

            total += noise.snoise(new float2((x + randomValue) * frequency * scale, (z + randomValue) * frequency * scale)) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }
}

public struct NoiseGen
{
    public static JobData<float3> ScheduleNoiseGenJob(float2 worldSpacePosition)
    {
        NoiseSettings noiseSettings = new();
        NativeArray<float3> vertexArray = new((ChunkGlobals.meshSpaceChunkSize + 1) * (ChunkGlobals.meshSpaceChunkSize + 1), Allocator.TempJob);

        // Setup the job
        NoiseGenJob job = new()
        {
            vertexArray = vertexArray,
            scale = noiseSettings.Scale,
            lacunarity = noiseSettings.Lacunarity,
            persistence = noiseSettings.Persistence,
            octaves = noiseSettings.Octaves,
            seed = noiseSettings.Seed,
            worldSpaceChunkSize = ChunkGlobals.WorldSpaceChunkSize,
            meshLengthInVertices = ChunkGlobals.meshSpaceChunkSize + 1,
            heightMultiplier = ChunkGlobals.heightMultiplier,
            worldSpaceChunkCenterX = worldSpacePosition.x,
            worldSpaceChunkCenterZ = worldSpacePosition.y
        };

        int innerLoopBatchSize = math.min(64, (ChunkGlobals.meshSpaceChunkSize + 1) * (ChunkGlobals.meshSpaceChunkSize + 1));

        JobHandle JobHandle = job.Schedule(vertexArray.Length, innerLoopBatchSize);
        return new JobData<float3>(JobHandle, vertexArray);
    }

    public static NativeArray<float3> CompleteNoiseGenJob(JobData<float3> jobData)
    {
        jobData.JobHandle.Complete();
        return jobData.Data;
    }
}
