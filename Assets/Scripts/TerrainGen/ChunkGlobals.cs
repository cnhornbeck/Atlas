using UnityEngine;
public class ChunkGlobals : MonoBehaviour
{
    public const int lodNumSize = 4;
    // Controls the resolution at which the noise is to be generated. lodNum of 2 means half resolution.
    public static readonly int[] lodNumArray = new int[lodNumSize] { 1, 2, 8, 16 };
    // Controls distances at which the next level of detail will be used
    public static readonly float[] lodCutoffArray = new float[lodNumSize] { 0.1f, 0.25f, 0.5f, 1f };

    // Size of chunk is measured by number of edges per chunk
    public const int meshSpaceChunkSize = 128;

    // Size of the actual mesh in the scene
    public const float worldSpaceChunkSize = 32;
    public const float heightMultiplier = 100;
    public static int renderDistance = 20;
}
