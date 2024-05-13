using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct NoiseGenJob : IJobParallelFor
{
    public NativeArray<Vector3> vertexArray;
    public float scale;
    public float worldSpaceChunkSize;
    public int meshLengthInVertices;
    public float heightMultiplier;
    public float worldSpaceChunkCenterX;
    public float worldSpaceChunkCenterZ;

    public void Execute(int index)
    {
        float stepSize = worldSpaceChunkSize / (meshLengthInVertices - 1);
        float initialCoord = -worldSpaceChunkSize / 2 + worldSpaceChunkCenterX;
        float zPosInitialCoord = -worldSpaceChunkSize / 2 + worldSpaceChunkCenterZ;

        float xPos = initialCoord + index % meshLengthInVertices * stepSize;
        float zPos = zPosInitialCoord + index / meshLengthInVertices * stepSize;

        vertexArray[index] = new Vector3(xPos - worldSpaceChunkCenterX, noise.snoise(new float2(xPos, zPos)) * heightMultiplier, zPos - worldSpaceChunkCenterZ);
    }
}
