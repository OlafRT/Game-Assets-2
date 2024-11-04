using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.Video; // For VideoPlayer

public class RemoteControl : MonoBehaviour
{
    public GameObject tv; // Reference to the TV GameObject
    public TMP_Text uiText; // Reference to the UI TextMeshPro component
    private bool isPlayerInRange = false;
    private bool isRemoteInRange = false;
    private bool isTVOn = false; // Track the state of the TV

    void Start()
    {
        uiText.gameObject.SetActive(false); // Hide the GameObject containing the UI text at the start
    }

    void Update()
    {
        // Check if the player is in range of both the TV and the remote and presses 'E'
        if (isPlayerInRange && isRemoteInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isTVOn)
            {
                TurnOffTV(); // Turn off the TV if it's currently on
            }
            else
            {
                TurnOnTV(); // Turn on the TV if it's currently off
            }
        }
    }

    private void TurnOnTV()
    {
        VideoPlayer videoPlayer = tv.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.Play(); // Play the video when turning on the TV
            isTVOn = true; // Update the TV state
            UpdateUIText(); // Update the UI text
        }
    }

    private void TurnOffTV()
    {
        VideoPlayer videoPlayer = tv.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.Pause(); // Pause the video when turning off the TV
            isTVOn = false; // Update the TV state
            UpdateUIText(); // Update the UI text
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered TV range.");
            UpdateUIText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited TV range.");
            UpdateUIText();
        }
    }

    public void UpdateUIText()
    {
        if (isPlayerInRange && isRemoteInRange)
        {
            if (isTVOn)
            {
                uiText.text = "Press E to turn the TV off"; // Update text for turning off
            }
            else
            {
                uiText.text = "Press E to turn on the TV"; // Update text for turning on
            }
            uiText.gameObject.SetActive(true); // Enable the entire GameObject that contains the TextMeshPro component
        }
        else
        {
            uiText.gameObject.SetActive(false); // Disable the entire GameObject if not in range
        }
    }

    public void SetRemoteInRange(bool inRange)
    {
        isRemoteInRange = inRange;
        UpdateUIText(); // Update the UI text based on the remote's range
    }
}