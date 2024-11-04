using UnityEngine;

public class CameraFollow : MonoBehaviour

    // This script still needs work to function better. It is currently a work in progress. Having an issue that the camera can still sometimes clip through the walls...

{
    public Transform target; // The target for the camera to follow
    public float distance = 40.0f; // Distance from the target
    public float height = 20.0f; // Height above the target
    public float rotationSpeed = 2.0f; // Speed of rotation
    public float verticalRotationLimit = 20.0f; // Limit for vertical rotation, so that  the camera doesn't go upside down or clips through the floor
    public LayerMask obstructionLayer; // Layer for walls and obstructions that we use to make sure the camera doesnt go outside the walls or other objects.
    public float slideSmoothness = 1f; // Smoothness factor for sliding along walls,  higher is smoother but also more expensive and we are using this so the camera won't be as "snappy"

    private float currentAngle = 0.0f; // Current angle around the target
    private float currentVerticalAngle = 0.0f; // Current vertical angle

    void Start()
    {
        // Setting the initial camera position
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        // Get mouse input for controlling the camera around the target
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed; // Horizontal mouse movement
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed; // Vertical mouse movement

        // Update the current angles based on mouse input
        currentAngle += mouseX; // Update the horizontal angle
        currentVerticalAngle -= mouseY; // Update the vertical movement (inverted)

        // Clamp the vertical rotation angle to avoid flipping over or clipping through the floor
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalRotationLimit, verticalRotationLimit);

        // Update the camera position and rotation
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Calculate the desired position
        Vector3 offset = new Vector3(0, height, -distance); // Offset from the target position
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentAngle, 0); // Rotation based on angles
        Vector3 desiredPosition = target.position + rotation * offset; // Calculating the desired position

        // Check for obstructions inbetween the camera and the target using raycasting
        RaycastHit hit; // Store information about what the raycast hits
        if (Physics.Raycast(target.position, rotation * Vector3.back, out hit, distance, obstructionLayer))
        {
            // If there is an obstruction, calculate a slide position
            Vector3 slidePosition = hit.point + hit.normal * 0.5f; // Offset slightly from the wall to avoid the clipping

            // Maintain the camera height and the current vertical angle
            slidePosition.y = target.position.y + height; // Set the y position to the target's height plus offset

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