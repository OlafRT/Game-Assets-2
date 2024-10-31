using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip enterAudioClip; // Variable for the enter sound
    public AudioClip exitAudioClip;  // Variable for the exit sound
    public float minPitch = 0.95f;    // Minimum pitch variation
    public float maxPitch = 1.05f;    // Maximum pitch variation

    private void Start()
    {
        // Get AudioSource on GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the Collider that entered is the player
        if (other.CompareTag("Player"))
        {
            // Set and play the enter audio clip when trigger is entered
            PlayAudioWithPitchVariation(enterAudioClip); // Play the enter sound
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the Collider that exited is the player
        if (other.CompareTag("Player"))
        {
            // Set and play the exit audio clip
            PlayAudioWithPitchVariation(exitAudioClip); // Play the exit sound
        }
    }

    private void PlayAudioWithPitchVariation(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // Set the audio clip
            audioSource.clip = clip;

            // Randomly vary the pitch
            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;

            // Play the audio
            audioSource.Play();
        }
    }
}

