using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class DbgGridView : MonoBehaviour
{
    public bool m_DbgDrawViewerChunkBound = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        float3 base_p = math.floor(transform.position);
        float3 size = 1;

        World world = Ethertia.GetWorld();
        if (world == null)
            return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(base_p + 0.5f, size);

        foreach (float3 v in ChunkMesher.SN_VERT)
        {
            float3 p = base_p + v;
            Cell c = world.GetCell(p);

            if (c.Value > 0)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawSphere(p, 0.05f);
            }

            Gizmos.color = Color.white;
            Handles.Label(p + 0.1f, c.Value.ToString());
        }

        if (m_DbgDrawViewerChunkBound)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube((float3)Chunk.ChunkPos(base_p) + 8.0f,
                                Vector3.one * 16.0f);
        }
    }
}
