using UnityEngine;

public class GetNoiseFromCompute : MonoBehaviour
{
    public struct Point
    {
        public Vector2 worldSpaceChunkCenter;
        public float height;
    };

    [SerializeField] private ComputeShader noiseGen; // Assign in the Inspector instead of loading via AssetDatabase

    private void Awake()
    {
        if (!noiseGen)
        {
            Debug.LogError("ComputeShader is not assigned.");
        }
    }

    public float[] GetNoiseMap(Vector2 worldSpacePosition, int meshSpaceChunkSize, float worldSpaceChunkSize)
    {
        int vertexNum = (meshSpaceChunkSize + 1) * (meshSpaceChunkSize + 1);
        float[] heights = new float[vertexNum];

        int stride = sizeof(float) * 2 + sizeof(float); // Adjusted stride to account for struct layout in memory

        ComputeBuffer buffer = new(vertexNum, stride);
        Point[] pointsData = new Point[vertexNum];

        for (int i = 0; i < vertexNum; i++)
        {
            pointsData[i].worldSpaceChunkCenter = worldSpacePosition;
        }

        buffer.SetData(pointsData);

        noiseGen.SetBuffer(0, "points", buffer);
        noiseGen.SetFloat("worldSpaceChunkSize", worldSpaceChunkSize);
        noiseGen.SetInt("meshLengthInVertices", meshSpaceChunkSize + 1);

        // Calculate the correct number of thread groups based on your data
        int threadGroups = Mathf.CeilToInt(vertexNum / 1024f);
        noiseGen.Dispatch(0, threadGroups, 1, 1);

        // Retrieve only the heights from the buffer
        Point[] heightsData = new Point[vertexNum];
        buffer.GetData(heightsData);
        for (int i = 0; i < vertexNum; i++)
        {
            heights[i] = heightsData[i].height;
        }

        buffer.Release();
        return heights;
    }
}