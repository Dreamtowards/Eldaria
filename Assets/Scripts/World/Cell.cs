using System;
using UnityEngine;
using Unity.Mathematics;

public struct Cell
{
    // Illegal Cell. 
    public static Cell Nil = new();

    // SDF (signed distance field) value.
    // Positives == Inside the volume, Negatives == Outside the volume
    public float Value;

    public int MtlId;

    public float3 FeaturePoint;  // Inf: Cache Invalidated, NaN: Illegal Cell (Access out of range)
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

    public void Invalidate()
    {
        Value = 0;
        MtlId = 0;
        FeaturePoint = float.PositiveInfinity;
    }

    public bool InvalidFP()
    {
        return float.IsInfinity(FeaturePoint.x);  // FeaturePoint.x == float.PositiveInfinity
    }

    public bool IsNil()
    {
        return float.IsNaN(Value);
    }

}
