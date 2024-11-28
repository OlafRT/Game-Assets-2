using UnityEngine;
using System.Collections;

public class AnimationTarget : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator; // Reference to the Animator component
    [SerializeField] private string animationTrigger = "Interact"; // Trigger name for the animation
    [SerializeField] private GameObject[] animationObjectsToToggle;

    public void Interact()
    {
        PlayAnimation();
        Debug.Log($"{gameObject.name} interacted with!");
        ToggleAnimationObjects(); // Call the renamed method
        StartCoroutine(DisableObjectsAfterDelay(9f)); // Start the coroutine to disable objects after 9 seconds
    }

    private void PlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(animationTrigger); // Use the specified trigger to play the animation
        }
    }

    private void ToggleAnimationObjects()
    {
        foreach (GameObject obj in animationObjectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf); // Toggle the active state
            }
        }
    }

    private IEnumerator DisableObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        foreach (GameObject obj in animationObjectsToToggle)
        {
            if (obj != null && obj.activeSelf) // Check if the object is active before disabling it
            {
                obj.SetActive(false); // Disable the object
            }
        }
    }
}