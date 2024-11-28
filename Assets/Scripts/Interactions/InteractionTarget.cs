using UnityEngine;

public class InteractionTarget : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject[] objectsToToggle; // Array of GameObjects to enable/disable

    // Method to handle interaction
    public void Interact()
    {
        ToggleObjects();
        Debug.Log($"{gameObject.name} interacted with.");
    }

    private void ToggleObjects()
    {
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf); // Toggle the active state
            }
        }
    }
}