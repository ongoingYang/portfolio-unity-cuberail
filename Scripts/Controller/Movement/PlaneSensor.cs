using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameCookInterface;
public class PlaneSensor : TriggerSensor
{

    PlaneBlock planeBlock = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(10))
        {
          
            planeBlock = other.GetComponent<PlaneBlock>();

            switch (planeBlock.category)
            {
                case PlaneItemType.Direction: // planeSensor 에서 할 일
                    switch (planeBlock.CurrentItemName)
                    {
                        case PlaneChildItem.Clockwise:
                            ActionByClockWise();
                            break;
                        case PlaneChildItem.CounterClockwise:
                            ActionByCounterClockWise();
                            break;
                        case PlaneChildItem.Slash:
                            ActionBySlash();
                            break;
                        case PlaneChildItem.BackSlash:
                            ActionByBackSlash();
                            break;
                        default:
                            // PlaneInteractionType.None
                            break;
                    }
                    break;
                default: // planeBlock 에서 할 일
                    planeBlock.ActionByMover();
                    break;
            }
        }
    }
    private void ActionByClockWise()
    {
        bodyMover.IsMoveActive = false;
        StartCoroutine(RoutineSetPosition(planeBlock.transform.position, SFXSoundType.None));
        Quaternion target = Quaternion.LookRotation(bodyTransform.TransformDirection(Vector3.right), bodyTransform.up);
        StartCoroutine(RoutineSetRotation(target));
        if (bodyMover.switchingIdentifier) bodyMover.movingClockWise = !bodyMover.movingClockWise;
        bodyMover.switchingIdentifier = !bodyMover.switchingIdentifier;
    }
    private void ActionByCounterClockWise()
    {
        bodyMover.IsMoveActive = false;
        StartCoroutine(RoutineSetPosition(planeBlock.transform.position, SFXSoundType.None));
        Quaternion target = Quaternion.LookRotation(bodyTransform.TransformDirection(Vector3.left), bodyTransform.up);
        StartCoroutine(RoutineSetRotation(target));
        if (!bodyMover.switchingIdentifier) bodyMover.movingClockWise = !bodyMover.movingClockWise;
        bodyMover.switchingIdentifier = !bodyMover.switchingIdentifier;
    }
    private void ActionBySlash()
    {
        bodyMover.IsMoveActive = false;
        StartCoroutine(RoutineSetPosition(planeBlock.transform.position, SFXSoundType.None));
        Quaternion target = Quaternion.LookRotation(bodyTransform.TransformDirection(
            SlashDirection(bodyMover.movingClockWise,bodyMover.switchingIdentifier)), bodyTransform.up);
        StartCoroutine(RoutineSetRotation(target));

        bodyMover.switchingIdentifier = !bodyMover.switchingIdentifier;
    }
    private void ActionByBackSlash()
    {
        bodyMover.IsMoveActive = false;
        StartCoroutine(RoutineSetPosition(planeBlock.transform.position, SFXSoundType.None));
        Quaternion target = Quaternion.LookRotation(bodyTransform.TransformDirection(
          BackSlashDirection(bodyMover.movingClockWise, bodyMover.switchingIdentifier)), bodyTransform.up);
        StartCoroutine(RoutineSetRotation(target));
        bodyMover.movingClockWise = !bodyMover.movingClockWise;
        bodyMover.switchingIdentifier = !bodyMover.switchingIdentifier;
    }

    private Func<bool, bool, Vector3> SlashDirection = (clockWise,identifier) =>
    {
        switch (clockWise)
        {
            case true:
                switch (identifier)
                {
                    case true:
                        return Vector3.left;
                    default:
                        return Vector3.right;
                }
            case false:
                switch (identifier)
                {
                    case true:
                        return Vector3.left;
                    default:
                        return Vector3.right;
                }
            default:
                return Vector3.zero;
        }
    };
    private Func<bool, bool, Vector3> BackSlashDirection = (clockWise, identifier) =>
    {
        switch (clockWise)
        {
            case true:
                switch (identifier)
                {
                    case true:
                        return Vector3.right;
                    default:
                        return Vector3.left;
                }
            case false:
                switch (identifier)
                {
                    case true:
                        return Vector3.right;
                    default:
                        return Vector3.left;
                }
            default:
                return Vector3.zero;
        }
    };
}
