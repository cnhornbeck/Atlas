using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int id = -1;
    public Vector2 position { get; set; } = Vector2.zero;
    public Mesh[] lodMeshList { get; set; } = new Mesh[ChunkConstants.lodNum];
    public Texture2D[] lodTextureList { get; set; } = new Texture2D[ChunkConstants.lodNum];
    public MeshCollider hitbox { get; set; } = new();
    public enum ChunkState
    {
        NotGenerated,
        Generated,
        Generating,
        NeedsUpdating
    }

    public ChunkState chunkState { get; set; } = ChunkState.NotGenerated;

    public Chunk()
    {
        // Create Constructor
    }

}
