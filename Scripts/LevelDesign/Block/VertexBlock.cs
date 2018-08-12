using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameCookInterface;
public class VertexBlock : Block {

    // 각 parent 좌표(x,y,z)에 해당하는 planeBlock들의 접근을 담당
    // planeBlock 들의 특수한 아이템(EGG 를 터치했을 때 색상 변화 등)들의 인터렉션을 옵저버할 수 있다.
    public override void Initialize(int origin, int j, int k)
    {
        base.Initialize(origin, j, k);
        planeBlockGroup = new PlaneBlock[GameDesignManager.blockLevelSize * GameDesignManager.blockLevelSize];
        StartCoroutine(RoutineShowUp(transform, SFXSoundType.BlockDrop, landingheight, landingTime));
        _isOncleared = false;
    }

    PlaneChildItem cleredByItem;
    public PlaneBlock[] planeBlockGroup; // 해당 Plane들을 모두 하나의 배열 테이블로 묶는다.
    public List<int> specificPlaneBlockList = new List<int>(); // 인터렉션의 대상이 되는 주시해야 할 planeBlock 들의 index를 여기에 추가
    private int goldCount;
    [SerializeField] Transform informSign;
    bool isOnMatched;
    private bool _isOncleared;
    float fadeHeight;


    public bool IsOncleared{
        get{return _isOncleared;}
    }
    public bool allConsumed;
    public int GoldCount
    {
        get
        {
            return goldCount;
        }

        set
        {
            goldCount = value;
            if (goldCount == 0) allConsumed = true;
            GameDesignManager.Instance.CheckConsumed();
        }
    }

    private void Start()
    {
        //levelDesigner에서 먼저 initialize가 완료된 이후 planeBlockGroup에 배열 사이즈를 할당한다.
        GoldCount = -1;
        SetPlaneGroup();
        fadeHeight = Mathf.Abs(informSign.localPosition.y);
    }

    private void SetPlaneGroup()
    {
        int index = 0;
        for (int j = 0; j < GameDesignManager.blockLevelSize; j++)
        {
            for (int k = 0; k < GameDesignManager.blockLevelSize; k++)
            {
                planeBlockGroup[index] = LevelDesigner.Instance.planeArray[parentDirection.AxisToIndex(), j, k];
                planeBlockGroup[index].planeGroupID = index;
                index++;
            }
        }
    }


    private void OnMouseDown()
    {
        if (!IsOnMatched) return;

        switch (cleredByItem)
        {
            case PlaneChildItem.Interation_Negative:
                PoolController.Instance.OnClickVertex(ObjecTPoolType.SpawnWolf, parentDirection, this.transform.position);
                break;
            case PlaneChildItem.Interation_Positive:
                PoolController.Instance.OnClickVertex(ObjecTPoolType.SpawnLizard, parentDirection, this.transform.position);
                break;
            default:
                break;
        }
            
        InterationItemToCoin();
        IsOnMatched = false;
        _isOncleared = true;
    }
    public bool IsOnMatched
    {
        get
        {
            return isOnMatched;
        }

        set
        {
            isOnMatched = value;
            if (isOnMatched)
            {
                cleredByItem = planeBlockGroup[specificPlaneBlockList[0]].CurrentItemName;
                AudioManager.Instance.PlaySFXSound(SFXSoundType.InformSign);
            }
            else
            {

                cleredByItem = PlaneChildItem.None;
            }
        }
    }
    public bool CheckItemIsEqual(PlaneChildItem itemType)
    {
        foreach (int id in specificPlaneBlockList){
            if (planeBlockGroup[id].CurrentItemName != itemType) {
                if(IsOnMatched) StartCoroutine(RoutineDisplaySign(informSign.position.y, -fadeHeight, 0.5f, informSign));
                IsOnMatched = false;
                return false;
            }
        }
        if (!IsOnMatched) StartCoroutine(RoutineDisplaySign(informSign.position.y, 0, 0.5f, informSign));
        IsOnMatched = true;
        return true;
    }
    public void InterationItemToCoin()
    {
        goldCount = 0;
        allConsumed = false;
        foreach (int id in specificPlaneBlockList)
        {
            planeBlockGroup[id].SetActiveBlockItem(PlaneChildItem.Coin);
            GoldCount++;
        }
        specificPlaneBlockList.Clear();
    }

    public void AddReferenceList()
    {
        specificPlaneBlockList.Clear();
        foreach (PlaneBlock block in planeBlockGroup.Where(t => t.category == PlaneItemType.Interaction))
        {
            specificPlaneBlockList.Add(block.planeGroupID);
        }
       
    }
    IEnumerator RoutineDisplaySign(float fromPos, float toPos,  float fadeTime, Transform displaySign)
    {
        float from = fromPos;
        float to = toPos;
        float elapsedTime = 0f;
        while (elapsedTime <= 1f)
        {
            elapsedTime += Time.deltaTime / fadeTime;
            displaySign.localPosition = new Vector3(
                displaySign.localPosition.x,
                Mathf.Lerp(from, to, elapsedTime.Interpolation(SmoothType.EaseInWithCos)),
                displaySign.localPosition.z);
            yield return null;
        }
    }

}

