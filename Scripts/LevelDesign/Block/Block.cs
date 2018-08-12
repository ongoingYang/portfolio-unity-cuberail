using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameCookInterface;


public abstract class Block : MonoBehaviour {

    public DirectionAxis parentDirection;
    protected Func<int, float> IndexToPositive = x => 1 + x;
    protected Func<int, float> IndexToNegative = x => -1 * (1 + x);
    protected float landingTime = 0.7f;
    protected float landingheight;
    public virtual void Initialize(int origin, int j, int k)
    {
        landingheight = GameDesignManager.blockLevelSize * 2;
        this.transform.position = new Vector3(IndexToNegative(j), 0, IndexToPositive(k)); // 월드 공간에 배치
        SetParentOrigin(origin); // 부모 좌표 인덱스 할당
    }

    protected void SetParentOrigin(int origin)
    {
        switch (origin)
        {
            case 0:
                parentDirection = DirectionAxis.xDirection;
                this.transform.SetParent(LevelDesigner.Instance.xDirectionAxis);
                break;
            case 1:
                parentDirection = DirectionAxis.yDirection;
                this.transform.SetParent(LevelDesigner.Instance.yDirectionAxis);
                break;
            case 2:
                parentDirection = DirectionAxis.zDirection;
                this.transform.SetParent(LevelDesigner.Instance.zDirectionAxis);
                break;
        }
    }
    protected IEnumerator RoutineShowUp(Transform transform, SFXSoundType effectType , float height, float time)
    {
        float from = transform.localPosition.y + height;
        float to = transform.localPosition.y;

        float elapsedTime = 0f;
        while (elapsedTime <= 1)
        {
            elapsedTime += Time.deltaTime / time;
            float h = Mathf.Lerp(from, to, elapsedTime.Interpolation(SmoothType.Exponential));
            transform.localPosition = new Vector3(transform.localPosition.x, h, transform.localPosition.z);
            yield return null;
        }
        AudioManager.Instance.PlaySFXSound(effectType);
    }
}
