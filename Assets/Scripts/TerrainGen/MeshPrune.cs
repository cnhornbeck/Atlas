using UnityEngine;

public class MeshPrune
{

    public static float[][] GetHeightValueArrays(float[] heightArray, int numberOfLODs)
    {
        numberOfLODs -= 1;
        // Initialize the 2D array with the number of LODs as the outer array length
        float[][] lodArrays = new float[numberOfLODs][];
        // Calculate the vertex count of the input height array
        int heightArrayVertexCount = Mathf.RoundToInt(Mathf.Sqrt(heightArray.Length));
        // Calculate the initial number of vertices for the first LOD
        int currentVertexCount = (heightArrayVertexCount - 1) / 2 + 1;

        for (int i = 0; i < numberOfLODs; i++)
        {
            // Use the CreateLODArray function to fill in the LOD arrays
            lodArrays[i] = CreateLODArray(heightArray, currentVertexCount, heightArrayVertexCount, i);
            // Update the vertex count for the next LOD, ensuring it does not go below the minimum
            currentVertexCount = Mathf.Max((currentVertexCount - 1) / 2 + 1, 2);
        }

        return lodArrays;
    }

    private static float[] CreateLODArray(float[] heightArray, int currentVertexCount, int heightArrayVertexCount, int lodLevel)
    {
        float[] lodArray = new float[currentVertexCount * currentVertexCount];
        int scaleFactor = Mathf.RoundToInt(Mathf.Pow(2, lodLevel + 1));

        for (int y = 0; y < currentVertexCount; y++)
        {
            for (int x = 0; x < currentVertexCount; x++)
            {
                // Ensure the sampling does not go out of bounds
                int index = scaleFactor * (x + heightArrayVertexCount * y);
                lodArray[x + currentVertexCount * y] = heightArray[index];
            }
        }

        return lodArray;
    }


}
