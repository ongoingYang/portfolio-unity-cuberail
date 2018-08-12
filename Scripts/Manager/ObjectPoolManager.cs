using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCookInterface;
public class ObjectPoolManager : MonoBehaviour {

    #region 싱글턴 정의
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance
    { get { return _instance; } }
    private void MakeSingleton()
    {
        if (_instance != null) DestroyImmediate(gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Awake()
    {
        MakeSingleton();
    }
    #endregion
    [SerializeField] SpawnPoolList poolItemList;
    public List<PoolContainer> poolContainedList = new List<PoolContainer>();
    private void Start()
    {
        poolItemList.InstantiatePoolItem();
    }

    public GameObject FindPoolObjectItem(ObjecTPoolType type)
    {
        return GetFromObjectPool(poolContainedList[type.PoolTypeToIndex()].PoolList, type);
    }
    private GameObject GetFromObjectPool(List<GameObject> objectList, ObjecTPoolType type)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (!objectList[i].activeSelf)
            {
                objectList[i].SetActive(true);
                return objectList[i];
            }
        }
        GameObject newPoolItem = Instantiate(poolContainedList[type.PoolTypeToIndex()].itemPrefab);
        poolContainedList[type.PoolTypeToIndex()].PoolList.Add(newPoolItem);
        return newPoolItem;
    }
    public void SetObjectPool(List<GameObject> objectList, int size, GameObject prefab, Transform transform)
    {
        objectList.Clear();
        for (int i = 0; i < size; i++)
        {
            GameObject poolObject = Instantiate(prefab) as GameObject;
            poolObject.transform.SetParent(transform);
            objectList.Add(poolObject);
            poolObject.SetActive(false);
        }
    }
}

