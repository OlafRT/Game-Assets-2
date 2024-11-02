using UnityEngine;

public class GrabStuff : MonoBehaviour
{
    [SerializeField] private KeyCode throwKey = KeyCode.Space; // Key to throw the object
    [SerializeField] private KeyCode grabKey = KeyCode.E; // Key to pick up the object
    [SerializeField] private KeyCode rotateUpKey = KeyCode.UpArrow; // Key to rotate up
    [SerializeField] private KeyCode rotateDownKey = KeyCode.DownArrow; // Key to rotate down
    [SerializeField] private float grabDistance = 10f; // Maximum distance to grab
    [SerializeField] private float throwForce = 10f; // Force to apply when throwing
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation

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
            HandleRotationInput(); // Check for rotation input when an object is grabbed
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
}