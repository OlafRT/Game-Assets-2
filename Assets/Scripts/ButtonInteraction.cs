using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    public PlayerSwitcher playerSwitcher; // Assign the PlayerSwitcher script in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure your player has the tag "Player"
        {
            playerSwitcher.ShowPrompt(); // Show the control prompt when entering the zone
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSwitcher.HidePrompt(); // Hide the control prompt when exiting the zone
            playerSwitcher.exitPrompt.SetActive(false); // Hide the exit prompt when leaving
        }
    }

    private void Update()
    {
        // Check for input to switch players when the appropriate prompt is active
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerSwitcher.controlPrompt.activeSelf || playerSwitcher.exitPrompt.activeSelf)
            {
                playerSwitcher.SwitchPlayer(); // Switch players if either prompt is visible
            }
        }
    }
}
