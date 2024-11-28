using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public TankController tankController; // Reference to the TankController

    private void Start()
    {
        // Get the TankController component from the parent object
        tankController = GetComponentInParent<TankController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a "Hardstop" object
        if (collision.gameObject.CompareTag("Hardstop"))
        {
            // Notify the TankController to stop movement
            if (tankController != null)
            {
                tankController.SetCollisionState(true);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the exit is from a "Hardstop" object
        if (collision.gameObject.CompareTag("Hardstop"))
        {
            // Notify the TankController that it's no longer colliding
            if (tankController != null)
            {
                tankController.SetCollisionState(false);
            }
        }
    }
}
