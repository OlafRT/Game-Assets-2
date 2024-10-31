using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("Player GameObjects")]
    [SerializeField] private GameObject firstPersonPlayer; // First person player GameObject
    [SerializeField] private GameObject tankPlayer; // Tank player GameObject

    [Header("Cameras")]
    [SerializeField] private Camera firstPersonCamera; // First person camera
    [SerializeField] private Camera tankCamera; // Tank camera

    [Header("UI Prompts")]
    public GameObject controlPrompt; // Control prompt UI
    public GameObject exitPrompt; // Exit prompt UI

    [Header("Mesh Collider")]
    public MeshCollider meshCollider; // Reference to the MeshCollider to enable/disable

    private TankController tankController; // Reference to the TankController script
    private Rigidbody tankRigidbody; // Reference to the Rigidbody component

    private void Awake()
    {
        ValidateComponents();
    }

    private void Start()
    {
        InitializePlayers();
        HideUI();
        GetTankComponents();
        DisableTankControls(); // Disable tank controls at the start

        // Ensure only the first-person camera is active at the start
        firstPersonCamera.gameObject.SetActive(true);
        tankCamera.gameObject.SetActive(false);
    }

    private void ValidateComponents()
    {
        if (firstPersonPlayer == null || tankPlayer == null || firstPersonCamera == null || tankCamera == null || controlPrompt == null || exitPrompt == null)
        {
            Debug.LogError("One or more required components are not assigned in the inspector.");
        }
    }

    private void InitializePlayers()
    {
        firstPersonPlayer.SetActive(true);
        tankPlayer.SetActive(true); // Keep the tank player GameObject active
    }

    private void HideUI()
    {
        controlPrompt.SetActive(false);
        exitPrompt.SetActive(false);
    }

    private void GetTankComponents()
    {
        tankController = tankPlayer.GetComponent<TankController>();
        tankRigidbody = tankPlayer.GetComponent<Rigidbody>();

        if (tankController == null)
        {
            Debug.LogError("TankController component not found on the tank player.");
        }

        if (tankRigidbody == null)
        {
            Debug.LogError("Rigidbody component not found on the tank player.");
        }
    }

    private void DisableTankControls()
    {
        if (tankController != null)
        {
            tankController.enabled = false; // Disable the TankController script
        }

        if (tankRigidbody != null)
        {
            tankRigidbody.isKinematic = true; // Disable physics by making the Rigidbody kinematic
        }
    }

    public void ShowPrompt()
    {
        controlPrompt.SetActive(true); // Show the control prompt
    }

    public void HidePrompt()
    {
        controlPrompt.SetActive(false); // Hide the control prompt
    }

    public void SwitchPlayer()
    {
        if (firstPersonPlayer.activeSelf)
        {
            SwitchToTankPlayer();
        }
        else
        {
            SwitchToFirstPersonPlayer();
        }
    }

    public void SwitchToTankPlayer()
    {
        firstPersonPlayer.SetActive(false);
        if (tankController != null)
        {
            tankController.enabled = true; // Enable the TankController script
        }

        if (tankRigidbody != null)
        {
            tankRigidbody.isKinematic = false; // Enable physics on the Rigidbody
        }

        // Switch cameras
        firstPersonCamera.gameObject.SetActive(false);
        tankCamera.gameObject.SetActive(true);

        // Parent the first-person player to the tank player
        firstPersonPlayer.transform.SetParent(tankPlayer.transform);

        // Manage UI prompts
        controlPrompt.SetActive(false); // Disable control prompt
        exitPrompt.SetActive(true); // Enable exit prompt

        // Disable the mesh collider when switching to the tank
        if (meshCollider != null)
        {
            meshCollider.enabled = false; // Disable the MeshCollider
        }
    }

    public void SwitchToFirstPersonPlayer()
    {
        tankPlayer.SetActive(true); // Ensure tank player is active
        if (tankController != null)
        {
            tankController.enabled = false; // Disable the TankController script
        }

        if (tankRigidbody != null)
        {
            tankRigidbody.isKinematic = true; // Disable physics by making the Rigidbody kinematic
        }

        firstPersonPlayer.SetActive(true); // Enable the first person player

        // Switch cameras
        firstPersonCamera.gameObject.SetActive(true);
        tankCamera.gameObject.SetActive(false);

        // Unparent the first-person player from the tank player
        firstPersonPlayer.transform.SetParent(null);

        // Manage UI prompts
        controlPrompt.SetActive(true); // Enable control prompt
        exitPrompt.SetActive(false); // Disable exit prompt

        // Enable the mesh collider when switching to the first-person player
        if (meshCollider != null)
        {
            meshCollider.enabled = true; // Enable the MeshCollider
        }

    }
}