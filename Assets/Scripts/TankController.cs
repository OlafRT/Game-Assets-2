using UnityEngine;
using UnityEngine.UI; // Add this to use UI elements

public class TankController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f; // Forward/Backward movement speed
    public float sprintMultiplier = 2f; // Sprint speed multiplier
    public float sprintDuration = 10f; // Duration of the sprint
    public float sprintCooldown = 5f; // Cooldown time after sprinting
    public float rotationSpeed = 200f; // Rotation speed

    // UI Elements
    public Image cooldownFill; // Reference to the cooldown fill image

    // References to the GameObjects and their scripts
    public GameObject object1; // First GameObject
    public GameObject object2; // Second GameObject
    private Rotation script1; // The script you want to toggle on object1
    private Rotation script2; // The script you want to toggle on object2

    private float currentSpeed; // Current movement speed
    private float currentSprint; // Current sprint value
    private bool isSprinting = false; // Flag to check if the tank is sprinting
    private bool isScriptEnabled = false; // Track if the script is enabled
    private bool isCollidingWithWall = false; // Track if the tank is colliding with a wall

    private void Start()
    {
        currentSpeed = speed; // Initialize current speed
        currentSprint = sprintDuration; // Start with full sprint
        cooldownFill.fillAmount = 1; // Initialize cooldown fill to full

        // Get the scripts from the GameObjects
        script1 = object1.GetComponent<Rotation>(); // Assuming the script is called Rotation
        script2 = object2.GetComponent<Rotation>(); // Assuming the script is called Rotation
    }

    private void Update()
    {
        // Check for "R" key press to toggle the rotation script on the bristles
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleScripts();
        }

        // Get input for movement
        float moveVertical = Input.GetAxis("Vertical"); // Forward/backward movement
        float moveHorizontal = Input.GetAxis("Horizontal"); // Left/right rotation

        // Check for sprinting
        if (Input.GetKey(KeyCode.LeftShift) && currentSprint > 0)
        {
            StartSprinting();
        }
        else
        {
            StopSprinting();
        }

        // Regenerate sprint when not sprinting
        RegenerateSprint();

        // Update UI fill amount
        UpdateCooldownUI();

        // Move the tank forward/backward only if not colliding with a wall
        if (!isCollidingWithWall)
        {
            MoveTank(moveVertical);
        }

        // Rotate the tank
        RotateTank(moveHorizontal);
    }

    private void ToggleScripts()
    {
        isScriptEnabled = !isScriptEnabled; // Toggle the state

        if (script1 != null)
        {
            script1.enabled = isScriptEnabled; // Enable/disable the script on object1
        }

        if (script2 != null)
        {
            script2.enabled = isScriptEnabled; // Enable/disable the script on object2
        }

        Debug.Log("Scripts " + (isScriptEnabled ? "Enabled" : "Disabled"));
    }

    private void StartSprinting()
    {
        if (!isSprinting)
        {
            isSprinting = true;
            currentSpeed = speed * sprintMultiplier; // Increase speed
        }

        // Decrease current sprint
        currentSprint -= Time.deltaTime; // Decrease current sprint
        currentSprint = Mathf.Max(currentSprint, 0); // Clamp to zero
        cooldownFill.fillAmount = currentSprint / sprintDuration; // Update UI fill amount

        // Stop sprinting if out of sprint
        if (currentSprint <= 0)
        {
            StopSprinting();
        }
    }

    private void StopSprinting()
    {
        if (isSprinting)
        {
            isSprinting = false;
            currentSpeed = speed; // Reset speed to normal
        }
    }

    private void RegenerateSprint()
    {
        if (!isSprinting)
        {
 currentSprint += Time.deltaTime; // Regenerate sprint
            currentSprint = Mathf.Min(currentSprint, sprintDuration); // Clamp to max sprint duration
            cooldownFill.fillAmount = currentSprint / sprintDuration; // Update UI fill amount
        }
    }

    private void UpdateCooldownUI()
    {
        cooldownFill.fillAmount = currentSprint / sprintDuration; // Update UI fill amount
    }

    private void MoveTank(float moveVertical)
    {
        transform.Translate(Vector3.forward * moveVertical * currentSpeed * Time.deltaTime);
    }

    private void RotateTank(float moveHorizontal)
    {
        transform.Rotate(Vector3.up * moveHorizontal * rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hardstop")
        {
            isCollidingWithWall = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Hardstop")
        {
            isCollidingWithWall = false;
        }
    }
}