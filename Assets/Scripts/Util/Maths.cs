


using UnityEngine;

public class Maths
{
    // note: MOD != REM (%), e.g. -3 MOD 16 -> 13 while -3 REM 16 = 3
    public static float Mod(float v, float n)
    {
        float f = v % n;
        return f < 0 ? f + n : f;
        // or return ((v % n) + n) % n;
        // or dummy return floor(v / n) * n;
    }

    public static float Floor(float v, float n)
    {
        float f = (int)(v / n) * n;
        return v < 0 ? f - n : f;
    }

    // VectorUtil

    public static Vector3 Vector3(float[] arr, int idx)
    {
        return new Vector3(arr[idx], arr[idx+1], arr[idx+2]);
    }
    public static Vector2 Vector2(float[] arr, int idx)
    {
        return new Vector2(arr[idx], arr[idx + 1]);
    }

    public static Vector3 Floor(Vector3 v)
    {
        return new((int)v.x, (int)v.y, (int)v.z);
    }

    // CollectionsUtil
    public static int[] Sequence(int n, int begin = 0)
    {
        int[] seq = new int[n];

        for (int i = 0; i < n; ++i)
        {
            seq[i] = i + begin;
        }
        return seq;
    }
}