using UnityEngine;
using GameCookInterface;
[CreateAssetMenu(fileName = "PoolContainer", menuName = "ObjectPool/PoolContainer", order = 2)]
public class PoolContainer : PoolItem {

    [Header("Pool Type")]
    public ObjecTPoolType poolType;
}
