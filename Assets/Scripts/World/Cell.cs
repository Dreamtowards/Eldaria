using System;
using UnityEngine;

public struct Cell
{

    public static Cell Nil = new Cell();
    public static readonly Vector3 InvalidFP = new(Mathf.Infinity, 0, 0);

    // for SDF
    public float Value;

    public int MtlId;

    public Vector3 FeaturePoint;
    public Vector3 Normal;

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
