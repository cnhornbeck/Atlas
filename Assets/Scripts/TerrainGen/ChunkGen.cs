using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ChunkGen
{

    Hashtable terrainChunksHash = new();
    List<GameObject> terrainChunksList = new();


    void GenerateChunk()
    {

    }

    // public void GenerateTerrain(int chunkSize, float xAdjustment, float yAdjustment, int seed, float heightMultiplier, float scale, float lacunarity, float persistence, int octaves, float vertSep, float waterHeight, GameObject player, TextureGen texGenerator, GameObject parent, List<TerrainLevel> colorList)
    // {

    //     MeshGen meshGenerator = new();

    //     int index = 0;


    //     float xOffset;
    //     float yOffset;

    //     Vector3 playerPos = player.transform.position;

    //     Vector3 thisChunk = new(Mathf.Round(playerPos.x / ((chunkSize - 1) * vertSep)), 0, Mathf.Round(playerPos.z / ((chunkSize - 1) * vertSep)));
    //     Vector2Int chunkKey = new((int)thisChunk.x, (int)thisChunk.z);

    //     // foreach (GameObject chunk in terrainChunksList)
    //     // {
    //     //   xOffset = xAdjustment + scale * chunk.transform.position.x / (width * vertSep);
    //     //   yOffset = yAdjustment + scale * chunk.transform.position.z / (height * vertSep);

    //     //   chunk.GetComponent<MeshFilter>().sharedMesh = meshGenerator.GenerateMesh(width, height, xOffset, yOffset, seed, heightMultiplier, scale, lacunarity, persistence, octaves, vertSep, waterHeight);
    //     //   Texture2D terrainTexture = texGenerator.TexGen(width, height, xOffset, yOffset, scale, lacunarity, persistence, octaves, seed);
    //     //   chunk.GetComponent<MeshRenderer>().material.mainTexture = terrainTexture;
    //     //   CalcUVs(width, height, chunk);
    //     //   terrainTexture.filterMode = FilterMode.Point;
    //     //   terrainTexture.Apply();

    //     // }

    //     if (!terrainChunksHash.ContainsKey(chunkKey))
    //     {

    //         xOffset = xAdjustment + scale * thisChunk.x - (thisChunk.x * scale / chunkSize);
    //         yOffset = yAdjustment + scale * thisChunk.z - (thisChunk.z * scale / chunkSize);


    //         string chunkName = "Terrain Chunk: (" + (int)thisChunk.x + ", " + (int)thisChunk.z + ")";
    //         GameObject terrainChunk = new(chunkName);
    //         terrainChunk.transform.parent = parent.transform;

    //         // Add the newly generated terrain chunk to the "terrainChunks" Hashtable with a value of index and to a list
    //         terrainChunksHash.Add(chunkKey, index);
    //         terrainChunksList.Add(terrainChunk);

    //         // Set the position of the terrain chunk object
    //         terrainChunk.transform.position = new Vector3(thisChunk.x * (chunkSize - 1) * vertSep, 0, thisChunk.z * (chunkSize - 1) * vertSep);


    //         // Debug.Log(xOffset);
    //         // Debug.Log(yOffset);

    //         // Generate a mesh for the terrain with the given parameters using the "meshGenerator" object
    //         Mesh terrainMesh = meshGenerator.GenerateMesh(chunkSize, xOffset, yOffset, seed, heightMultiplier, scale, lacunarity, persistence, octaves, vertSep, waterHeight);

    //         // Add MeshFilter and MeshRenderer components to the "terrain" GameObject
    //         terrainChunk.AddComponent<MeshFilter>();
    //         terrainChunk.AddComponent<MeshRenderer>();

    //         // Set the sharedMesh property of the MeshFilter component to the generated terrain mesh
    //         terrainChunk.GetComponent<MeshFilter>().sharedMesh = terrainMesh;

    //         // Generate a texture for the terrain with the given parameters
    //         Texture2D terrainTexture = texGenerator.TextureGenerator(chunkSize, xOffset, yOffset, scale, lacunarity, persistence, octaves, seed, colorList);

    //         // terrainTexture.SetPixel(0, 0, Color.black);

    //         // Calculate the UV coordinates for the mesh
    //         CalcUVs(chunkSize, chunkSize, terrainChunk);

    //         // Set the main texture of the shared material of the mesh renderer to the generated texture
    //         terrainChunk.GetComponent<MeshRenderer>().material.mainTexture = terrainTexture;
    //         terrainChunk.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", 0.0f);

    //         // Set the filter mode to point to prevent texture blurring when the texture is scaled up
    //         terrainTexture.filterMode = FilterMode.Point;

    //         // Set the wrap mode to clamp to prevent the texture from wrapping around the mesh
    //         terrainTexture.wrapMode = TextureWrapMode.Clamp;



    //         // Apply the texture to make sure that the changes to the texture are actually shown on the mesh
    //         terrainTexture.Apply();


    //         index++;
    //     }

    // }




    public void CalcUVs(int width, int height, GameObject terrain)
    {
        Mesh terrainMesh = terrain.GetComponent<MeshFilter>().sharedMesh;
        // Generate the UV coordinates for the mesh
        Vector2[] uvs = new Vector2[width * height];
        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                uvs[index] = new Vector2((float)x / (float)width, (float)y / (float)height);
                index++;
            }
        }
        // Set the UV coordinates of the mesh
        terrainMesh.SetUVs(0, uvs);
    }

    public void ClearCachedChunks()
    {
        Debug.Log("Clearing cached chunks");
        foreach (GameObject item in terrainChunksList)
        {
            GameObject.Destroy(item);
        }
        terrainChunksHash.Clear();
        terrainChunksList.Clear();
    }


}
