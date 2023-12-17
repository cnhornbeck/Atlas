using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class MeshGen
{
    // This method generates the triangle indices for the mesh
    // The width and height parameters represent the number of vertices in each dimension
    private int[] GenerateMeshTriangles(int chunkSize)
    {
        // Calculate the total number of triangles needed for the mesh
        int numTriangles = 6 * (chunkSize - 1) * (chunkSize - 1);
        // Create an array to hold the triangle indices
        int[] triangles = new int[numTriangles];

        // Iterate over each square in the mesh, adding two triangles per square
        int j = 0;
        for (int i = 0; i < triangles.Length;)
        {
            // If j is not at the right edge of the terrain, create two triangles for the square. 
            // The right edge of the terrain should not have triangles added to it.
            if ((j) % chunkSize != (chunkSize - 1))
            {
                // Add the indices for the two triangles in the square to the array.
                triangles[i++] = j;
                triangles[i++] = j + chunkSize + 1;
                triangles[i++] = j + 1;
                triangles[i++] = j;
                triangles[i++] = j + chunkSize;
                triangles[i++] = j + chunkSize + 1;
            }
            j++;
        }

        // Return the array of triangle indices
        return triangles;
    }

    // This function takes in an array of height values, the width and height of the mesh, the vertical separation between vertices, 
    // the height multiplier, and the water height, and returns an array of Vector3 objects representing the vertices of the mesh.
    private Vector3[] GenerateMeshVertices(float[] heightArray, int chunkSize, float vertSep, float heightMultiplier, float waterHeight)
    {
        // Get the length of the height array, which is the number of vertices in the mesh.
        int size = heightArray.Length;


        // Create a new array to hold the vertex positions.
        Vector3[] verts = new Vector3[size];

        // Calculate the half-width and half-height of the mesh.
        float halfChunkSize = vertSep * chunkSize / 2;

        // Loop through each vertex in the mesh.
        for (int i = 0; i < size; i++)
        {
            // Calculate the y-position of the vertex based on the height value and the water height.
            // The Mathf.Max function ensures that the y-position is at least the water height, so that the water surface is visible.
            float y = Mathf.Max(waterHeight, heightArray[i]) * heightMultiplier;

            // Calculate the x and z positions of the vertex based on the index i and the width and vertical separation of the mesh.
            // The % and / operators are used to calculate the row and column of the vertex.
            verts[i] = new Vector3(
                vertSep * (i % chunkSize) - halfChunkSize,
                y,
                vertSep * (i / chunkSize) - halfChunkSize);
        }

        // Return the array of vertex positions.
        return verts;
    }

    // This method generates the mesh
    public Mesh GenerateMesh(NoiseSettings noiseSettings, float heightMultiplier, float vertSep, float waterHeight)
    {
        // Generate the height array
        float[] heightArray = NoiseGen.GeneratePerlinNoise(noiseSettings);

        // Create the terrain mesh and set its properties
        Mesh terrainMesh = new()
        {
            name = "TerrainMesh",
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        // Generate the mesh vertices
        Vector3[] verts = GenerateMeshVertices(heightArray, noiseSettings.chunkSize, vertSep, heightMultiplier, waterHeight);

        // Generate the triangles
        int[] triangles = GenerateMeshTriangles(noiseSettings.chunkSize);


        // Set the mesh vertices, triangles, and normals
        terrainMesh.SetVertices(verts);
        terrainMesh.triangles = triangles;
        terrainMesh.RecalculateNormals();

        return terrainMesh;
    }
}
