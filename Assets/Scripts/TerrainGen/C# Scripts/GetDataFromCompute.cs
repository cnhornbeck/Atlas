using UnityEngine;

public class GetDataFromCompute : MonoBehaviour
{
    [SerializeField] private ComputeShader noiseGen;

    private ComputeBuffer vertexBuffer;
    private Vector3[] vertexData;
    private RenderTexture heightTexture;

    private static NoiseSettings settings = new();

    void Start()
    {
        SetStaticShaderParameters();
    }

    public void CalculateMeshData(Vector2 worldSpacePosition)
    {
        int meshSpaceChunkSize = ChunkGlobals.meshSpaceChunkSize;
        int vertexNum = (meshSpaceChunkSize + 1) * (meshSpaceChunkSize + 1);
        vertexData = new Vector3[vertexNum];

        vertexBuffer = new ComputeBuffer(vertexNum, sizeof(float) * 3);
        vertexBuffer.SetData(vertexData);

        noiseGen.SetBuffer(0, "vertices", vertexBuffer);
        noiseGen.SetBuffer(1, "vertices", vertexBuffer);

        noiseGen.SetFloat("worldSpaceChunkCenterX", worldSpacePosition.x);
        noiseGen.SetFloat("worldSpaceChunkCenterZ", worldSpacePosition.y);

        noiseGen.Dispatch(0, Mathf.CeilToInt(vertexNum / 1024f), 1, 1); // Dispatch GetVertices

        vertexBuffer.GetData(vertexData);

        int textureSize = meshSpaceChunkSize;
        heightTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        heightTexture.Create();
        noiseGen.SetTexture(1, "outputTexture", heightTexture);
        noiseGen.Dispatch(1, Mathf.CeilToInt((float)textureSize / 8), Mathf.CeilToInt((float)textureSize / 8), 1);

        vertexBuffer.Release(); // Release the buffer
    }

    private void SetStaticShaderParameters()
    {

        // Set float values
        noiseGen.SetFloat("worldSpaceChunkSize", ChunkGlobals.worldSpaceChunkSize);
        noiseGen.SetFloat("frequency", settings.Scale);
        noiseGen.SetFloat("lacunarity", settings.Lacunarity);
        noiseGen.SetFloat("persistence", settings.Persistence);

        // Set int values
        noiseGen.SetInt("meshLengthInVertices", ChunkGlobals.meshSpaceChunkSize + 1);
        noiseGen.SetInt("heightMultiplier", ChunkGlobals.heightMultiplier);
        noiseGen.SetInt("seed", settings.Seed);
        noiseGen.SetInt("octaves", settings.Octaves);

        Texture colorLookupTexture = TextureGen.lookupTexture;

        noiseGen.SetTexture(1, "colorLookupTexture", colorLookupTexture);
        noiseGen.SetInt("textureWidth", colorLookupTexture.width);

        // Debug.Log("Set static shader parameters");
    }

    public Vector3[] GetHeightData()
    {
        return vertexData;
    }

    public Texture GetTextureData()
    {
        return heightTexture;
    }
}
