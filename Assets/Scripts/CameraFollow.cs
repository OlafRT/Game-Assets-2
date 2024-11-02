using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The tank to follow
    public float distance = 40.0f; // Distance from the tank
    public float height = 20.0f; // Height above the tank
    public float rotationSpeed = 2.0f; // Speed of rotation
    public float verticalRotationLimit = 20.0f; // Limit for vertical rotation
    public LayerMask obstructionLayer; // Layer for walls and obstructions
    public float slideSmoothness = 1f; // Smoothness factor for sliding along walls

    private float currentAngle = 0.0f; // Current angle around the tank
    private float currentVerticalAngle = 0.0f; // Current vertical angle

    void Start()
    {
        // Set the initial camera position
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Update the current angles based on mouse input
        currentAngle += mouseX;
        currentVerticalAngle -= mouseY; // Invert the vertical movement

        // Clamp the vertical rotation angle to avoid flipping over
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalRotationLimit, verticalRotationLimit);

        // Update the camera position and rotation
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Calculate the desired position
        Vector3 offset = new Vector3(0, height, -distance);
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentAngle, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Check for obstructions
        RaycastHit hit;
        if (Physics.Raycast(target.position, rotation * Vector3.back, out hit, distance, obstructionLayer))
        {
            // If there is an obstruction, calculate a slide position
            Vector3 slidePosition = hit.point + hit.normal * 0.5f; // Offset slightly from the wall

            // Maintain the camera height and the current vertical angle
            slidePosition.y = target.position.y + height;

            // Smoothly transition to the slide position but keep the vertical angle unchanged
            transform.position = Vector3.Lerp(transform.position, slidePosition, slideSmoothness);
        }
        else
        {
            // If no obstruction, set the camera to the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, slideSmoothness);
        }

        // Set the camera to look at the target, maintaining the current vertical angle
        transform.LookAt(target.position + Vector3.up * height); // Adjust look at to account for height
    }
}