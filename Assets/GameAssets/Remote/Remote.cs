using UnityEngine;

public class Remote : MonoBehaviour
{
    private RemoteControl remoteControl;

    void Start()
    {
        remoteControl = FindObjectOfType<RemoteControl>(); // Find the RemoteControl script in the scene
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered remote range.");
            remoteControl.SetRemoteInRange(true); // Notify the RemoteControl that the player is in range of the remote
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited remote range.");
            remoteControl.SetRemoteInRange(false); // Notify the RemoteControl that the player is no longer in range of the remote
        }
    }
}