using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SimpleDOF : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public Volume globalVolume; // Reference to the global volume with DoF settings

    private DepthOfField depthOfField; // Access the depth of field component
    public float checkInterval = 0.1f; // How often to check for focus distance

    private Coroutine focusCoroutine; // Reference to the coroutine

    void Start()
    {
        // Check if the global volume has a Depth of Field component
        if (globalVolume.profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.active = true; // Ensure it's active
            StartFocusCoroutine(); // Start the focus coroutine
        }
        else
        {
            Debug.LogError("Depth of Field component not found in the global volume.");
        }
    }

    private IEnumerator CheckFocusDistance()
    {
        while (true)
        {
            UpdateFocusBasedOnRaycast();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void UpdateFocusBasedOnRaycast()
    {
        if (mainCamera != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the collider is not a trigger
                if (!hit.collider.isTrigger)
                {
                    // Get the distance to the hit object
                    float hitDistance = hit.distance;

                    // Set the focus distance to the hit distance
                    if (depthOfField != null)
                    {
                        depthOfField.focusDistance.Override(hitDistance);
                    }
                }
                else
                {
                    Debug.Log("Hit a trigger collider, ignoring.");
                }
            }
        }
    }

    // Method to stop the focus coroutine
    public void StopFocusCoroutine()
    {
        if (focusCoroutine != null)
        {
            StopCoroutine(focusCoroutine);
            focusCoroutine = null;
        }
    }

    public void StartFocusCoroutine()
    {
        if (focusCoroutine == null)
        {
            focusCoroutine = StartCoroutine(CheckFocusDistance());
        }
    }

    private void OnDestroy()
    {
        // Stop the coroutine when the object is destroyed
        StopFocusCoroutine();
    }
}
