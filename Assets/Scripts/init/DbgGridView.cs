using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class DbgGridView : MonoBehaviour
{
    public bool m_DbgDrawViewerChunkBound = false;
    public bool m_DbgRepositionToCursor = false;
    public bool m_DbgShowTextNormalValue = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DbgRepositionToCursor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_DbgRepositionToCursor = false;
            }

            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView != null)
            {
                Camera camera = sceneView.camera;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;
                    transform.position = objectHit.position;
                }
            }
        }
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

        Cell cell = world.GetCell(base_p);
        if (cell.IsSolid())
        {
            if (Maths.IsFinite(cell.FeaturePoint))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(base_p + cell.FeaturePoint, 0.05f);
            }
            if (Maths.IsFinite(cell.Normal))
            {
                float3 from = base_p + cell.FeaturePoint;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(from, from+cell.Normal);
            }
        }
        if (m_DbgShowTextNormalValue)
        {
            string mtl = "nil";
            if (cell.Mtl != null)
            {
                mtl = $"{cell.Mtl.RegistryId} {cell.Mtl.Id}";
            }
            Handles.Label(Maths.IsFinite(cell.FeaturePoint) ? base_p + cell.FeaturePoint + 0.1f : base_p + 0.5f,
                $"\nP: {cell.FeaturePoint}\nN: {cell.Normal}\nMtl: {mtl}");
        }

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
            Gizmos.color = Color.gray * 0.5f;
            Gizmos.DrawCube((float3)Chunk.ChunkPos(base_p) + 8.0f,
                                Vector3.one * 16.0f);
        }
    }
}
