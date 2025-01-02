using System;
using UnityEngine;

public class ChunkGlobals : MonoBehaviour
{
    public const int lodCount = 1;

    // Size of chunk is measured by number of edges per chunk at highest detail LOD
    public static readonly int meshSpaceChunkSize = 64;

    // Size of the actual mesh in the scene
    public const float worldSpaceChunkSize = 10;
    public const int heightMultiplier = 50;
    public static int renderDistance = 10;
    public static int[][] triangleArrays;
    public static Vector2[][] uvArrays;
}
