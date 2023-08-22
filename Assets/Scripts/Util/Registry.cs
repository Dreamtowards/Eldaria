

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Registry<T> : IEnumerable<KeyValuePair<string, T>>
{
    private SortedDictionary<string, T> m_Map = new SortedDictionary<string, T>();

    private List<T> m_Id2Entry = new List<T>(); 

    public void Register(string id, T entry)
    {
        if (Has(id))
            throw new System.Exception("Failed register, key "+id+" already exists");
        m_Id2Entry.Clear();  // invalidate.
    
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

    public int Count { get { return m_Map.Count; } }

    public void BuildNumberIds(Action<T, int> assign)
    {
        m_Id2Entry.Clear();
        m_Id2Entry.Capacity = m_Map.Count;
        int id = 0;
        foreach (var kv in m_Map)
        {
            m_Id2Entry.Add(kv.Value);
            assign(kv.Value, id);
            ++id;
        }
    }

    public string DbgPrintEntries()
    {
        string s = "";
        foreach (var kv in m_Map)
        {
            s += kv.Key + ", ";
        }
        return s;
    }

    public T Get(int numId)
    {
        return m_Id2Entry[numId];
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