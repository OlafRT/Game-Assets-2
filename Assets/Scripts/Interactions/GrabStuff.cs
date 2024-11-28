using UnityEngine;
using TMPro;

public class GrabStuff : MonoBehaviour
{
    [SerializeField] private KeyCode throwKey = KeyCode.Space; // Key to throw the object, we can set this to any key
    [SerializeField] private KeyCode grabKey = KeyCode.Mouse0; // Key to pick up the object, we can set this to any key
    [SerializeField] private float grabDistance = 10f; // Maximum distance to grab
    [SerializeField] private float throwForce = 10f; // Force to apply when throwing
    [SerializeField] private float holdDistance = 2f; // Distance to hold the object from the player
    [SerializeField] private TMP_Text interactionText; // Reference to the TextMeshPro UI text
    [SerializeField] private string interactionMessage = "M1"; // Message to display
    [SerializeField] private float smoothingFactor = 10f; // Smoothing factor for object movement
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation

    private Rigidbody targetRigidbody;
    private Transform objectTransform;

    private void Awake()
    {
        objectTransform = transform;
        HideInteractionText(); // Initially hide the interaction text
    }

    private void Update()
    {
        // Check for input to throw or grab
        if (targetRigidbody != null)
        {
            HandleThrowInput();
            HandleMouseDragRotation(); // Handle rotation input via mouse drag
            HideInteractionText(); // Hide interaction text when holding an object
        }
        else
        {
            HandleGrabInput();
            CheckForInteractable(); // Check for interactable objects
        }
    }

    private void FixedUpdate()
    {
        if (targetRigidbody != null)
        {
            MoveHeldObject(); // Move the object to the hold position
        }
    }

    private void HandleThrowInput()
    {
        if (Input.GetKeyDown(throwKey) && !Input.GetKey(KeyCode.R)) // Prevent throwing while holding "R"
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

    private void HandleMouseDragRotation()
    {
        if (Input.GetKey(KeyCode.R) && Input.GetMouseButton(0) && targetRigidbody != null) // Holding "R" and left mouse button
        {
            float rotationX = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime; // Keep Y rotation as is
            float rotationY = -Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime; // Invert X rotation

            // Rotate the object based on mouse movement
            targetRigidbody.transform.Rotate(Vector3.left, rotationX);
            targetRigidbody.transform.Rotate(Vector3.up, rotationY);
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

        // Set constraints to prevent rotation while holding
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent rotation
        rb.useGravity = false; // Disable gravity while holding
    }

    private void MoveHeldObject()
    {
        if (targetRigidbody != null)
        {
            // Calculate the position to hold the object in front of the player
            Vector3 holdPosition = objectTransform.position + objectTransform.forward * holdDistance;

            // Smoothly move the object towards the hold position
            Vector3 direction = holdPosition - targetRigidbody.position;
            Vector3 velocity = direction * smoothingFactor;

            targetRigidbody.velocity = velocity; // Set the velocity to move towards the hold position
        }
    }

    private void Throw()
    {
        if (targetRigidbody != null)
        {
            // Remove constraints before throwing
            targetRigidbody.constraints = RigidbodyConstraints.None; // Remove constraints
            targetRigidbody.useGravity = true; // Re-enable gravity
            targetRigidbody.AddForce(objectTransform.forward * throwForce, ForceMode.Impulse); 
            targetRigidbody = null; // Reset the target after throwing
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

    public bool IsHoldingObject()
    {
        return targetRigidbody != null; // Return true if an object is being held
    }
}
