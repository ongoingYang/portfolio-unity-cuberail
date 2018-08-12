using System.Collections.Generic;
using UnityEngine;

public abstract class PoolItem : ScriptableObject
{
    private List<GameObject> poolList = new List<GameObject>();
    [Header("SpawnPool GameObject Prefab")]
    [Space]
    public GameObject itemPrefab;
    public int poolSize;

    public List<GameObject> PoolList
    {
        get{ return poolList; }
    }
    private int poolIndex;
    public int PoolIndex
    {
        get{
            return poolIndex;
        }

        set{
            poolIndex = value;
        }
    }

    
}
