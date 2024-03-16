using System;
using UnityEngine;

public class GetNoiseFromCompute : MonoBehaviour
{
    [SerializeField] private ComputeShader noiseGen;

    private ComputeBuffer heightsBuffer;

    float[] heightsData;
    private RenderTexture[] heightTextures;
    private static NoiseSettings settings = NoiseSettings.CreateDefault();

    public void CalculateMeshData(Vector2 worldSpacePosition, int meshSpaceChunkSize, float worldSpaceChunkSize)
    {
        int vertexNum = (meshSpaceChunkSize + 1) * (meshSpaceChunkSize + 1);
        // heightsBuffer = new ComputeBuffer(vertexNum, sizeof(float));

        // // Initialize or reset ComputeBuffer and RenderTexture if not done or if parameters have changed
        heightsBuffer = new ComputeBuffer(vertexNum, sizeof(float));
        heightsData = new float[vertexNum]; // Initialize heights data array only when buffer is recreated
        heightsBuffer.SetData(heightsData); // It's enough to set empty data once upon creation

        // Set shader parameters
        noiseGen.SetBuffer(0, "heights", heightsBuffer);
        noiseGen.SetBuffer(1, "heights", heightsBuffer);
        SetShaderParameters(worldSpaceChunkSize, meshSpaceChunkSize, worldSpacePosition.x, worldSpacePosition.y);

        noiseGen.Dispatch(0, meshSpaceChunkSize, 1, 1); // Dispatch GetHeightValues

        heightsBuffer.GetData(heightsData); // This is needed every time the buffer is updated
        heightsBuffer.Release(); // Release the buffer after it's no longer needed

        heightTextures = new RenderTexture[ChunkGlobals.lodCount];

        for (int i = 0; i < ChunkGlobals.lodCount; i++)
        {
            int textureSize = Math.Max(meshSpaceChunkSize >> i, 1);
            int threadGroups = textureSize;
            // print("Texture size: " + textureSize);
            // print("Thread groups: " + threadGroups);

            heightTextures[i] = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            heightTextures[i].Create();
            noiseGen.SetInt("lod", i);
            noiseGen.SetTexture(1, "outputTexture", heightTextures[i]);
            noiseGen.Dispatch(1, threadGroups, threadGroups, 1);
        }
    }

    private void SetShaderParameters(float worldSpaceChunkSize, int meshSpaceChunkSize, float worldSpaceChunkCenterX, float worldSpaceChunkCenterY)
    {
        noiseGen.SetFloat("worldSpaceChunkSize", worldSpaceChunkSize);
        noiseGen.SetInt("meshLengthInVertices", meshSpaceChunkSize + 1);
        noiseGen.SetFloat("worldSpaceChunkCenterX", worldSpaceChunkCenterX);
        noiseGen.SetFloat("worldSpaceChunkCenterY", worldSpaceChunkCenterY);
        noiseGen.SetInt("seed", settings.Seed);
        noiseGen.SetFloat("frequency", settings.Scale);
        noiseGen.SetInt("octaves", settings.Octaves);
        noiseGen.SetFloat("lacunarity", settings.Lacunarity);
        noiseGen.SetFloat("persistence", settings.Persistence);

        noiseGen.SetTexture(1, "colorLookupTexture", TextureGen.lookupTexture);
    }

    public float[] GetHeightData()
    {
        return heightsData;
    }

    public Texture[] GetTextureData()
    {
        return heightTextures;
    }

    private void OnDestroy()
    {
        heightsBuffer?.Release();
        if (heightTextures != null)
        {
            foreach (RenderTexture texture in heightTextures)
            {
                texture.Release();
            }
        }
    }
}
