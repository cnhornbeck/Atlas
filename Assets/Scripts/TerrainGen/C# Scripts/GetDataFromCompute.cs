// using UnityEngine;
// using System.Collections;

// public class GetDataFromCompute : MonoBehaviour
// {
//     [SerializeField] private ComputeShader noiseGen;

//     private ComputeBuffer vertexBuffer;
//     private Vector3[] vertexData;
//     private RenderTexture heightTexture;

//     private static NoiseSettings settings = new();

//     void Start()
//     {
//         SetStaticShaderParameters();
//     }

//     public void CalculateMeshData(Vector2 worldSpacePosition)
//     {
//         int meshSpaceChunkSize = ChunkGlobals.meshSpaceChunkSize;
//         int vertexNum = (meshSpaceChunkSize + 1) * (meshSpaceChunkSize + 1);
//         vertexData = new Vector3[vertexNum];

//         vertexBuffer = new ComputeBuffer(vertexNum, sizeof(float) * 3);
//         vertexBuffer.SetData(vertexData);

//         noiseGen.SetBuffer(0, "vertices", vertexBuffer);
//         noiseGen.SetBuffer(1, "vertices", vertexBuffer);

//         noiseGen.SetFloat("worldSpaceChunkCenterX", worldSpacePosition.x);
//         noiseGen.SetFloat("worldSpaceChunkCenterZ", worldSpacePosition.y);

//         noiseGen.Dispatch(0, Mathf.CeilToInt(vertexNum / 1024f), 1, 1); // Dispatch GetVertices

//         vertexBuffer.GetData(vertexData);

//         int textureSize = meshSpaceChunkSize;
//         heightTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat)
//         {
//             enableRandomWrite = true,
//             filterMode = FilterMode.Point,
//             wrapMode = TextureWrapMode.Clamp
//         };
//         heightTexture.Create();
//         noiseGen.SetTexture(1, "outputTexture", heightTexture);
//         noiseGen.Dispatch(1, Mathf.CeilToInt((float)textureSize / 8), Mathf.CeilToInt((float)textureSize / 8), 1);

//         vertexBuffer.Release(); // Release the buffer
//     }

//     public IEnumerator CalculateMeshDataCoroutine(Vector2 worldSpacePosition)
//     {
//         int meshSpaceChunkSize = ChunkGlobals.meshSpaceChunkSize;
//         int vertexNum = (meshSpaceChunkSize + 1) * (meshSpaceChunkSize + 1);
//         vertexData = new Vector3[vertexNum];

//         vertexBuffer = new ComputeBuffer(vertexNum, sizeof(float) * 3);
//         vertexBuffer.SetData(vertexData);

//         noiseGen.SetBuffer(0, "vertices", vertexBuffer);
//         noiseGen.SetBuffer(1, "vertices", vertexBuffer);

//         noiseGen.SetFloat("worldSpaceChunkCenterX", worldSpacePosition.x);
//         noiseGen.SetFloat("worldSpaceChunkCenterZ", worldSpacePosition.y);

//         // Dispatch the compute shader for vertices
//         noiseGen.Dispatch(0, Mathf.CeilToInt(vertexNum / 1024f), 1, 1);

//         // Wait for a frame after dispatching the compute shader to give the GPU time to process
//         yield return new WaitForSeconds(0.01f);

//         // Optionally, wait for more frames if you expect the operation to take longer
//         // yield return new WaitForSeconds(waitTime);

//         vertexBuffer.GetData(vertexData);

//         int textureSize = meshSpaceChunkSize;
//         heightTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat)
//         {
//             enableRandomWrite = true,
//             filterMode = FilterMode.Point,
//             wrapMode = TextureWrapMode.Clamp
//         };
//         heightTexture.Create();

//         noiseGen.SetTexture(1, "outputTexture", heightTexture);

//         // Dispatch the compute shader for textures
//         noiseGen.Dispatch(1, Mathf.CeilToInt((float)textureSize / 8), Mathf.CeilToInt((float)textureSize / 8), 1);

//         // Again, wait for the GPU to finish processing the texture
//         yield return new WaitForSeconds(0.01f);

//         // Cleanup
//         vertexBuffer.Release();
//     }

//     private void SetStaticShaderParameters()
//     {

//         // Set float values
//         noiseGen.SetFloat("worldSpaceChunkSize", ChunkGlobals.worldSpaceChunkSize);
//         noiseGen.SetFloat("frequency", settings.Scale);
//         noiseGen.SetFloat("lacunarity", settings.Lacunarity);
//         noiseGen.SetFloat("persistence", settings.Persistence);

//         // Set int values
//         noiseGen.SetInt("meshLengthInVertices", ChunkGlobals.meshSpaceChunkSize + 1);
//         noiseGen.SetInt("heightMultiplier", ChunkGlobals.heightMultiplier);
//         noiseGen.SetInt("seed", settings.Seed);
//         noiseGen.SetInt("octaves", settings.Octaves);

//         Texture colorLookupTexture = TextureGen.lookupTexture;

//         noiseGen.SetTexture(1, "colorLookupTexture", colorLookupTexture);
//         noiseGen.SetInt("textureWidth", colorLookupTexture.width);

//         // Debug.Log("Set static shader parameters");
//     }

//     public Vector3[] GetHeightData()
//     {
//         return vertexData;
//     }

//     public Texture GetTextureData()
//     {
//         return heightTexture;
//     }
// }
