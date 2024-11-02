using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    public Collider[] triggerColliders; // Array of trigger colliders
    public string targetTag = "Destroy"; // Tag of objects to destroy
    public AudioClip[] destructionSounds; // Array of sound clips for destruction
    public AudioSource audioSource; // Reference to an existing AudioSource component

    private void Start()
    {
        // Check if all trigger colliders are set as triggers
        foreach (var collider in triggerColliders)
        {
            if (collider != null && !collider.isTrigger)
            {
                // You could keep this check if needed for debugging purposes
                // Debug.LogWarning($"{collider.name} is not a trigger collider. Please ensure all colliders are set as triggers.");
            }
        }

        // Optionally, you can check if the audioSource is assigned
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is not assigned. Please assign an AudioSource component in the inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the specified tag
        if (other.CompareTag(targetTag))
        {
            PlayDestructionSound(); // Play a random destruction sound
            Destroy(other.gameObject); // Destroy the object
        }
    }

    private void PlayDestructionSound()
    {
        if (destructionSounds.Length > 0 && audioSource != null)
        {
            // Randomly select a sound clip
            AudioClip clip = destructionSounds[Random.Range(0, destructionSounds.Length)];

            // Set a random pitch between 0.8 and 1.2
            audioSource.pitch = Random.Range(0.8f, 1.2f);

            // Play the sound
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Optional: You can keep this if you need to handle exit events
        // if (other.CompareTag(targetTag))
        // {
        //     // Handle exit logic if needed
        // }
    }
}