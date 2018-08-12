using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameCookInterface;
public class CameraViewController : MonoBehaviour {

    public float zoomDuration = 1.0f;
    [SerializeField] private float cameraDist;
    private Camera mainCamera;
    public Transform cameraTarget;
    public void Start()
    {
        if(GameDesignManager.Instance != null)
        {
            cameraDist = 2 * (GameDesignManager.blockLevelSize + 1) * Mathf.Sin(Mathf.PI / 3f) * Mathf.Sin(Mathf.PI / 3f);
            mainCamera = this.GetComponent<Camera>();
            this.transform.position = new Vector3(cameraDist, cameraDist, -cameraDist);
            mainCamera.transform.LookAt(cameraTarget);
            float viewSize = cameraDist;
            StartCoroutine(RoutineZoomIn(viewSize));
        }else
        {
            Debug.LogWarning("GameDesignManager.blockLevelSize missing");
        }
        
    }
    IEnumerator RoutineZoomIn(float size)
    {
        float prev = size * 3;
        float next = size;

        float elapsedTime = 0f;
        while (elapsedTime <= 1.0f)
        {
            elapsedTime += Time.deltaTime / zoomDuration;
            mainCamera.orthographicSize = Mathf.Lerp(prev, next, elapsedTime.Interpolation(SmoothType.SmootherStep));
            yield return null;
        }
    }
}
