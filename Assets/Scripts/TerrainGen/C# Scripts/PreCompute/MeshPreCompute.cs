using UnityEngine;

public class MeshPreCompute : MonoBehaviour
{
    void Awake()
    {
        ChunkGlobals.vertexArrays = GetLODVertexArrays();
        ChunkGlobals.triangleArrays = GetLODTriangleArrays();
        ChunkGlobals.uvArrays = GetLODUVArrays();
    }

    public static Vector3[][] GetLODVertexArrays()
    {
        Vector3[][] vertexArrays = new Vector3[ChunkGlobals.lodCount][];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            vertexArrays[i] = GenerateLODVertexArray(ChunkGlobals.meshSpaceChunkSize + 1, i);
        }

        return vertexArrays;
    }

    private static Vector3[] GenerateLODVertexArray(int meshLengthInVertices, int lod)
    {
        int lodMeshLengthInVertices = ((meshLengthInVertices - 1) / (int)Mathf.Pow(2, lod)) + 1;

        if (lodMeshLengthInVertices < 2)
        {
            lodMeshLengthInVertices = 2;
            lod = (int)(Mathf.Log(meshLengthInVertices - 1) / Mathf.Log(2));
        }

        // Create an array to hold the vertices
        Vector3[] vertices = new Vector3[lodMeshLengthInVertices * lodMeshLengthInVertices];

        // Iterate over each vertex in the mesh, setting the y-coordinate of each vertex to 0
        for (int i = 0, y = 0; y < lodMeshLengthInVertices; y++)
        {
            for (int x = 0; x < lodMeshLengthInVertices; x++, i++)
            {
                vertices[i] = new Vector3(x * (int)Mathf.Pow(2, lod), 0, y * (int)Mathf.Pow(2, lod));
            }
        }

        // Return the array of vertices
        return vertices;
    }

    public static int[][] GetLODTriangleArrays()
    {
        int[][] triangleArrays = new int[ChunkGlobals.lodCount][];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            triangleArrays[i] = GenerateLODTriangleArray(ChunkGlobals.meshSpaceChunkSize + 1, i);
        }

        return triangleArrays;
    }

    private static int[] GenerateLODTriangleArray(int meshLengthInVertices, int lod)
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

        // Return the array of triangle indices
        return triangles;
    }

    public static Vector2[][] GetLODUVArrays()
    {
        Vector2[][] uvArrays = new Vector2[ChunkGlobals.lodCount][];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            uvArrays[i] = GenerateLODUVArray(i);
        }

        return uvArrays;
    }

    private static Vector2[] GenerateLODUVArray(int lod)
    {
        int size = ChunkGlobals.meshSpaceChunkSize / (int)Mathf.Pow(2, lod) + 1;
        Vector2[] uvs = new Vector2[size * size];
        for (int i = 0, y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++, i++)
            {
                uvs[i] = new Vector2(x / (float)(size - 1), y / (float)(size - 1));
            }
        }
        return uvs;
    }
}
