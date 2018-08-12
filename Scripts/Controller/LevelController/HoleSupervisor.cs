using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class HoleSupervisor : MonoBehaviour {

    
    const int _maxControllerIndex = 6; // default
    const int _axisIndex = 3; // default
    [SerializeField] private HoleController holeControllerPrefab;
    public HoleController[] controllerGroup = new HoleController[_maxControllerIndex];

    #region controller index info / Initialize
    /**
    controllerGroup[0] == negativeXcontroller 
    controllerGroup[1] == positiveXcontroller 
    controllerGroup[2] == negativeYcontroller 
    controllerGroup[3] == positiveYcontroller 
    controllerGroup[4] == negativeZcontroller 
    controllerGroup[5] == positiveZcontroller 
    **/
    public static HoleSupervisor Instance;
    private void MakeInstance(){
        if (Instance == null){
            Instance = this;
        }
    }
    private void Awake(){
        MakeInstance();
    }
    private void Start(){
        Initialize();
    }

    public void Initialize()
    {
        for (int id = 0; id < _maxControllerIndex; id++)
        {
            HoleController controller = Instantiate(holeControllerPrefab) as HoleController;
            controller.transform.SetParent(this.transform);
            controller.Initialize(id);
            controllerGroup[id] = controller;
        }
        SetFaceBlock();
    }
    public void SetFaceBlock()
    {
        for (int i = 0; i < GameDesignManager.blockLevelSize; i++)
        {
            controllerGroup[0].holeGroup[i].faceBlock = controllerGroup[3].holeGroup[i];
            controllerGroup[1].holeGroup[i].faceBlock = controllerGroup[4].holeGroup[i];
            controllerGroup[2].holeGroup[i].faceBlock = controllerGroup[5].holeGroup[i];
            controllerGroup[3].holeGroup[i].faceBlock = controllerGroup[0].holeGroup[i];
            controllerGroup[4].holeGroup[i].faceBlock = controllerGroup[1].holeGroup[i];
            controllerGroup[5].holeGroup[i].faceBlock = controllerGroup[2].holeGroup[i];
        }
    }
    #endregion

    Func<int, int, bool> CheckLessThan = (count, value) => count < value;
    Func<int, int, bool> CheckMoreThan = (count, value) => count > value;

    public Queue<int> activeControllerID = new Queue<int>();

    public HoleBlock FindOutletBlock(int inletID)
    {
        foreach (HoleController controller in controllerGroup.Where(x => x.IsActivated)) 
        {
            if (controller.ControllerID != inletID)
                return controller.holeGroup[controller.groupIndicator];
        }
        return null;
    }

    public void EnqueueActiveID(int ID)
    {
        switch (CheckLessThan(activeControllerID.Count, 2))
        {
            case true: // 0 ~ 1 일 때 그냥 넣음
                activeControllerID.Enqueue(ID);
                controllerGroup[ID].IsActivated = true;
                break;

            case false: // 2 일 때 앞에 꺼 빼고 넣음
                activeControllerID.Enqueue(ID);
                controllerGroup[ID].IsActivated = true;

                int dequeue = activeControllerID.Dequeue();
                controllerGroup[dequeue].holeGroup[controllerGroup[dequeue].groupIndicator].IsSelected = false;
                controllerGroup[dequeue].groupIndicator = -1;
                controllerGroup[dequeue].IsActivated = false;
                break;
        }
    }
    public void DequeueActiveID(int ID)
    {
        switch (CheckMoreThan(activeControllerID.Count, 1)) // 하나보다 많은지
        {
            case true:
                if (activeControllerID.Peek() == ID){
                    controllerGroup[activeControllerID.Dequeue()].IsActivated = false;
                }
                else{
                    int begin = activeControllerID.Dequeue();
                    int end = activeControllerID.Dequeue();
                    controllerGroup[end].IsActivated = false;
                    activeControllerID.Enqueue(begin);
                }
                break;
            case false:
                int dequeue = activeControllerID.Dequeue();
                controllerGroup[dequeue].IsActivated = false;
                break;
        }
    }
    public void SelectIDToEndQueue(int ID) // 먼저 들어간 것(먼저 나오는 것)을 나중으로 바꿈
    {
        if (activeControllerID.Peek() == ID){
            activeControllerID.Enqueue(activeControllerID.Dequeue());
        }
    }
}

