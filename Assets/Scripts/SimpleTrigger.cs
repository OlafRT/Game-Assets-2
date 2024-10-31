using UnityEngine;

public class SimpleTrigger : MonoBehaviour
{
    public Collider[] triggerColliders; // Array of trigger colliders

    private void OnTriggerEnter(Collider other)
    {
        // Log when something enters the trigger
        Debug.Log($"{other.gameObject.name} entered the trigger.");
    }
}