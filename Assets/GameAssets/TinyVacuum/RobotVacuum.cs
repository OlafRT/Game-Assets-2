using UnityEngine;

public class RobotVacuum : MonoBehaviour
{
    public float moveSpeed = 2f;           // Speed!
    public float rotationSpeed = 180f;      // How fast it rotates in degrees per second!
    public bool isMoving = true;            // Is it moving?

    private Rigidbody rb;                   // Reference to the Rigidbody
    private Quaternion targetRotation;      // Target rotation for smooth turning

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // getting the rigidbody
        targetRotation = transform.rotation;  // setting the target rotation to the current rotation

    }

    void Update()  // This is called every frame

    {
        if (isMoving)   // If it's moving

        {
            MoveForward();   // Move forward

        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);  // Rotate towards the target rotation
    }

    void MoveForward() 
    {
        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);  // Move forward at the speed

    }

    void OnCollisionEnter(Collision collision)   // When it collides with something
    {
        if (collision.gameObject.CompareTag("Wall"))   // If it hits a wall
        {
            TurnAround();   // Turn around... Kinda obvious?
        }
    }

    private void TurnAround() 
    {
        // Random angle between -30 and 30 degrees
        float randomAngle = Random.Range(-30f, 30f);
        
        // Rotation to 180 degrees plus the random angle
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f + randomAngle, 0);
    }
}









