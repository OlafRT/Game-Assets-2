using System.Collections;
using UnityEngine;
using StarterAssets;

public class PlayerScaling : MonoBehaviour
{
    [Header("Scaling Settings")]
    public float GrowFactor = 1.5f; // How much the player grows
    public float ShrinkFactor = 0.5f; // How much the player shrinks
    public KeyCode growKey = KeyCode.G; // Key to grow
    public KeyCode shrinkKey = KeyCode.H; // Key to shrink
    private Vector3 originalScale;
    private bool isGrowing = false;
    private bool isShrinking = false;

    private FirstPersonController firstPersonController; // Reference to the FirstPersonController script

    void Start()
    {
        // Store the original scale of the player
        originalScale = transform.localScale;

        // Get reference to the FirstPersonController (if you want to adjust speed/jump)
        firstPersonController = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        HandleScaling();
    }

    private void HandleScaling()
    {
        // Detect if the player presses the grow key and isn't already growing
        if (Input.GetKeyDown(growKey) && !isGrowing)
        {
            StartCoroutine(ChangeScale(originalScale * GrowFactor, 1f));
            isGrowing = true;
            isShrinking = false;
        }
        // Detect if the player presses the shrink key and isn't already shrinking
        else if (Input.GetKeyDown(shrinkKey) && !isShrinking)
        {
            StartCoroutine(ChangeScale(originalScale * ShrinkFactor, 1f));
            isShrinking = true;
            isGrowing = false;
        }
    }

    // Coroutine to smoothly change the player's scale
    private IEnumerator ChangeScale(Vector3 targetScale, float duration)
    {
        Vector3 startingScale = transform.localScale;
        float elapsedTime = 0f;

        // Optionally, adjust speed based on scale (directly modify FirstPersonController's MoveSpeed and SprintSpeed)
        if (firstPersonController != null)
        {
            float scaleRatio = targetScale.x / originalScale.x;
            firstPersonController.MoveSpeed *= scaleRatio; // Directly adjust MoveSpeed
            firstPersonController.SprintSpeed *= scaleRatio; // Directly adjust SprintSpeed
        }

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
