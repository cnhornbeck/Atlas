using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using System.Threading.Tasks;

public class TextureGen
{
    [BurstCompile]
    public struct TextureGenJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> vertexArray;
        [ReadOnly] public NativeArray<Color> lookupTable;
        public NativeArray<Color> colorData;
        public float numberOfColors;
        public void Execute(int index)
        {
            int textureSize = ChunkGlobals.meshSpaceChunkSize;
            int meshSize = textureSize + 1;

            int x = index % textureSize;
            int y = index / textureSize;

            // Calculate the indices of the vertices
            int bottomLeft = y * meshSize + x;
            int bottomRight = bottomLeft + 1;
            int topLeft = bottomLeft + meshSize;
            int topRight = topLeft + 1;

            float summedHeight =
                vertexArray[bottomLeft].y +
                vertexArray[bottomRight].y +
                vertexArray[topLeft].y +
                vertexArray[topRight].y;

            float smoothedHeight = ((summedHeight / (4 * ChunkGlobals.heightMultiplier)) + 1) / 2;
            int colorIndex = (int)(smoothedHeight * numberOfColors);
            colorData[index] = lookupTable[colorIndex];
        }
    }

    public static JobData<Color> ScheduleTextureGenJob(NativeArray<Vector3> vertexArray)
    {
        int textureSize = ChunkGlobals.meshSpaceChunkSize;

        NativeArray<Color> colorData = new(textureSize * textureSize, Allocator.TempJob);

        TextureGenJob job = new()
        {
            vertexArray = vertexArray,
            lookupTable = TexturePreCompute.lookupTable,
            colorData = colorData,
            numberOfColors = TexturePreCompute.numberOfColors
        };

        JobHandle jobHandle = job.Schedule(colorData.Length, 64);
        return new JobData<Color>(jobHandle, colorData);
    }

    public static Texture2D CompleteTextureGenJob(JobData<Color> jobData)
    {
        jobData.jobHandle.Complete();

        int textureSize = ChunkGlobals.meshSpaceChunkSize;
        Texture2D textureData = new(textureSize, textureSize)
        {
            filterMode = FilterMode.Point, // Ensures sharp edges, important for pixel art or blocky styles
            wrapMode = TextureWrapMode.Clamp // Prevents texture from tiling
        };

        textureData.SetPixels(jobData.data.ToArray());
        textureData.Apply();

        jobData.data.Dispose();

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