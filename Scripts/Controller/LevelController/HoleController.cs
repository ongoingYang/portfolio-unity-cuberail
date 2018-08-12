using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCookInterface;
public class HoleController : MonoBehaviour {

    private bool isActivated;       //활성화 여부 
    private int _controllerID;       //HoleSupervisor에서 관리할 list 인덱스로 참조됨

    public int groupIndicator;      //holeGroup 선택 항목 인덱스를 가리킴
    public HoleBlock[] holeGroup;

    public bool IsActivated{
        get {
            return isActivated;
        }
        set{
            isActivated = value;
            /**
            if (IsActivated) // 활성화 그래픽 효과 디버그용 
            {
                for (int i = 0; i < designer.BlockSize; i++){
                    holeGroup[i].gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
            else{
                for (int i = 0; i < designer.BlockSize; i++){
                    holeGroup[i].gameObject.GetComponent<Renderer>().material.color = Color.gray;
                }
            }    
     **/
        }
    }
    public int ControllerID
    {
        get
        {
            return _controllerID;
        }

        set
        {
            _controllerID = value;
        }
    }

    public void Initialize(int id)
    {
        holeGroup = new HoleBlock[GameDesignManager.blockLevelSize];
        ControllerID = id;
        groupIndicator = -1;

        for (int i = 0; i < GameDesignManager.blockLevelSize; i++)
        {
            holeGroup[i] = LevelDesigner.Instance.holeArray[id / 2, id % 2, i];
        }
        LinkToHoleBlock();
    }

    public void DetectHoleClick(HoleBlock click, int index)
    {
        HandleWithHole(click);
    }
    public void HandleWithHole(HoleBlock click)
    {
        if (click.IsSelected)
        { 
            AudioManager.Instance.PlaySFXSound(SFXSoundType.ClickNegative); // 기존 holeBlock 재선택
            click.IsSelected = false;
            this.groupIndicator = -1;
            HoleSupervisor.Instance.DequeueActiveID(ControllerID);
        }
        else
        {                 
            AudioManager.Instance.PlaySFXSound(SFXSoundType.ClickPositive); // 새로 선택
            switch (groupIndicator)
            {
                case -1: // 활성화가 안된 다른 그룹에서 holeBlock 선택
                    click.IsSelected = true;
                    this.groupIndicator = click.groupIndex;
                    HoleSupervisor.Instance.EnqueueActiveID(ControllerID);
                    break;
                default: // 활성화가 된 곳의 그룹 내의 다른 holeBlock 선택 // 마지막으로 조작한 곳을 end로 
                    holeGroup[groupIndicator].IsSelected = false;
                    click.IsSelected = true;
                    this.groupIndicator = click.groupIndex;
                    HoleSupervisor.Instance.SelectIDToEndQueue(ControllerID);
                    break;
            }
        }
    }

    // 컨트롤러 이벤트 연결 초기화
    public void LinkToHoleBlock()
    {
        for (int i = 0; i < holeGroup.Length; i++)
        {
            holeGroup[i].BlockClickEvent += DetectHoleClick;
            holeGroup[i].groupIndex = i;
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < holeGroup.Length; i++)
        {
            holeGroup[i].BlockClickEvent -= DetectHoleClick;
        }
    }
}
