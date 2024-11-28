using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0; // Key to interact
    [SerializeField] private AudioClip interactionSound; // Sound to play on interaction
    [SerializeField] private AudioSource audioSource; //  AudioSource for playing sound
    [SerializeField] private GrabStuff grabStuff; // The GrabStuff script

    private void Update()
    {
        // Only allow interaction if not holding an object
        if (Input.GetKeyDown(interactKey) && grabStuff.IsHoldingObject() == false)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // Use the TryInteract method from the GrabStuff script
        if (grabStuff.TryInteract(out RaycastHit hitInfo))
        {
            // Check if the object has the IInteractable component
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Call the interact method on the target
                interactable.Interact(); // Call the interact method of the target

                // Play the interaction sound
                PlayInteractionSound();
            }
        }
    }

    private void PlayInteractionSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound); // Play the sound
        }
    }
}