using System;
using UnityEngine;

public class ChunkGlobals : MonoBehaviour
{
    public const int lodCount = 10;

    // Size of chunk is measured by number of edges per chunk
    public static readonly int meshSpaceChunkSize = (int)Math.Pow(2, lodCount - 1);

    // Size of the actual mesh in the scene
    public const float worldSpaceChunkSize = 10;
    public const int heightMultiplier = 4;
    public static int renderDistance = 5;
    public static int[][] triangleArrays;
    public static Vector2[][] uvArrays;
    public static Vector3[][] vertexArrays;
}
