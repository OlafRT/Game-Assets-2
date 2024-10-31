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
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveForward();
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void MoveForward()
    {
        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            TurnAround();
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









