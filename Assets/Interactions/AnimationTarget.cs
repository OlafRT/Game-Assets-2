using UnityEngine;

public class AnimationTarget : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator; // Reference to the Animator component
    [SerializeField] private string animationTrigger = "Interact"; // Trigger name for the animation

    public void Interact()
    {
        PlayAnimation();
        Debug.Log($"{gameObject.name} interacted with.");
    }

    private void PlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(animationTrigger); // Use the specified trigger to play the animation
        }
    }
}