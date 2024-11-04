using UnityEngine;

    // This script is functioning  as intended. Could probably be improved somehow...
public class DestroyOnTrigger : MonoBehaviour
{
    public string targetTag = "Destroy"; // Tag of objects to destroy, we only want to destroy the ones that have this tag!!!!!
    public AudioClip[] destructionSounds; // Array of sound clips for destruction, to make it more fun to destroy stuff!
    public AudioSource audioSource; // AudioSource component on the object we use to destroy things

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