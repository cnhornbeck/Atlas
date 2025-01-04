using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;

// TODO - Implement MeshGenJob
// TODO - Implement Seamless LODs

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
    public static Mesh[] GenerateMeshes(NativeArray<float3> vertexArray)
    {
        int totalLodCount = CalculateTotalLODCount();
        Mesh[] meshes = new Mesh[totalLodCount];

        meshes[0] = GenerateSingleMesh(vertexArray.ToArray(), 0);

        // GenerateHighResolutionLODs(meshes, vertexArray);
        // GenerateLowResolutionLOD(meshes, vertexArray);

        return meshes;
    }

    /// <summary>
    /// Generates a single mesh for a specific level of detail.
    /// </summary>
    /// <param name="vertices">The vertex data for the mesh.</param>
    /// <param name="lodLevel">The level of detail to generate.</param>
    /// <returns>A generated mesh for the specified LOD.</returns>
    public static Mesh GenerateSingleMesh(float3[] vertices, int lodLevel)
    {
        int[] triangles = ChunkGlobals.triangleArrays[lodLevel];
        float2[] uvs = ChunkGlobals.uvArrays[0];


        #region 
        int vertexAttributeCount = 2;
        int vertexCount = vertices.Length;
        int triangleIndexCount = triangles.Length;

        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(
            vertexAttributeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory
        );

        vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3);
        vertexAttributes[1] = new VertexAttributeDescriptor(
            VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1
        );

        meshData.SetVertexBufferParams(vertexCount, vertexAttributes);
        vertexAttributes.Dispose();

        NativeArray<float3> positions = meshData.GetVertexData<float3>();
        for (int i = 0; i < vertexCount; i++)
        {
            positions[i] = vertices[i];
        }

        NativeArray<float2> texCoords = meshData.GetVertexData<float2>(1);
        for (int i = 0; i < vertexCount; i++)
        {
            texCoords[i] = uvs[i];
        }

        meshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt16);
        NativeArray<ushort> triangleIndices = meshData.GetIndexData<ushort>();
        for (int i = 0; i < triangleIndexCount; i++)
        {
            triangleIndices[i] = (ushort)triangles[i];
        }

        var bounds = new Bounds();
        // Debug.Log($"Bounds: {bounds}");

        meshData.subMeshCount = 1;
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
        {
            bounds = bounds,
            vertexCount = vertexCount
        });

        Mesh terrainMesh = new Mesh
        {
            name = $"TerrainMesh_LOD{lodLevel}",
            bounds = bounds,
            indexFormat = IndexFormat.UInt32
        };

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, terrainMesh);
        // TODO: Implement Better Recalculate Bounds Method
        terrainMesh.RecalculateBounds();
        #endregion


        // terrainMesh.vertices = vertices;
        // terrainMesh.triangles = triangles;
        // terrainMesh.RecalculateNormals();
        // terrainMesh.uv = uvs;

        return terrainMesh;
    }

    private static int CalculateTotalLODCount()
    {
        return 1;
    }

    // private static void GenerateHighResolutionLODs(Mesh[] meshes, NativeArray<float3> vertexArray)
    // {
    //     int highestLodCount = Math.Min(3, ChunkGlobals.lodCount);
    //     for (int i = 0; i < highestLodCount; i++)
    //     {
    //         int lodIndex = ChunkGlobals.lodCount - 1 - i;
    //         meshes[i] = GenerateSingleMesh(vertexArray.ToArray(), lodIndex);
    //     }
    // }

    // private static void GenerateLowResolutionLOD(Mesh[] meshes, NativeArray<float3> vertexArray)
    // {
    //     if (ChunkGlobals.lodCount > 3)
    //     {
    //         meshes[3] = GenerateSingleMesh(vertexArray.ToArray(), 0);
    //     }
    // }

    // private static void FlatShading(float3[] vertices, int[] triangles, float2[] uvs, Mesh mesh)
    // {
    //     float3[] newVertices = new float3[triangles.Length];
    //     float2[] newUVs = new float2[triangles.Length];
    //     for (int i = 0; i < triangles.Length; i++)
    //     {
    //         newVertices[i] = vertices[triangles[i]];
    //         newUVs[i] = uvs[triangles[i]];
    //         triangles[i] = i;
    //     }
    //     mesh.vertices = newVertices;
    //     mesh.uv = newUVs;
    //     mesh.triangles = triangles;
    //     mesh.RecalculateNormals();
    // }
}
