using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCookInterface;

public class HoleSensor : TriggerSensor {

    private void OnTriggerExit(Collider other)
    {
        if (!IsOntrigger)
        {
            if (other.gameObject.layer.Equals(9)) //holeBlock Layer
            {
                IsOntrigger = true;

                HoleBlock holeblock = other.GetComponent<HoleBlock>();
                bool isSelected = holeblock.IsSelected;
                bool isHoleLinked = HoleSupervisor.Instance.activeControllerID.Count == 2;
                bool isActivated = HoleSupervisor.Instance.controllerGroup[holeblock.controllerID].IsActivated;

                switch (isActivated)
                {
                    case true:
                        switch (isSelected) // 충돌 블락 선택됨
                        {
                            case true:
                                switch (isHoleLinked)  // 스택 카운트 2개 --> 텔레포트 할 곳 존재
                                {
                                    case true:

                                        ActionByLinkedHole(holeblock);
                                        break;

                                    case false:
                                        ActionByInverse();
                                        break;
                                }
                                break;
                            case false: // isSelected가 아니지만 activated된 상태의 컨트롤러의 어느 holeblock 충돌
                                ActionByInverse();
                                break;
                        }
                        break;
                    case false:  //activated 상태가 아닌 컨트롤러의 어느 holeblock 충돌
                        ActionByInverse();
                        break;
                }

            }
        }
    }

    private void ActionByInverse()
    {
        bodyMover.IsMoveActive = false;
        AudioManager.Instance.PlaySFXSound(SFXSoundType.MoverTurn);
        bodyMover.movingClockWise = !bodyMover.movingClockWise;
        Quaternion target = Quaternion.LookRotation(bodyTransform.TransformDirection(Vector3.back), bodyTransform.up);
        StartCoroutine(RoutineSetRotation(target));
    }
    private void ActionByLinkedHole(HoleBlock block)
    {
        AudioManager.Instance.PlaySFXSound(SFXSoundType.MoverEnter);
        bodyTransform.rotation = HoleSupervisor.Instance.FindOutletBlock(block.controllerID).transform.rotation;
        bodyTransform.position = HoleSupervisor.Instance.FindOutletBlock(block.controllerID).transform.position;
        if (HoleSupervisor.Instance.FindOutletBlock(block.controllerID).identifier == Identifier.Positive){
            bodyMover.movingClockWise = true;
            bodyMover.switchingIdentifier = true;
        }
        else{
            bodyMover.movingClockWise = false;
            bodyMover.switchingIdentifier = false;
        }
    }
}
