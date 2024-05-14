using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class MeshGen
{
    public static Mesh[] GetMeshes(NativeArray<Vector3> vertexArray)
    {
        Mesh[] meshes = new Mesh[ChunkGlobals.lodCount];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            meshes[i] = GenerateMesh(vertexArray.ToArray(), i);
        }

        return meshes;
    }

    public static Mesh GenerateMesh(Vector3[] vertexArray, int lod)
    {
        int[] triangles = ChunkGlobals.triangleArrays[lod];
        Vector2[] uvs = ChunkGlobals.uvArrays[0];

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

        return terrainMesh;
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
}
