using UnityEngine;
using System.Collections.Generic;

public class MeshGen
{
    public static Mesh[] GenerateMeshes(Vector3[] vertexArray)
    {
        // Generate vertex number
        Mesh[] meshes = new Mesh[ChunkGlobals.lodCount];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            meshes[i] = GenerateMesh(vertexArray, i);
        }

        return meshes;
    }


    public static Mesh GenerateMesh(Vector3[] vertexArray, int lod)
    {
        int meshLengthInVertices = (int)Mathf.Sqrt(vertexArray.Length);
        // Generate the triangles
        // int[] triangles = GenerateMeshTriangles(meshLengthInVertices, lod);

        Vector2[] uvs = CalcUVs(vertexArray);
        // Create the terrain mesh and set its properties
        Mesh terrainMesh = new()
        {
            name = "TerrainMesh",
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        // Set the mesh vertices, triangles, and normals
        terrainMesh.SetVertices(vertexArray);
        // terrainMesh.triangles = triangles;
        terrainMesh.RecalculateNormals();
        terrainMesh.SetUVs(0, new List<Vector2>(uvs));

        // FlatShading(vertices, triangles, uvs, terrainMesh);

        return terrainMesh;
    }


    public static Mesh GenerateMesh(Vector3[] vertexArray)
    {
        int meshLengthInVertices = (int)Mathf.Sqrt(vertexArray.Length);
        // Generate the triangles
        int[] triangles = GenerateMeshTriangles(meshLengthInVertices, 0);

        Vector2[] uvs = CalcUVs(vertexArray);
        // Create the terrain mesh and set its properties
        Mesh terrainMesh = new()
        {
            name = "TerrainMesh",
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        // Set the mesh vertices, triangles, and normals
        terrainMesh.SetVertices(vertexArray);
        terrainMesh.triangles = triangles;
        terrainMesh.RecalculateNormals();
        terrainMesh.SetUVs(0, new List<Vector2>(uvs));

        // FlatShading(vertices, triangles, uvs, terrainMesh);

        return terrainMesh;
    }

    // This method generates the triangle indices for the mesh
    // The width and height parameters represent the number of vertices in each dimension
    private static int[] GenerateMeshTriangles(int meshLengthInVertices, int lod)
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

    private static void FlatShading(Vector3[] vertices, int[] triangles, Vector2[] uvs, Mesh mesh)
    {
        Vector3[] newVertices = new Vector3[triangles.Length];
        Vector2[] newUVs = new Vector2[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            newVertices[i] = vertices[triangles[i]];
            newUVs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }
        mesh.vertices = newVertices;
        mesh.uv = newUVs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // Calculates UVs based on height array.
    private static Vector2[] CalcUVs(Vector3[] heightArray)
    {
        int size = (int)Mathf.Sqrt(heightArray.Length);
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
