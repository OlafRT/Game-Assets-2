using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smashed : MonoBehaviour
{
    // The new material to apply when the object is smashed
    public Material smashedMaterial;

    // The minimum force required to count as a smash
    public float smashForceThreshold = 5.0f;

    // The AudioClip to play when the object is smashed
    public AudioClip smashSound;

    // Reference to the AudioSource component
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This method is called when the collider enters the trigger
    private void OnCollisionEnter(Collision collision)
    {
        // Calculate the impact force based on relative velocity and mass
        float impactForce;

        // Check if the colliding object has a Rigidbody
        if (collision.rigidbody != null)
        {
            // If it has a Rigidbody, use its mass
            impactForce = collision.relativeVelocity.magnitude * collision.rigidbody.mass;
        }
        else
        {
            // If it does not have a Rigidbody, assume a default mass (e.g., 1)
            impactForce = collision.relativeVelocity.magnitude * 1.0f; // Assuming a mass of 1 for static objects
        }

        // Check if the impact force exceeds the threshold
        if (impactForce >= smashForceThreshold)
        {
            // Change the material of the object to the smashed material
            GetComponent<Renderer>().material = smashedMaterial;

            // Play the smash sound
            if (audioSource != null && smashSound != null)
            {
                audioSource.PlayOneShot(smashSound);
            }

            // Optionally, you can add additional logic here, like playing a particle effect
        }
    }
}