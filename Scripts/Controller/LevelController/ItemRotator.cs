using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotator : MonoBehaviour {

    Transform thisTransform;

    public float rotateSpeed;
    private void Awake()
    {
        thisTransform = GetComponent<Transform>();
    }
	// Update is called once per frame
	void Update () {
        thisTransform.RotateAround(thisTransform.position, thisTransform.TransformDirection(Vector3.up), rotateSpeed * Time.deltaTime);
    }
}
