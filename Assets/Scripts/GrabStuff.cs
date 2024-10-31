using UnityEngine;

public class GrabStuff : MonoBehaviour
{
    [SerializeField] private KeyCode throwKey = KeyCode.Space; // Key to throw the object
    [SerializeField] private float grabDistance = 10f; // Maximum distance to grab
    [SerializeField] private float throwForce = 10f; // Force to apply when throwing

    private Rigidbody targetRigidbody;
    private Transform objectTransform;

    private void Awake()
    {
        objectTransform = transform;
    }

    private void Update()
    {
        // Check if we have a target to throw
        if (targetRigidbody != null)
        {
            HandleThrowInput();
        }
        else
        {
            HandleGrabInput();
        }
    }

    private void HandleThrowInput()
    {
        if (Input.GetKeyDown(throwKey))
        {
            Throw();
        }
    }

    private void HandleGrabInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            TryGrab();
        }
    }

    private void TryGrab()
    {
        // Perform a raycast to check for objects within grab distance
        if (Physics.Raycast(objectTransform.position, objectTransform.forward, out RaycastHit hitInfo, grabDistance))
        {
            Rigidbody rb = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Grab(rb);
            }
        }
    }

    private void Grab(Rigidbody rb)
    {
        targetRigidbody = rb;

        // Optional: Attach the object to the player (e.g., set parent)
        rb.transform.SetParent(objectTransform); // Set the grabbed object as a child of this object
        rb.isKinematic = true; // Make it kinematic to prevent physics interactions while held
    }

    private void Throw()
    {
        if (targetRigidbody != null)
        {
            // Optional: Detach the object from the player before throwing
            targetRigidbody.transform.SetParent(null); // Remove the parent
            targetRigidbody.isKinematic = false; // Make it non-kinematic to allow physics interactions
            targetRigidbody.AddForce(objectTransform.forward * throwForce, ForceMode.Impulse);
            targetRigidbody = null; // Reset the target after throwing
        }
    }
}
