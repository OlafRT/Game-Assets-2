using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour

    // This script is working as intended and doesn't really need any work to improve it.

{
    public PlayerSwitcher playerSwitcher; // PlayerSwitcher script that will be assigned in the inspector,  this script is responsible for switching between the player mode


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure player has the tag "Player"
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
        if (Input.GetKeyDown(KeyCode.E)) // Check for input to switch player mode when the appropriate prompt is active
        {
            if (playerSwitcher.controlPrompt.activeSelf || playerSwitcher.exitPrompt.activeSelf)
            {
                playerSwitcher.SwitchPlayer(); // Switch players if either prompt is visible
            }
        }
    }
}