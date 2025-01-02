using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public struct TextureGenJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> vertexArray;
    [ReadOnly] public NativeArray<Color> lookupTable;
    [ReadOnly] public int textureSize;
    [ReadOnly] public float numberOfColors;
    [WriteOnly] public NativeArray<Color> colorData;
    public void Execute(int index)
    {
        int meshSizeInVertices = textureSize + 1;

        int x = index % textureSize;
        int y = index / textureSize;

        // Calculate the indices of the vertices
        int bottomLeft = y * meshSizeInVertices + x;
        int bottomRight = bottomLeft + 1;
        int topLeft = bottomLeft + meshSizeInVertices;
        int topRight = topLeft + 1;

        // Ensure the indices are within the bounds of the vertexArray
        if (bottomLeft < 0 || bottomLeft >= vertexArray.Length ||
            bottomRight < 0 || bottomRight >= vertexArray.Length ||
            topLeft < 0 || topLeft >= vertexArray.Length ||
            topRight < 0 || topRight >= vertexArray.Length)
        {
            Debug.LogError($"Index out of bounds: index = {index}, bottomLeft = {bottomLeft}, bottomRight = {bottomRight}, topLeft = {topLeft}, topRight = {topRight}, textureSize = {textureSize}");
            return; // Skip processing for this index
        }

        float summedHeight =
            vertexArray[bottomLeft].y +
            vertexArray[bottomRight].y +
            vertexArray[topLeft].y +
            vertexArray[topRight].y;

        float smoothedHeight = ((summedHeight / (4 * ChunkGlobals.heightMultiplier)) + 1) / 2;
        int colorIndex = (int)(smoothedHeight * numberOfColors);

        // Ensure the colorIndex is within the bounds of the lookupTable
        if (colorIndex < 0 || colorIndex >= lookupTable.Length)
        {
            Debug.LogError($"Color index out of bounds: colorIndex = {colorIndex}, numberOfColors = {numberOfColors}, smoothedHeight = {smoothedHeight}");
            return; // Skip processing for this index
        }

        colorData[index] = lookupTable[colorIndex];
    }
}

public class TextureGen
{
    public static JobData<Color> ScheduleTextureGenJob(NativeArray<Vector3> vertexArray)
    {
        int textureSize = ChunkGlobals.meshSpaceChunkSize;

        NativeArray<Color> colorData = new(textureSize * textureSize, Allocator.TempJob);

        TextureGenJob job = new()
        {
            vertexArray = vertexArray,
            lookupTable = TexturePreCompute.lookupTable,
            textureSize = textureSize,
            colorData = colorData,
            numberOfColors = TexturePreCompute.numberOfColors
        };

        int innerLoopBatchSize = Unity.Mathematics.math.min(64, textureSize * textureSize);

        JobHandle JobHandle = job.Schedule(colorData.Length, innerLoopBatchSize);
        return new JobData<Color>(JobHandle, colorData);
    }

    public static Texture2D CompleteTextureGenJob(JobData<Color> jobData)
    {
        jobData.JobHandle.Complete();

        int textureSize = ChunkGlobals.meshSpaceChunkSize;
        Texture2D textureData = new(textureSize, textureSize)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents texture from tiling
        };

        unsafe
        {
            // Get pointers for source and destination
            void* srcPtr = NativeArrayUnsafeUtility.GetUnsafePtr(jobData.Data);
            Color[] colorArray = new Color[jobData.Data.Length];
            fixed (Color* dstPtr = colorArray)
            {
                // Copy memory
                UnsafeUtility.MemCpy(dstPtr, srcPtr, jobData.Data.Length * UnsafeUtility.SizeOf<Color>());
            }

            // Set pixels with the copied array
            textureData.SetPixels(colorArray);
        }

        textureData.Apply();

        jobData.Dispose();

        return textureData;
    }
}

[Serializable]
public struct TerrainLevelColor
{
    public string Name;
    public Color ColorStart;
    public Color ColorEnd;
    public AnimationCurve Gradient;
    [Range(0f, 1f)] public float MaxHeight;
}