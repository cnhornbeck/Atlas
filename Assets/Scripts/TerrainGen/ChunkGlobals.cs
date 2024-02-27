using UnityEngine;
public class ChunkGlobals : MonoBehaviour
{
    public const int lodNumSize = 4;
    // Controls the resolution at which the noise is to be generated. lodNum of 2 means half resolution.
    public static readonly int[] lodNumArray = new int[lodNumSize] { 1, 2, 4, 8 };
    // Controls distances at which the next level of detail will be used
    public static readonly int[] lodDistArray = new int[lodNumSize - 1] { 50, 100, 200 };

    // Size of chunk is measured by number of edges per chunk
    public const int meshSpaceChunkSize = 32;

    // Size of the actual mesh in the scene
    public const float worldSpaceChunkSize = 32;
    public static int renderDistance = 10;


}
