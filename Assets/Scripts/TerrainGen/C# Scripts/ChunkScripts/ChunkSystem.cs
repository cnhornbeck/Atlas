using UnityEngine;
using Unity.Mathematics;

public class ChunkSystem : MonoBehaviour
{
    void Start()
    {
        ChunkConstructorManager.ParentTransform = transform;
    }

    void Update()
    {
        float3 cameraPos = Camera.main.transform.position;

        ChunkVisibilityManager.UpdateChunkVisibility(cameraPos);

        ChunkConstructorManager.HandleChunkGeneration();
    }
}

