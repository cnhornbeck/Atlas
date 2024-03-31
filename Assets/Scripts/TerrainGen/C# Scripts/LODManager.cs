using UnityEngine;

public class LODManager : MonoBehaviour
{
    Transform player;
    [SerializeField] int currentLOD;
    int meshLOD;
    Mesh mesh;

    public Vector3 worldSpaceChunkCenter;

    void Start()
    {
        player = Camera.main.transform;
        mesh = GetComponent<MeshFilter>().mesh;
        meshLOD = -1;
        SetLOD();
    }

    void Update()
    {
        SetLOD();
    }

    void SetLOD()
    {
        CalculateLOD();
        UpdateTriangles();
    }

    void CalculateLOD()
    {
        float distance = Vector3.Distance(worldSpaceChunkCenter, player.position);
        currentLOD = Mathf.FloorToInt((distance / 15) - 1);
        if (currentLOD > ChunkGlobals.lodCount - 1)
        {
            currentLOD = ChunkGlobals.lodCount - 1;
        }
        if (currentLOD < 0)
        {
            currentLOD = 0;
        }
    }

    void UpdateTriangles()
    {
        if (meshLOD != currentLOD)
        {
            mesh.triangles = LODTriangleArrayGen.triangleArrays[currentLOD];
            meshLOD = currentLOD;
        }
    }

}
