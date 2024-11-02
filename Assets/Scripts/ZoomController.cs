using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ZoomController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float zoomSpeed = 5f;
    public float minFOV = 10f;
    public float maxFOV = 60f;
    public float zoomTime = 1f; // Time it takes to zoom in/out in seconds
    private float zoomTimer = 0f;
    private bool isZoomingIn = false;

    private void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            if (!isZoomingIn)
            {
                isZoomingIn = true;
                zoomTimer = 0f;
            }

            zoomTimer += Time.deltaTime;
            float t = Mathf.Clamp01(zoomTimer / zoomTime);

            float newFOV = Mathf.Lerp(maxFOV, minFOV, t);
            virtualCamera.m_Lens.FieldOfView = newFOV;
        }
        else
        {
            if (isZoomingIn)
            {
                zoomTimer += Time.deltaTime;
                float t = Mathf.Clamp01(zoomTimer / zoomTime);

                float newFOV = Mathf.Lerp(minFOV, maxFOV, t);
                virtualCamera.m_Lens.FieldOfView = newFOV;

                if (t >= 1f)
                {
                    isZoomingIn = false;
                }
            }
        }
    }
}

