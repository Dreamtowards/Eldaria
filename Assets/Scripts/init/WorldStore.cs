

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldStoreData", menuName = "Eldara/WorldStoreDataMenu")]
class WorldStore : ScriptableObject
{
    public Dictionary<int3, int3> m_Chunks;

    public WorldStore()
    {
        Log.info("WorldStore New()");
    }
}