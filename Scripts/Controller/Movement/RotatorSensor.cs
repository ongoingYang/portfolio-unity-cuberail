using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCookInterface;
public class RotatorSensor : TriggerSensor {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(11))
        {
            bodyMover.switchingIdentifier = !bodyMover.switchingIdentifier;
            bodyMover.IsMoveActive = false;
            bodyMover.ActionState = ActionState.Jumping;
            StartCoroutine(RoutineSetPosition(other.transform.position, SFXSoundType.MoverJump));
            Quaternion target = Quaternion.LookRotation(bodyTransform.TransformDirection(Vector3.down), bodyTransform.forward);
            StartCoroutine(RoutineSetRotation(target));
        }
    }
    #region Legacy
    //private void OnTriggerExit(Collider other)
    //{
    //    if (!IsOntrigger)
    //    {
    //        if (other.gameObject.layer.Equals(11))
    //        {
    //            IsOntrigger = true;
    //            bodyMover.IsMoveActive = false;
    //            bodyMover.switchingIdentifier = !bodyMover.switchingIdentifier;
    //            bodyMover.ActionState = ActionState.Jump;
    //            Quaternion target = Quaternion.LookRotation(bodyTransform.TransformDirection(Vector3.down), bodyTransform.forward);
    //            StartCoroutine(RoutineSetRotation(target));

    //        }
    //    }
    //}
    #endregion



}
