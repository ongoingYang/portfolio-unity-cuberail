using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCookInterface;

public class HoleBlock : Block {

    public delegate void EventHandler(HoleBlock block, int index);
    public event EventHandler BlockClickEvent;
    public Identifier identifier;

    private bool isSelected;
    public int groupIndex;
    public int controllerID;
    public HoleBlock faceBlock;

    public bool IsSelected
    {
        get{ return isSelected; }
        set{
            isSelected = value; 
            if (isSelected) { // 선택 표시 그래픽 효과
                this.transform.GetChild(0).gameObject.SetActive(true);
            }
            else {
                this.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    private void Awake()
    {
        IsSelected = false;
    }
    public override void Initialize(int origin, int j, int k){
        base.Initialize(origin, j, k);
        if (k == GameDesignManager.blockLevelSize)
        {
            this.transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);
            identifier = Identifier.Negative;
        }
        else if (j == GameDesignManager.blockLevelSize)
        {
            this.transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.up);
            identifier = Identifier.Positive;

        }
        controllerID = origin * 2 + (identifier == Identifier.Negative ? 0 : 1);

        StartCoroutine(RoutineShowUp(transform, SFXSoundType.BlockDrop, landingheight, landingTime));
    }
    //negativeXcontroller 0 //origin 0
    //positiveXcontroller 1 //origin 0
    //negativeYcontroller 2 //origin 1 
    //positiveYcontroller 3 //origin 1
    //negativeZcontroller 4 //origin 2 
    //positiveZcontroller 5 //origin 2 

    private void OnMouseDown()
    {
        BlockClickEvent(this, groupIndex);
    }



}
