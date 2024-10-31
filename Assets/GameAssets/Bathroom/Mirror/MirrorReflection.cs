using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorReflection : MonoBehaviour 
{
    // these are the components you can set up in the inspector for the mirror
    public Camera mainCamera;
    public Camera mirrorCamera;
    public Transform mirrorPlane;
    public float maxReflectionAngle = 45f;

    private void Start() //this disables the mirror camera when you start the game as to not render unnecessarily
    {
        if (mirrorCamera != null) 
        {
            mirrorCamera.gameObject.SetActive(false);
        }
    }

    private void LateUpdate() 
    {
        if (!mainCamera || !mirrorCamera || !mirrorPlane || !mirrorCamera.gameObject.activeSelf) return;

        //normal of the mirror surface
        Vector3 mirrorNormal = mirrorPlane.forward; 
        //position of the main camera relative to the mirror
        Vector3 cameraToMirror = mainCamera.transform.position - mirrorPlane.position;
        //reflect camera position across the mirror
        Vector3 reflectedPosition = mirrorPlane.position - cameraToMirror;
        //reflected direction of the main camera forward vector across the mirror normal
        Vector3 reflectedDirection = Vector3.Reflect(mainCamera.transform.forward, mirrorNormal);

        //angle between the mirror forward direction and the reflected direction
        float angleToMirrorNormal = Vector3.Angle(mirrorNormal, reflectedDirection);

        //clamp reflection if over max angle
        if (angleToMirrorNormal > maxReflectionAngle) 
        {
            reflectedDirection = Vector3.Slerp(mirrorNormal, reflectedDirection, maxReflectionAngle / angleToMirrorNormal);
        }

        //does the same but on the vertical instead of the horizontal
        float verticalAngle = Vector3.Angle(mirrorNormal, reflectedDirection);

        if (verticalAngle > maxReflectionAngle) 
        {
            Vector3 verticalClampedDirection = Vector3.Slerp(mirrorNormal, reflectedDirection, maxReflectionAngle / verticalAngle);
            reflectedDirection = verticalClampedDirection;
        }
        //face camera in the clamped reflection direction
        mirrorCamera.transform.rotation = Quaternion.LookRotation(reflectedDirection, mirrorPlane.up);
    }

    //enables and disables the camera when the player enters/exits the trigger collider for efficiency and unneccesary rendering
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player") && mirrorCamera != null) {
            mirrorCamera.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player") && mirrorCamera != null) {
            mirrorCamera.gameObject.SetActive(false);
        }
    }
}


