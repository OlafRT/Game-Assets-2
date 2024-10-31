using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DOFCamera : MonoBehaviour
{
    public Camera mainCamera; // Main camera reference
    public Volume globalVolume; // Global volume reference

    private DepthOfField depthOfField; // Access the depth of field
    public float focusDistance = 10f; // Default focus distance
    public float aperture = 0.5f; // Controls the amount of blur
    public float checkInterval = 0.1f; // How often to check for focus distance

    private Coroutine focusCoroutine; // Reference to the coroutine

    void Start()
    {
        // Check if the global volume has a Depth of Field component
        if (globalVolume.profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.active = true; // Ensure it's active
        }

        // Start the coroutine to continuously check focus distance
        focusCoroutine = StartCoroutine(CheckFocusDistance());
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
                if (!hit.collider.isTrigger)
                {
                    float hitDistance = hit.distance;

                    // Set the focus distance to the hit distance
                    focusDistance = hitDistance;

                    // Update the depth of field settings
                    if (depthOfField != null)
                    {
                        // Clamp the focus distance to avoid extreme values
                        focusDistance = Mathf.Clamp(focusDistance, 0.1f, 100f);
                        depthOfField.focusDistance.Override(focusDistance);
                        
                        // Clamp the aperture to avoid excessive blur
                        aperture = Mathf.Clamp(aperture, 0.1f, 1.0f); // Adjust as needed
                        depthOfField.aperture.Override(aperture);
                    }
                }
                else
                {
                    Debug.Log("Hit a trigger collider, ignoring.");
                }
            }

            // Debug ray
            Debug.DrawRay(ray.origin, ray.direction * 10, Color .red, 1f);
        }
    }

    // Method to stop the focus coroutine
    public void StopFocusCoroutine()
    {
        if (focusCoroutine != null)
        {
            StopCoroutine(focusCoroutine);
            focusCoroutine = null;
            ResetDepthOfField(); // Reset focus and aperture
        }
    }

    // Method to reset depth of field settings
    public void ResetDepthOfField()
    {
        if (depthOfField != null)
        {
            depthOfField.focusDistance.Override(10f); // Reset to default focus distance
            depthOfField.aperture.Override(0.5f); // Reset to default aperture
        }
    }

    public void StartFocusCoroutine()
    {
        if (focusCoroutine == null)
        {
            focusCoroutine = StartCoroutine(CheckFocusDistance());
        }
    }
}
