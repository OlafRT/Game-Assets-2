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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayRandomSounds());
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

        // Rotate the monster to face the bounce direction
        Quaternion targetRotation = Quaternion.LookRotation(bounceDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation

        // Instantiate the dust prefab at the current position
        Instantiate(dustPrefab, new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z), Quaternion.identity);

        // Play a random bounce sound
        PlayBounceSound();

        // Start the cooldown
        canBounce = false;
        StartCoroutine(BounceCooldown());
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
        hasPlayedRunAwaySound = false; // Reset the flag after cooldown to allow sound to play again if needed
    }

    private IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(randomSoundMinInterval, randomSoundMaxInterval));
            PlayRandomSound();
        }
    }

    private void PlayRandomSound()
    {
        if (randomSounds.Length > 0)
        {
            AudioClip soundToPlay = randomSounds[Random.Range(0, randomSounds.Length)];
            audioSource.PlayOneShot(soundToPlay);
        }
    }

    private IEnumerator SpinAndLaunchCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spinLaunchInterval);
            StartCoroutine(SpinAndLaunch());
        }
    }

    private IEnumerator SpinAndLaunch()
    {
        if (spinLaunchSound != null)
        {
            audioSource.PlayOneShot(spinLaunchSound); // Play the spin launch sound
        }

        float spinAngle = 360f / spinCount;
        for (int i = 0; i < spinCount; i++)
        {
            transform.Rotate(0, spinAngle, 0);
            yield return new WaitForSeconds(spinDuration / spinCount);
        }

        // Launch the prefab in all directions
        for (int i = 0; i < 10; i++)
        {
            Vector3 launchDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Instantiate(dustPrefab, transform.position, Quaternion.LookRotation(launchDirection));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check for collisions with the floor or walls
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Wall"))
        {
            // You can add additional logic here if needed
        }
    }

    private void FixedUpdate()
    {
        // Check for nearby DustMonsters
        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider nearbyMonster in nearbyMonsters)
        {
            if (nearbyMonster.gameObject != gameObject && nearbyMonster.gameObject.CompareTag("DustMonster"))
            {
                // Move towards the nearby monster
                Vector3 direction = (nearbyMonster.transform.position - transform.position).normalized;
                rb.velocity = direction * 2f;
            }
        }
    }
}