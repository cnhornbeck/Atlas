using UnityEngine;

public class MeshPrune
{

    public static float[][] GetHeightValueArrays(float[] heightArray, int numberOfLODs)
    {
        numberOfLODs -= 1;
        // Initialize the 2D array with the number of LODs as the outer array length
        float[][] lodArrays = new float[numberOfLODs][];
        // Calculate the vertex count of the input height array
        int meshLengthInVertices = Mathf.RoundToInt(Mathf.Sqrt(heightArray.Length));
        // Debug.Log($"Mesh length in vertices: {meshLengthInVertices}");
        // Calculate the initial number of vertices for the first LOD
        int currentMeshLengthInVertices = Mathf.RoundToInt((meshLengthInVertices - 1) / 2f + 1);

        for (int i = 0; i < numberOfLODs; i++)
        {
            // Use the CreateLODArray function to fill in the LOD arrays
            lodArrays[i] = CreateLODArray(heightArray, currentMeshLengthInVertices, meshLengthInVertices, i);
            // Update the vertex count for the next LOD, ensuring it does not go below the minimum
            currentMeshLengthInVertices = Mathf.Max((currentMeshLengthInVertices - 1) / 2 + 1, 2);
        }

        return lodArrays;
    }

    private static float[] CreateLODArray(float[] heightArray, int currentMeshLengthInVertices, int meshLengthInVertices, int lodLevel)
    {
        float[] lodArray = new float[currentMeshLengthInVertices * currentMeshLengthInVertices];
        // Calculate the base scale factor as 2 raised to the power of (lodLevel + 1)
        float baseScaleFactor = Mathf.Pow(2, lodLevel + 1);

        // Determine the maximum allowable scale factor based on mesh dimensions
        int maxAllowedScaleFactor = meshLengthInVertices - 1;

        // Choose the smaller of the two values and round to an integer
        int scaleFactor = Mathf.RoundToInt(Mathf.Min(baseScaleFactor, maxAllowedScaleFactor));


        // Compare the two values in the above min function
        // Debug.Log($"{Mathf.Pow(2, lodLevel + 1)} vs {meshLengthInVertices - 1}");
        // Debug.Log($"Scale factor: {scaleFactor}");
        // Debug.Log("");

        for (int y = 0; y < currentMeshLengthInVertices; y++)
        {
            for (int x = 0; x < currentMeshLengthInVertices; x++)
            {
                // Ensure the sampling does not go out of bounds
                int index = scaleFactor * (x + meshLengthInVertices * y);
                lodArray[x + currentMeshLengthInVertices * y] = heightArray[index];
            }
        }

        // Debug.Log(lodArray.Length);

        return lodArray;
    }


}
