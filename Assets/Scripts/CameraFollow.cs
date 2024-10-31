using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The tank to follow
    public float distance = 5.0f; // Distance from the tank
    public float height = 2.0f; // Height above the tank
    public float rotationSpeed = 5.0f; // Speed of rotation
    public float verticalRotationLimit = 30.0f; // Limit for vertical rotation

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
        // Calculate the new position
        Vector3 offset = new Vector3(0, height, -distance);
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentAngle, 0);
        Vector3 targetPosition = target.position + rotation * offset;

        // Set the camera position and look at the target
        transform.position = targetPosition;
        transform.LookAt(target.position + Vector3.up * height); // Adjust look at to account for height
    }
}
