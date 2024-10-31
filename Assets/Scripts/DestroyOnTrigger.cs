using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    public Collider[] triggerColliders; // Array of trigger colliders
    public string targetTag = "Destroy"; // Tag of objects to destroy

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
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the specified tag
        if (other.CompareTag(targetTag))
        {
            Destroy(other.gameObject); // Destroy the object
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