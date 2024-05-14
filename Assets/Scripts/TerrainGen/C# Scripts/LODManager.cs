using UnityEngine;

public class LODManager : MonoBehaviour
{
    Transform player;
    [SerializeField] int currentLOD;
    int meshLOD;
    MeshFilter meshFilter;
    public Mesh[] meshes;
    public Vector3 worldSpaceChunkCenter;

    void Start()
    {
        player = Camera.main.transform;
        meshFilter = GetComponent<MeshFilter>();
        meshLOD = -1;
    }

    void Update()
    {
        if (meshes == null)
        {
            return;
        }
        CalculateLOD();
        SetLOD();
    }

    void SetLOD()
    {
        if (meshLOD != currentLOD)
        {
            meshFilter.mesh = meshes[currentLOD];
            meshLOD = currentLOD;
        }
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
}
