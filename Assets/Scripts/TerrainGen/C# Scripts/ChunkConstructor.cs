using UnityEngine;
using Unity.Collections;


public class ChunkConstructor
{
    private JobData<Vector3> noiseJobData;
    private JobData<Color> textureJobData;
    private NativeArray<Vector3> vertexArray;
    private Texture2D textureData;
    private Mesh[] meshData;
    private Vector2 worldSpacePosition;

    public void StartNoiseJob(Vector2 worldSpacePosition)
    {
        vertexArray = new NativeArray<Vector3>((ChunkGlobals.meshSpaceChunkSize + 1) * (ChunkGlobals.meshSpaceChunkSize + 1), Allocator.TempJob);
        noiseJobData = NoiseGen.ScheduleNoiseGenJob(worldSpacePosition);

        this.worldSpacePosition = worldSpacePosition;
    }

    public void StartTextureJob()
    {
        textureData = new Texture2D(ChunkGlobals.meshSpaceChunkSize, ChunkGlobals.meshSpaceChunkSize);
        textureJobData = TextureGen.ScheduleTextureGenJob(vertexArray);
    }

    public void CreateMesh()
    {
        meshData = MeshGen.GetMeshes(vertexArray);
        vertexArray.Dispose();
    }

    public void CompleteNoiseJob()
    {
        vertexArray = NoiseGen.CompleteNoiseGenJob(noiseJobData);
    }

    public void CompleteTextureJob()
    {
        textureData = TextureGen.CompleteTextureGenJob(textureJobData);
    }

    public NativeArray<Vector3> GetVertexArray()
    {
        return vertexArray;
    }

    public Texture2D GetTextureData()
    {
        return textureData;
    }

    public Mesh[] GetMeshData()
    {
        return meshData;
    }

    public bool GetNoiseJobStatus()
    {
        return noiseJobData.jobHandle.IsCompleted;
    }

    public bool GetTextureJobStatus()
    {
        return textureJobData.jobHandle.IsCompleted;
    }

    public Vector2 GetWorldSpacePosition()
    {
        return worldSpacePosition;
    }

    // public static ChunkData ConstructChunk(Vector2 worldSpacePosition)
    // {
    //     NativeArray<Vector3> vertexArray = AllocateVertexArray();
    //     Texture2D textureData = AllocateTextureData();

    //     RegisterNoiseGenerationJob(worldSpacePosition, ref vertexArray, ref textureData);

    //     Mesh[] meshData = MeshGen.GetMeshes(vertexArray);
    //     Vector3 worldSpaceChunkCenter = GetWorldSpaceChunkCenter(vertexArray, worldSpacePosition);

    //     vertexArray.Dispose();

    //     return new ChunkData(worldSpaceChunkCenter, meshData, textureData);
    // }

    // private static NativeArray<Vector3> AllocateVertexArray()
    // {
    //     return new NativeArray<Vector3>((ChunkGlobals.meshSpaceChunkSize + 1) * (ChunkGlobals.meshSpaceChunkSize + 1), Allocator.TempJob);
    // }

    // private static Texture2D AllocateTextureData()
    // {
    //     return new Texture2D(ChunkGlobals.meshSpaceChunkSize, ChunkGlobals.meshSpaceChunkSize);
    // }

    // private static void RegisterNoiseGenerationJob(Vector2 worldSpacePosition, ref NativeArray<Vector3> vertexArray, ref Texture2D textureData)
    // {
    //     JobManager.RegisterJob(
    //         NoiseGen.ScheduleNoiseGenJob,
    //         NoiseGen.CompleteNoiseGenJob,
    //         worldSpacePosition,
    //         ref vertexArray,
    //         (scheduleJobMethod, completeJobMethod, input, ref NativeArray<Vector3> noiseOutput) =>
    //         {
    //             RegisterTextureGenerationJob(noiseOutput, ref textureData);
    //         }
    //     );
    // }

    // private static void RegisterTextureGenerationJob(NativeArray<Vector3> vertexArray, ref Texture2D textureData)
    // {
    //     JobManager.RegisterJob(
    //         TextureGen.ScheduleTextureGenJob,
    //         TextureGen.CompleteTextureGenJob,
    //         vertexArray,
    //         ref textureData
    //     );
    // }

    // private static Vector3 GetWorldSpaceChunkCenter(NativeArray<Vector3> vertexArray, Vector2 worldSpacePosition)
    // {
    //     int meshLengthInVertices = ChunkGlobals.meshSpaceChunkSize + 1;

    //     float summedHeight = (
    //         vertexArray[0].y +
    //         vertexArray[meshLengthInVertices - 1].y +
    //         vertexArray[(meshLengthInVertices * meshLengthInVertices) - 1].y +
    //         vertexArray[(meshLengthInVertices * meshLengthInVertices) - meshLengthInVertices].y
    //     );

    //     float centerHeight = summedHeight / 4;

    //     return new Vector3(worldSpacePosition.x, centerHeight, worldSpacePosition.y);
    // }
}
