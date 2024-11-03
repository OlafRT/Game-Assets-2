using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustMonster : MonoBehaviour
{
    public Transform player; // Assign the player in the inspector
    public GameObject dustPrefab; // Assign the prefab in the inspector
    public float bounceForce = 5f; // The force of the bounce
    public float bounceDistance = 3f; // Distance to move away from the player
    public float bounceCooldown = 2f; // Cooldown time between bounces
    public float maxDistanceFromPlayer = 10f; // Maximum distance to run from the player
    public float turnAmount = 15f; // Maximum angle to turn when bouncing
    public AudioClip[] bounceSounds; // Array of sounds for bouncing
    public AudioClip[] randomSounds; // Array of random sounds
    public AudioClip runAwaySound; // Sound to play when running away
    public AudioClip spinLaunchSound; // Sound to play when spinning and launching
    public float randomSoundMinInterval = 10f; // Minimum interval for random sounds
    public float randomSoundMaxInterval = 30f; // Maximum interval for random sounds
    public float spinLaunchInterval = 120f; // Interval for spinning and launching
    public float spinDuration = 5f; // Duration of the spin
    public int spinCount = 5; // Number of spins

    private Rigidbody rb;
    private AudioSource audioSource;
    private bool canBounce = true;
    private bool hasPlayedRunAwaySound = false; // Flag to track if the run away sound has been played

    // Scaling variables
    public float jumpScaleFactor = 1.5f; // Factor to scale up on jump (make it taller)
    public float landScaleFactor = 0.75f; // Factor to scale down on landing (make it shorter)
    public float scaleSpeed = 5f; // Speed of scaling
    public float spawnRadius = 10f; // Radius for spawning dust prefabs
    public int maxDustPrefabs = 20; // Maximum number of dust prefabs to spawn

    private Vector3 originalScale; // Store the original scale of the monster
    private int currentDustCount = 0; // Track the number of dust prefabs spawned

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale; // Store the original scale
        StartCoroutine(PlayRandomSounds()); // Start playing random sounds
        StartCoroutine(SpinAndLaunchCoroutine());
    }

    void Update()
    {
        if (canBounce && player != null)
        {
            // Check the distance from the player
            float distanceFromPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceFromPlayer < maxDistanceFromPlayer)
            {
                BounceAwayFromPlayer();
            }
        }
    }

    private void BounceAwayFromPlayer()
    {
        // Play the run away sound only once when the monster escapes
        if (!hasPlayedRunAwaySound && runAwaySound != null)
        {
            audioSource.PlayOneShot(runAwaySound); // Play the run away sound
            hasPlayedRunAwaySound = true; // Set the flag to true to prevent playing again
        }

        Vector3 direction = (transform.position - player.position).normalized; // Get the direction away from the player

        // Add some randomness to the bounce direction
        float randomTurn = Random.Range(-turnAmount, turnAmount);
        Quaternion rotation = Quaternion.Euler(0, randomTurn, 0);
        Vector3 bounceDirection = rotation * direction * bounceDistance; // Calculate the bounce direction with rotation

        // Apply the bounce force
        rb.velocity = new Vector3(bounceDirection.x, bounceForce, bounceDirection.z);

        // Scale up during the jump
        StartCoroutine(ScaleOverTime(new Vector3(originalScale.x, originalScale.y * jumpScaleFactor, originalScale.z), scaleSpeed));

        // Rotate the monster to face the bounce direction smoothly
        Quaternion targetRotation = Quaternion.LookRotation(bounceDirection, Vector3.up);
        StartCoroutine(SmoothRotate(targetRotation, 0.5f)); // Use a coroutine for smooth rotation

        // Instantiate the dust prefab at the current position
        if (currentDustCount < maxDustPrefabs)
        {
            Instantiate(dustPrefab, new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z), Quaternion.identity);
            currentDustCount++;
        }

        // Play a random bounce sound
        PlayBounceSound();

        // Start the cooldown
        canBounce = false;
        StartCoroutine(BounceCooldown());
    }

    private IEnumerator SmoothRotate(Quaternion targetRotation, float duration)
    {
        float timeElapsed = 0f;
        Quaternion initialRotation = transform.rotation;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation; // Ensure we end at the target rotation
    }

    private void PlayBounceSound()
    {
        if (bounceSounds.Length > 0)
        {
            AudioClip soundToPlay = bounceSounds[Random.Range(0, bounceSounds.Length)];
            audioSource.PlayOneShot(soundToPlay);
        }
    }

    private IEnumerator BounceCooldown()
    {
        yield return new WaitForSeconds(bounceCooldown);
        canBounce = true;
    }

    private IEnumerator ScaleOverTime(Vector3 targetScale, float speed)
    {
        float timeElapsed = 0f;
        Vector3 initialScale = transform.localScale;

        while (timeElapsed < 1f)
        {
            float t = timeElapsed;
            t = t * t * (3f - 2f * t); // Smoothstep function
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            timeElapsed += Time.deltaTime * speed;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator SpinAndLaunchCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spinLaunchInterval);
            SpinAndLaunch();
        }
    }

    private void SpinAndLaunch()
    {
        // Play the spin launch sound
        if (spinLaunchSound != null)
        {
            audioSource.PlayOneShot(spinLaunchSound);
        }

        // Spin the monster
        StartCoroutine(SpinOverTime(spinDuration));

        // Launch the prefab in all directions
        for (int i = 0; i < 10; i++)
        {
            Vector3 launchDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Instantiate(dustPrefab, transform.position, Quaternion.LookRotation(launchDirection));
        }
    }

    private IEnumerator SpinOverTime(float duration)
    {
        float timeElapsed = 0f;
        float spinAmount = spinCount * 360f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float spin = spinAmount * t;
            transform.Rotate(Vector3.up, spin);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(randomSoundMinInterval, randomSoundMaxInterval));
            if (randomSounds.Length > 0)
            {
                AudioClip soundToPlay = randomSounds[Random.Range(0, randomSounds.Length)];
                audioSource.PlayOneShot(soundToPlay);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the monster has landed
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Scale down on landing
            StartCoroutine(ScaleOverTime(new Vector3(originalScale.x, originalScale.y * landScaleFactor, originalScale.z), scaleSpeed));

            // Wait for a short delay before scaling back to the original size
            Invoke("ScaleBackToOriginal", 0.5f);
        }
    }

    private void ScaleBackToOriginal()
    {
        StartCoroutine(ScaleOverTime(originalScale, scaleSpeed));
    }
}