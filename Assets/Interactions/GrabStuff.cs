using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro

public class GrabStuff : MonoBehaviour
{
    [SerializeField] private KeyCode throwKey = KeyCode.Space; // Key to throw the object
    [SerializeField] private KeyCode grabKey = KeyCode.E; // Key to pick up the object
    [SerializeField] private float grabDistance = 10f; // Maximum distance to grab
    [SerializeField] private float throwForce = 10f; // Force to apply when throwing
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation
    [SerializeField] private TMP_Text interactionText; // Reference to the TextMeshPro UI text
    [SerializeField] private string interactionMessage = "E"; // Message to display

    private Rigidbody targetRigidbody;
    private Transform objectTransform;

    private void Awake()
    {
        objectTransform = transform;
        HideInteractionText(); // Initially hide the interaction text
    }

    private void Update()
    {
        // Check if we have a target to throw
        if (targetRigidbody != null)
        {
            HandleThrowInput();
            HandleRotationInput(); // Check for rotation input when an object is grabbed
            HideInteractionText(); // Hide interaction text when holding an object
        }
        else
        {
            HandleGrabInput();
            CheckForInteractable(); // Check for interactable objects
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
        if (Input.GetKeyDown(grabKey)) // Use the customizable grab key
        {
            TryGrab();
        }
    }

    private void HandleRotationInput()
    {
        // Check for rotation input using the customizable keys
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // Scrolling up
        {
            RotateObject(1); // Rotate up
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) // Scrolling down
        {
            RotateObject(-1); // Rotate down
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

    private void RotateObject(int direction)
    {
        // Calculate the rotation amount based on the direction and rotation speed
        float rotationAmount = direction * rotationSpeed * Time.deltaTime;

        // Rotate around the X axis
        if (targetRigidbody != null)
        {
            targetRigidbody.transform.Rotate(Vector3.right, rotationAmount);
        }
    }

    private void CheckForInteractable()
    {
        // Perform a raycast to check for interactable objects
        if (Physics.Raycast(objectTransform.position, objectTransform.forward, out RaycastHit hitInfo, grabDistance))
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                ShowInteractionText(true); // Show the interaction text
                // Check for interaction input
                if (Input.GetKeyDown(grabKey)) // Use the grab key for interaction
                {
                    interactable.Interact(); // Call the interact method of the target
                }
            }
            else
            {
                HideInteractionText(); // Hide the interaction text if not looking at an interactable object
            }
        }
        else
        {
            HideInteractionText(); // Hide the interaction text if not looking at any object
        }
    }
    private void ShowInteractionText(bool show)
    {
        interactionText.gameObject.SetActive(show);
        if (show)
        {
            interactionText.text = interactionMessage; // Set the interaction message
        }
    }

    private void HideInteractionText()
    {
        interactionText.gameObject.SetActive(false); // Hide the interaction text
    }

    // New method for interaction
    public bool TryInteract(out RaycastHit hitInfo)
    {
        // Perform a raycast to check for objects within grab distance
        return Physics.Raycast(objectTransform.position, objectTransform.forward, out hitInfo, grabDistance);
    }
}