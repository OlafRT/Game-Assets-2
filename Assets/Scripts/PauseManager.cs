using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuCanvas;

    private bool isPaused = false;
    private AudioSource[] allAudioSources;
    private ParticleSystem[] allParticleSystems;
    private VideoPlayer[] allVideoPlayers;

    // Dictionary to keep track of video player's play state
    private Dictionary<VideoPlayer, bool> videoPlayerPlayStates = new Dictionary<VideoPlayer, bool>();

    void Start()
    {
        // Initially hide the pause menu
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        // Find all audio sources, particle systems, and video players in the scene
        allAudioSources = FindObjectsOfType<AudioSource>();
        allParticleSystems = FindObjectsOfType<ParticleSystem>();
        allVideoPlayers = FindObjectsOfType<VideoPlayer>();

        // Initialize the video player play states
        foreach (var videoPlayer in allVideoPlayers)
        {
            videoPlayerPlayStates[videoPlayer] = false; // Assume all are initially not playing
        }
    }

    void Update()
    {
        // Check for input to toggle pause state
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1; // Pause/unpause time

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        // Pause or resume other game elements
        PauseOrResumeAudio(isPaused);
        PauseOrResumeParticles(isPaused);
        PauseOrResumeVideos(isPaused);

        // Show or hide pause menu canvas
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(isPaused);
    }

    void PauseOrResumeAudio(bool pause)
    {
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (pause)
                audioSource.Pause();
            else
                audioSource.UnPause();
        }
    }

    void PauseOrResumeParticles(bool pause)
    {
        foreach (ParticleSystem particleSystem in allParticleSystems)
        {
            if (pause)
                particleSystem.Pause();
            else
                particleSystem.Play();
        }
    }

    void PauseOrResumeVideos(bool pause)
    {
        foreach (VideoPlayer videoPlayer in allVideoPlayers)
        {
            if (pause)
            {
                // Store the current play state before pausing
                videoPlayerPlayStates[videoPlayer] = videoPlayer.isPlaying;
                videoPlayer.Pause();
            }
            else
            {
                // Resume only if it was playing before pause
                if (videoPlayerPlayStates[videoPlayer])
                {
                    videoPlayer.Play();
                }
            }
        }
    }

    public void ExitGame()
    {
        // Code to exit the game
        Application.Quit();
    }
}

