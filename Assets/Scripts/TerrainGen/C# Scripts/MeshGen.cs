using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Rendering;

/// <summary>
/// Handles mesh generation for terrain chunks with multiple levels of detail (LOD).
/// </summary>
public class MeshGen
{
    /// <summary>
    /// Generates an array of meshes with different levels of detail.
    /// </summary>
    /// <param name="vertexArray">The vertex data for the mesh.</param>
    /// <returns>An array of meshes with different LODs.</returns>
    public static Mesh[] GenerateMeshes(NativeArray<Vector3> vertexArray)
    {
        int totalLodCount = CalculateTotalLODCount();
        Mesh[] meshes = new Mesh[totalLodCount];

        GenerateHighResolutionLODs(meshes, vertexArray);
        GenerateLowResolutionLOD(meshes, vertexArray);

        Array.Reverse(meshes);  // Arrange meshes from lowest to highest detail
        return meshes;
    }

    /// <summary>
    /// Generates a single mesh for a specific level of detail.
    /// </summary>
    /// <param name="vertices">The vertex data for the mesh.</param>
    /// <param name="lodLevel">The level of detail to generate.</param>
    /// <returns>A generated mesh for the specified LOD.</returns>
    public static Mesh GenerateSingleMesh(Vector3[] vertices, int lodLevel)
    {
        int[] triangles = ChunkGlobals.triangleArrays[lodLevel];
        Vector2[] uvs = ChunkGlobals.uvArrays[0];

        Mesh terrainMesh = new Mesh
        {
            name = $"TerrainMesh_LOD{lodLevel}",
            indexFormat = IndexFormat.UInt32
        };

        terrainMesh.SetVertices(vertices);
        terrainMesh.triangles = triangles;
        terrainMesh.RecalculateNormals();
        terrainMesh.SetUVs(0, new List<Vector2>(uvs));

        return terrainMesh;
    }

    private static int CalculateTotalLODCount()
    {
        int lodCount = ChunkGlobals.lodCount;
        int highestLodCount = Math.Min(3, lodCount);
        return highestLodCount + (lodCount > highestLodCount ? 1 : 0);
    }

    private static void GenerateHighResolutionLODs(Mesh[] meshes, NativeArray<Vector3> vertexArray)
    {
        int highestLodCount = Math.Min(3, ChunkGlobals.lodCount);
        for (int i = 0; i < highestLodCount; i++)
        {
            int lodIndex = ChunkGlobals.lodCount - 1 - i;
            meshes[i] = GenerateSingleMesh(vertexArray.ToArray(), lodIndex);
        }
    }

    private static void GenerateLowResolutionLOD(Mesh[] meshes, NativeArray<Vector3> vertexArray)
    {
        if (ChunkGlobals.lodCount > 3)
        {
            meshes[3] = GenerateSingleMesh(vertexArray.ToArray(), 0);
        }
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
