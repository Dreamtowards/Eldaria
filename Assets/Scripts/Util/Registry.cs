

using System.Collections;
using System.Collections.Generic;
using System.Threading;

class Registry<T> : IEnumerable<KeyValuePair<string, T>>
{
    private Dictionary<string, T> m_Map = new Dictionary<string, T>();


    public void Register(string id, T entry)
    {
        if (Has(id))
            throw new System.Exception("Failed register, key "+id+" already exists");
    
        m_Map.Add(id, entry);
    }

    public bool Has(string id)
    {
        return m_Map.ContainsKey(id);
    }

    public T Get(string id)
    {
        return m_Map[id];
    }

    public int Count()
    {
        return m_Map.Count;
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, T>>)m_Map).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)m_Map).GetEnumerator();
    }
}