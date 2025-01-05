using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

// TODO - Implement MeshGenJob
// TODO - Implement Seamless LODs

// [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
// public struct MeshGenJob : IJobParallelFor
// {
//     [ReadOnly] public NativeArray<float3> vertexArray;
//     [ReadOnly] public int lodLevel;
//     // [WriteOnly] public Mesh mesh;

//     public void Execute(int index)
//     {
//         //     meshes[index] = MeshGen.GenerateSingleMesh(vertexArray.ToArray(), lodLevel);
//     }
// }

public class MeshGen
{
    public static Mesh GenerateMesh(NativeArray<float3> vertices, int lodLevel)
    {
        Mesh terrainMesh = new Mesh
        {
            name = $"TerrainMesh_LOD{lodLevel}",
            indexFormat = IndexFormat.UInt32
        };

        int[] triangles = ChunkGlobals.triangleArrays[lodLevel];
        float2[] uvs = ChunkGlobals.uvArrays[0];

        int vertexCount = vertices.Length;
        int triangleIndexCount = triangles.Length;

        terrainMesh.SetVertexBufferParams(
            vertexCount,
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, stream: 0),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, stream: 1)
        );
        terrainMesh.SetVertexBufferData(vertices, 0, 0, vertexCount, stream: 0);
        terrainMesh.SetVertexBufferData(uvs, 0, 0, vertexCount, stream: 1);

        terrainMesh.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt32);
        terrainMesh.SetIndexBufferData(triangles, 0, 0, triangleIndexCount);

        var bounds = new Bounds();
        terrainMesh.bounds = bounds;

        terrainMesh.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
        {
            bounds = bounds,
            vertexCount = vertexCount

        });

        // TODO: Implement Better Recalculate Bounds Method
        terrainMesh.RecalculateBounds();

        return terrainMesh;
    }

    // private static int CalculateTotalLODCount()
    // {
    //     return 1;
    // }

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
