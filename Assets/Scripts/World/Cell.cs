using System;


public struct Cell
{
    // for SDF
    public float SignedDistanceValue;

    public int BlockId;

    public static Cell Nil = new Cell();
}
