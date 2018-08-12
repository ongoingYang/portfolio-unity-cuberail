using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameCookInterface;
public class PlaneBlock : Block {


    public override void Initialize(int origin, int j, int k)
    {
        base.Initialize(origin, j, k);
        thisTransform = GetComponent<Transform>();
        for (int i = 0; i < thisTransform.childCount; i++){
            thisTransform.GetChild(i).gameObject.SetActive(false);
        }
        thisTransform.GetChild(0).gameObject.SetActive(true);
        _ChildItemStackIndex.Push(0);
        CurrentItemName = PlaneChildItem.None;
        StartCoroutine(RoutineShowUp(thisTransform, SFXSoundType.BlockDrop, landingheight, landingTime));
    }
    [SerializeField]private PlaneChildItem currentItemName;
    public PlaneItemType category;
    public int planeGroupID;
 

    private Transform thisTransform;
    private Stack<int> _ChildItemStackIndex = new Stack<int>();


    public PlaneChildItem CurrentItemName
    {
        get {
            return currentItemName;
        }
        set {
            currentItemName = value;
            if (
                currentItemName == PlaneChildItem.Interation_Positive || currentItemName == PlaneChildItem.Interation_Negative)
            {
                category = PlaneItemType.Interaction;

            }
            else if (currentItemName == PlaneChildItem.Clockwise || currentItemName == PlaneChildItem.CounterClockwise ||
                currentItemName == PlaneChildItem.Slash || currentItemName == PlaneChildItem.BackSlash || currentItemName == PlaneChildItem.None)
            {
                category = PlaneItemType.Direction;
            }
            else if
                (currentItemName == PlaneChildItem.Coin)
                category = PlaneItemType.Consumption;
        }
    }

    public void SetActiveBlockItem(PlaneChildItem itemName)
    {
        if (_ChildItemStackIndex.Count >= 2)
            thisTransform.GetChild(_ChildItemStackIndex.Pop()).gameObject.SetActive(false);

        int index = itemName.BlockItemToIndex();
        thisTransform.GetChild(index).gameObject.SetActive(true);
        _ChildItemStackIndex.Push(index);
        CurrentItemName = itemName;
        StartCoroutine(RoutineShowUp(thisTransform.GetChild(index), EffectTypeByItem(itemName), 2.5f, 0.4f));
    }

    public void ActionByMover() {

        switch (category)
        {
            case PlaneItemType.Interaction:
                Action_Interaction();
                break;
            case PlaneItemType.Consumption:
                Action_Consumption(); // coin에 해당함
                break;
            default: // PlaneItemType.Direction 이것은 Sensor에서 취급
                break;
        }
    }
    //
    private void Action_Consumption()
    {
        LevelDesigner.Instance.vertexArray[parentDirection.AxisToIndex()].GoldCount--;
        AudioManager.Instance.PlaySFXSound(SFXSoundType.CoinPickup);
        GameDesignManager.Instance.GameGold += 100;
        StartCoroutine(RoutineShowOff(thisTransform.GetChild(CurrentItemName.BlockItemToIndex()), 3.0f));
        CurrentItemName = PlaneChildItem.None;
    }
    private void Action_Interaction()
    {
        AudioManager.Instance.PlaySFXSound(SFXSoundType.ItemTouch);
        SetActiveBlockItem(ExchangeItemType(GameMode.Easy, CurrentItemName));
        LevelDesigner.Instance.vertexArray[this.parentDirection.AxisToIndex()].CheckItemIsEqual(CurrentItemName);
    }
 

    private IEnumerator RoutineShowOff(Transform transform , float height)
    {
        float from = transform.localPosition.y;
        float to = height;
        float elapsedTime = 0f;
        while (elapsedTime <= 1)
        {
            elapsedTime += Time.deltaTime / landingTime;
            float h = Mathf.Lerp(from, to, elapsedTime.Interpolation(SmoothType.Exponential));
            transform.localPosition = new Vector3(transform.localPosition.x, h, transform.localPosition.z);
            yield return null;
        }
        transform.localPosition = new Vector3(transform.localPosition.x, from, transform.localPosition.z);
        thisTransform.GetChild(_ChildItemStackIndex.Pop()).gameObject.SetActive(false);

    }
    private Func<PlaneChildItem, SFXSoundType> EffectTypeByItem = item =>
    {
        switch (item)
        {
            case PlaneChildItem.Coin:
                return SFXSoundType.CoinDrop;
            default:
                return SFXSoundType.ItemDrop;
        }
    };

    private Func<GameMode, PlaneChildItem, PlaneChildItem> ExchangeItemType = (mode, type) =>
    {
        switch (mode)
        {
            default:
                switch (type)
                {
                    case PlaneChildItem.Interation_Negative:
                        return PlaneChildItem.Interation_Positive;

                    case PlaneChildItem.Interation_Positive:
                        return PlaneChildItem.Interation_Negative;

                    default:
                        return PlaneChildItem.None;
                }
            case GameMode.Hard:

                switch (type)
                {
                    default:
                        return PlaneChildItem.None;
                }

        }
        
    };



}
