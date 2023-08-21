


using UnityEngine;
using Unity.Mathematics;

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
        return Mathf.Floor(v / n) * n;
        //float f = (int)(v / n) * n;
        //return v < 0 ? f - n : f;
    }

    // VectorUtil

    public static float3 vec3(float[] arr, int idx)
    {
        return new(arr[idx], arr[idx+1], arr[idx+2]);
    }
    public static float2 vec2(float[] arr, int idx)
    {
        return new(arr[idx], arr[idx + 1]);
    }

    public static float3 Floor(float3 v)
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