using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Key to interact
    [SerializeField] private AudioClip interactionSound; // Sound to play on interaction
    [SerializeField] private AudioSource audioSource; // Reference to an AudioSource for playing sound
    [SerializeField] private GrabStuff grabStuff; // Reference to the GrabStuff script

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
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