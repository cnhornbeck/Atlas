using UnityEngine;

public class LODTriangleArrayGen : MonoBehaviour
{

    public static int[][] triangleArrays;
    // Start is called before the first frame update
    void Awake()
    {
        triangleArrays = new int[ChunkGlobals.lodCount][];
        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            triangleArrays[i] = GenerateLODTriangleArray(ChunkGlobals.meshSpaceChunkSize + 1, i);
        }
    }

    static int[] GenerateLODTriangleArray(int meshLengthInVertices, int lod)
    {
        int lodMeshLengthInVertices = ((meshLengthInVertices - 1) / (int)Mathf.Pow(2, lod)) + 1;

        if (lodMeshLengthInVertices < 2)
        {
            lodMeshLengthInVertices = 2;
            lod = (int)(Mathf.Log(meshLengthInVertices - 1) / Mathf.Log(2));
        }

        // Calculate the total number of triangles needed for the mesh
        int numTriangles = 6 * (lodMeshLengthInVertices - 1) * (lodMeshLengthInVertices - 1);
        // Create an array to hold the triangle indices
        int[] triangles = new int[numTriangles];

        // Iterate over each square in the mesh, adding two triangles per square
        int scaleFactor = (int)Mathf.Pow(2, lod);
        int jumpFactor = ChunkGlobals.meshSpaceChunkSize - (int)Mathf.Pow(2, ChunkGlobals.lodCount - (lod + 1)) + 1;
        int j = 0;
        // int count = 0;
        for (int i = 0; i < triangles.Length;)
        {
            // If j is not at the right edge of the terrain, create two triangles for the square. 
            // The right edge of the terrain should not have triangles added to it.
            if (j % meshLengthInVertices < (lodMeshLengthInVertices - 1))
            {
                // Add the indices for the two triangles in the square to the array.
                triangles[i++] = j * scaleFactor;
                triangles[i++] = (j + meshLengthInVertices + 1) * scaleFactor;
                triangles[i++] = (j + 1) * scaleFactor;
                triangles[i++] = j * scaleFactor;
                triangles[i++] = (j + meshLengthInVertices) * scaleFactor;
                triangles[i++] = (j + meshLengthInVertices + 1) * scaleFactor;
                j++;

            }
            else
            {
                j += jumpFactor;
            }
        }

        // Print the triangle array to the console by forming a string of three indices at a time

        // if (lod == 0)
        // {
        //     Debug.Log($"Scale Factor: {scaleFactor}");
        //     Debug.Log($"Mesh Length Vertices {meshLengthInVertices}");
        //     Debug.Log($"lod Mesh Length Vertices {lodMeshLengthInVertices}");
        //     Debug.Log($"Jump Factor: {ChunkGlobals.meshSpaceChunkSize - (int)Mathf.Pow(2, ChunkGlobals.lodCount - (lod + 1)) + 1}");
        //     Debug.Log($"Jump Power: {(int)Mathf.Pow(2, ChunkGlobals.lodCount - (lod + 1))}");
        //     string triangleString = "";
        //     for (int i = 0; i < triangles.Length; i += 3)
        //     {
        //         triangleString += triangles[i] + ", " + triangles[i + 1] + ", " + triangles[i + 2] + "\n";
        //     }
        //     Debug.Log(triangleString);
        // }

        // Return the array of triangle indices
        return triangles;
    }
}
