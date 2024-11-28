using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target; // The target for the camera to follow

    [Header("Camera Settings")]
    public float distance = 5.0f; // Default distance from the target
    public float rotationSpeed = 5.0f; // Speed of rotation
    public float smoothSpeed = 0.1f; // Smoothness factor for camera movement
    public LayerMask obstructionLayer; // Layer for walls and obstructions
    public float wallOffset = 0.5f; // Offset to keep the camera away from walls

    [Header("Vertical Rotation Limits")]
    public float minVerticalAngle = -30.0f; // Minimum vertical angle
    public float maxVerticalAngle = 30.0f; // Maximum vertical angle

    [Header("Zoom Settings")]
    public float minDistance = 2.0f; // Minimum distance for zooming in
    public float maxDistance = 10.0f; // Maximum distance for zooming out
    public float zoomSpeed = 2f; // Speed of zooming in/out
    private float targetDistance; // Distance to the target camera position

    private float currentAngle; // Current horizontal angle around the target
    private float currentVerticalAngle; // Current vertical angle of the camera

    void Start()
    {
        currentAngle = transform.eulerAngles.y; // Initialize the current angle
        currentVerticalAngle = 0.0f; // Initialize vertical angle
        targetDistance = distance; // Initialize target distance
    }

    void LateUpdate()
    {
        HandleRotation(); // Handle camera rotation based on input
        HandleZoom(); // Handle zooming in and out
        UpdateCameraPosition(); // Update the camera position
    }

    void HandleRotation()
    {
        // Get mouse input for controlling the camera rotation
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        currentAngle += mouseX; // Update the horizontal angle

        // Update the vertical angle and clamp it
        currentVerticalAngle -= mouseY; // Invert for natural movement
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle); // Clamp vertical angle
    }

    void HandleZoom()
    {
        // Check if the right mouse button is pressed for zooming in
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            targetDistance -= zoomSpeed * Time.deltaTime; // Zoom in
        }
        else
        {
            targetDistance += zoomSpeed * Time.deltaTime; // Zoom out
        }

        // Clamp the target distance to ensure it stays within min and max bounds
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

        // Update the distance variable to smoothly transition
        distance = Mathf.Lerp(distance, targetDistance, smoothSpeed);
    }

    void UpdateCameraPosition()
    {
        // Calculate the desired position based on the target's position and the current angles
        Vector3 offset = new Vector3(0, 0, -distance); // Offset from the target position (no height adjustment)
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentAngle, 0); // Rotation based on angles
        Vector3 desiredPosition = target.position + rotation * offset; // Calculating the desired position

        // Check for obstructions between the camera and the target
        Vector3 direction = (desiredPosition - target.position).normalized; // Direction from target to desired position
        float distanceToTarget = Vector3.Distance(desiredPosition, target.position);

        // Raycast to check for walls in the direction of the desired position
        if (Physics.Raycast(target.position, direction, out RaycastHit hit, distanceToTarget, obstructionLayer))
        {
            // If we hit something, adjust the position to be closer to the target
            desiredPosition = hit.point + hit.normal * wallOffset; // Offset slightly from the wall
        }

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.LookAt(target.position); // Look at the target
    }
}