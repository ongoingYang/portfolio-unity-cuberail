using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameCookInterface;
public class SpawnPoolList : MonoBehaviour {

    [Header("Pool Object Source")]
    [Space(2)]
    [SerializeField] List<PoolContainer> containerList = new List<PoolContainer>();

    private void Awake()
    {
        InitIndexOrder();
    }

    private void InitIndexOrder()
    {
        foreach (PoolContainer container in containerList){
            container.PoolIndex = container.poolType.PoolTypeToIndex();
        }
        int index = 0;
        List<PoolContainer> temp = containerList;
        foreach (PoolContainer item in temp.OrderBy(t => t.PoolIndex)){
            containerList[index++] = item;
        }
    }
    public void InstantiatePoolItem()
    {
        foreach (PoolContainer container in containerList)
        {
            if (container != null)
            {
                PoolContainer poolContainer = Instantiate(container) as PoolContainer;
                ObjectPoolManager.Instance.poolContainedList.Add(poolContainer);
                ObjectPoolManager.Instance.SetObjectPool(
                    poolContainer.PoolList,
                    poolContainer.poolSize,
                    poolContainer.itemPrefab,
                    this.transform);
            }
            else Debug.LogWarning("containerPrefab missing : " + container);
        }
    }
}
