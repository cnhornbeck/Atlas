using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // SetLOD();
    }

    void SetLOD(Chunk chunk, float distance)
    {
        // Set the LOD of the Chunk based on distance from player
        for (int i = 0; i < ChunkGlobals.lodDistArray.Length; i++)
        {
            if (distance < ChunkGlobals.lodDistArray[i])
            {
                // chunk.
            }
        }
    }
}
