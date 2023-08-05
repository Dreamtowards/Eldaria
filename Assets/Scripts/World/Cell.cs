using System;
using UnityEngine;
using Unity.Mathematics;

public struct Cell
{

    public static Cell Nil = new Cell();
    public static readonly float3 InvalidFP = new(Mathf.Infinity, 0, 0);

    // for SDF
    public float Value;

    public int MtlId;

    public float3 FeaturePoint;
    public float3 Normal;

    //public Cell()
    //{
    //    Value = 0;
    //    MtlId = 0;
    //    FeaturePoint = InvalidFP;
    //    Normal = new Vector3();
    //}

    public bool IsSolid()
    {
        return Value > 0;
    }


}
