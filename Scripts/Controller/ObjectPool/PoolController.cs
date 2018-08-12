using System.Collections;
using UnityEngine;
using GameCookInterface;

public class PoolController : MonoBehaviour {

    #region make instance
    public static PoolController Instance;
    private void MakeInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Awake()
    {
        MakeInstance();
    }
    #endregion

    public float lerpTime = 0.8f;
    public Transform[,] moverSpawnPoint;


    private void Start()
    {
        moverSpawnPoint = new Transform[3, GameDesignManager.blockLevelSize];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < GameDesignManager.blockLevelSize; j++)
            {
                moverSpawnPoint[i, j] = LevelDesigner.Instance.rotatorArray[i, j].transform;
            }
        }
    }

    public void OnSpawnMover(ObjecTPoolType type, Identifier identifier)
    {
        int axis = Random.Range(0, 3);
        int index = Random.Range(0, GameDesignManager.blockLevelSize);

        GameObject mover = ObjectPoolManager.Instance.FindPoolObjectItem(type);
        StartCoroutine(RoutineSetPosition(mover.transform, Vector3.zero, SetTargetPosition(axis, index)));
        StartCoroutine(RoutineSetRotation(mover.transform, Quaternion.identity, SetTargetRotation(axis, index, identifier)));

    }
    public void OnSpawnMover(DirectionAxis axis, ObjecTPoolType type, Identifier identifier)
    {
        
        int index = Random.Range(0, GameDesignManager.blockLevelSize);

        GameObject mover = ObjectPoolManager.Instance.FindPoolObjectItem(type);
        StartCoroutine(RoutineSetPosition(mover.transform, Vector3.zero, SetTargetPosition(axis.AxisToIndex(), index)));
        StartCoroutine(RoutineSetRotation(mover.transform, Quaternion.identity, SetTargetRotation(axis.AxisToIndex(), index, identifier)));

    }

    public void OnClickVertex(ObjecTPoolType type, DirectionAxis axis, Vector3 start)
    {
        int index = Random.Range(0, GameDesignManager.blockLevelSize);
        int axisConversion = (axis.AxisToIndex() + 2) % 3;

        GameObject mover = ObjectPoolManager.Instance.FindPoolObjectItem(type);

        Vector3 end = SetTargetPosition(axisConversion, index);
        AudioManager.Instance.PlaySFXSound(SFXSoundType.MoverSpawn);
        StartCoroutine(RoutineSetParabola(mover.transform, start, end));
        StartCoroutine(RoutineSetRotation(mover.transform, Quaternion.identity, SetTargetRotation(axisConversion, index, Identifier.Positive)));
    }

    #region Spawn Interpolation Lerp Routine

    IEnumerator RoutineSetPosition(Transform target, Vector3 prev, Vector3 next)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / lerpTime;

            target.position = Vector3.Lerp(prev, next, elapsedTime.Interpolation(SmoothType.SmootherStep));
            yield return null;
        }
        target.position = next;
    }
    IEnumerator RoutineSetRotation(Transform target, Quaternion prev, Quaternion next) 
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / lerpTime;
            target.rotation = Quaternion.Lerp(prev, next, elapsedTime.Interpolation(SmoothType.SmootherStep));
            yield return null;
        }
        target.transform.rotation = next;
    }
    IEnumerator RoutineSetParabola(Transform target, Vector3 prev, Vector3 next)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= 1.0f){
            elapsedTime += Time.deltaTime / lerpTime;
            Vector3 mid = Vector3.Lerp(prev, next, elapsedTime.Interpolation(SmoothType.Exponential));
            target.position = new Vector3(mid.x, elapsedTime.Interpolation(SmoothType.Exponential).PalabolaMethod(2.5f), mid.z);
            yield return null;
        }
        target.position = next;
    }
    private Vector3 SetTargetPosition(int axis, int index)
    {
        Vector3 position = moverSpawnPoint[axis, index].position;
        return position;
    }
    private Quaternion SetTargetRotation(int axis, int index, Identifier identifier)
    {
        switch (identifier)
        {
            case Identifier.Positive:
                return moverSpawnPoint[axis, index].GetChild(0).rotation;
            default:
                return moverSpawnPoint[axis, index].GetChild(1).rotation;
        }
    }

    #endregion

}
