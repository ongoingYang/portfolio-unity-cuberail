using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameCookInterface;

public abstract class TriggerSensor : MonoBehaviour {

    protected BodyMover bodyMover;
    protected Transform bodyTransform;

    public SensorType sensorType;
    
    public bool isOnTrigger;
    public float passTime = 0.7f;
    public float turnTime = 0.8f;

    public bool IsOntrigger
    {
        get
        {
            return isOnTrigger;
        }

        set
        {
            isOnTrigger = value;
            if (isOnTrigger) Invoke("ReadyDetecting", 0.9f);
        }
    }
    void ReadyDetecting()
    {
        IsOntrigger = false;
    }
    private void Awake(){
        bodyMover = GetComponentInParent<BodyMover>();
        bodyTransform = bodyMover.GetComponent<Transform>();
        
        if (bodyMover == null || bodyTransform == null)
        {
            Debug.LogWarning("sensorType: "+ sensorType + "의 Body Missing Error !! : body 객체 참조 안됨");
        }/**else   Debug.Log("바디 객체 참조 완료"); **/
    }
    protected IEnumerator RoutineSetPosition(Vector3 targetPosition, SFXSoundType effectType) // 충돌 타겟의 중심 좌표까지 이동
    {
        bool isOnEffect = false;
        Vector3 start = bodyTransform.position;
        Vector3 end = targetPosition;
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            if (elapsedTime > 0.5f && isOnEffect == false) 
            {
                AudioManager.Instance.PlaySFXSound(effectType);
                isOnEffect = true;
            }
            elapsedTime += Time.deltaTime / passTime;
            bodyTransform.position = Vector3.Lerp(start, end, elapsedTime.Interpolation(SmoothType.SmootherStep));
            yield return null;
        }
        
    }

    protected IEnumerator RoutineSetRotation(Quaternion targetRotation) // 충돌 타겟이 지시하는 방향으로 회전
    {
        Quaternion from = bodyTransform.rotation;
        Quaternion to = targetRotation;
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / turnTime;
            bodyTransform.rotation = Quaternion.Lerp(from, to, elapsedTime.Interpolation(SmoothType.SmootherStep));
            yield return null;
        }
        bodyMover.IsMoveActive = true;
    }

    

    #region Legacy Func
    /**  대상까지 근접 거리 체크 계산 람다식**/
    //public Func<float, Vector3, Vector3, bool> CheckDistance = (closeDist, from, to) => {
    //    if ((from - to).sqrMagnitude < closeDist * closeDist) return true;
    //    else return false;
    //};

    public Func<Vector3, Vector3, bool> CheckDistance = (mover, target) =>
    {
        if (mover.magnitude - target.magnitude >= 0) return true;
        else return false;
    };
    /**  transform.translate 로 움직이는 객체의 다른 대상과의 거리체크는 
     * 움직임이 빨라지면 정교한 계산에서 오류가 있을 수 있으므로 실제 사용하지는 말자 
     * 정확하게는 업데이트의 한 프레임에서 이동하는 거리가 collider의 크기를 벗어나지 않더라도
     * CheckDistance의 float value의 minimal한 수치를 넘어가면 감지하지 못한다. **/

    IEnumerator RoutineApproachToDistance(Transform from, Vector3 to){

        // 사전 처리
        yield return new WaitUntil(() => CheckDistance(from.position, to));
        // 사후 처리
    }
    //Func<float, float> SmootherStep = t => t * t * (3 - 2 * t);                             //SmootherStep Function
    //Func<float, float> SoSmootherStep = t => t = t * t * t * (t * (6f * t - 15f) + 10f);    //So smooth
    //Func<float, float> EaseOutWithSin = t => Mathf.Sin(t * Mathf.PI * 0.5f);                //ease out with sin
    //Func<float, float> EaseInWithCos = t => 1f - Mathf.Cos(t * Mathf.PI * 0.5f);            //ease in with cos
    #endregion


}
