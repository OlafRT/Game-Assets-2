using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour

    // This script has a kind of misleading name, as it doesn’t really have anything to do with a button, but the way it works is that when the player is within a trigger zone, 
    // it enables a text element on the canvas that tells you to press “E” to activate. It checks for if the E key is pressed, and then does the SwitchPlayer function on the PlayerSwitcher script. 
    // The reason for it being called a button is that it is supposed to be in the area of a button on the control panel that takes over the controls of the robot vacuum.
    // This script is working as intended and doesn't really need any work to improve it.

{
    public PlayerSwitcher playerSwitcher; // PlayerSwitcher script. Assign in the inspector.  This script is responsible for switching between the player mode


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