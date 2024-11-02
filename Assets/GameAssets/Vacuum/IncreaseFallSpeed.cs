using UnityEngine;
public class IncreaseFallSpeed : MonoBehaviour
{
    public float extraGravityForce = 10f; // Adjust this value as needed
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * extraGravityForce, ForceMode.Acceleration);
    }
}
