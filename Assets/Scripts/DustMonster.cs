using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // This script is a total mess,  I know. I'm just trying to get something working here. I'll clean it up later. Maybe. Eventually. Hopefully. Please don't judge me.  

public class DustMonster : MonoBehaviour // It's a Dust Monster!
{
    public Transform player; // Transform of the player (The monster wants to run away from them!)
    public GameObject dustPrefab; // Prefab that the monster will spawn when it jumps and when it does its spin move
    public float bounceForce = 15f; // How high our monster can jump, like a super bouncy ball!
    public float bounceDistance = 40f; // Distance to jump/bounce every leap.
    public float bounceCooldown = 2f; // Cooldown between bounces, so when it lands it takes some time before bouncing again.
    public float maxDistanceFromPlayer = 150f; // Maximum distance to run away from the player before stopping.
    public float turnAmount = 100f; // Maximum angle to turn when the monster is bouncing
    public float rotationSpeed = 180f; // Speed of rotation in degrees per second
    public AudioClip[] bounceSounds; // Array for sounds of the bouncing
    public AudioClip[] randomSounds; // Array for random sounds (laughing or chittering... etc.)
    public AudioClip runAwaySound; // Sound to play when running away
    public AudioClip spinLaunchSound; // Sound to play when spinning and launching
    public float randomSoundMinInterval = 10f; // Minimum interval for playing the random sounds
    public float randomSoundMaxInterval = 30f; // Maximum interval for playing the random sounds
    public float spinLaunchInterval = 120f; // Interval for doing the spinning and launching move
    public float spinDuration = 5f; // Duration of the spin move
    public int spinCount = 5; // Number of spins in the spin move
    public float jumpScaleFactor = 1.5f; // How much to scale up on the jump (make it taller basically)
    public float landScaleFactor = 0.75f; // How much to scale down on landing (make it shorter, basically!!)
    public float scaleSpeed = 5f; // Speed of the scaling to make it smoother
    public float spawnRadius = 10f; // Radius for spawning the dust prefabs
    public int maxDustPrefabs = 20; // Maximum number of dust prefabs to spawn

    private Rigidbody rb; // The Rigidbody component
    private AudioSource audioSource; // The AudioSource component
    private bool canBounce = true; // Flag to check if the monster can bounce
    private bool hasPlayedRunAwaySound = false; // Flag to track if the run away sound has been played
    private Vector3 originalScale; // Storing the original scale of the monster
    private int currentDustCount = 0; // Tracking the number of dust prefabs spawned
    private Quaternion targetRotation; // Target rotation for smooth turning

    void Start() // Monster wakes up and gets ready to play!
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        originalScale = transform.localScale; // Store the original scale

        // Find the player GameObject by tag and assign its Transform
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform; // Assign the Transform of the player
        }
        else
        {
            Debug.LogWarning("Player GameObject not found. Please ensure it has the 'Player' tag.");
        }

        StartCoroutine(PlayRandomSounds()); // Start the coroutine for playing random sounds
        StartCoroutine(SpinAndLaunchCoroutine()); // Start the coroutine for the spin and launch move
        targetRotation = transform.rotation; // Initialize target rotation
    }

    void Update() // Hmmm... The monster looks around and decides what to do!
    {
        if (canBounce && player != null) // If our monster can jump and there's a mean player...
        {
            // Check the distance from the player
            float distanceFromPlayer = Vector3.Distance(transform.position, player.position); // Measure how far the monster is from the player
            if (distanceFromPlayer < maxDistanceFromPlayer) // If the monster is too close to the player
            {
                BounceAwayFromPlayer(); // Call your bounce logic to move away from the player
            }
        }

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void BounceAwayFromPlayer() // Monster starts jumping away!
    {
        // Play the run away sound only once when the monster escapes
        if (!hasPlayedRunAwaySound && runAwaySound != null) // If we haven't played the sound yet and it exists
        {
            audioSource.PlayOneShot(runAwaySound); // Play the run away sound
            hasPlayedRunAwaySound = true; // Set to true to prevent playing again
        }

        // Calculate the direction to bounce away from the player
        Vector3 direction = (transform.position - player.position).normalized; // Get the direction away from the player

        // Add some randomness to the bounce direction
        float randomTurn = Random.Range(-turnAmount, turnAmount); // Selects a random number between a negative and a possitve turnAmount to add to the turn
        Quaternion rotation = Quaternion.Euler(0, randomTurn, 0); // Adds the random number to the rotation and rotate it
        Vector3 bounceDirection = rotation * direction * bounceDistance; // Calculate the bounce direction with rotation

        // Apply the bounce force
        rb.velocity = new Vector3(bounceDirection.x, bounceForce, bounceDirection.z);  // Do the bounce force and direction to the monster's Rigidbody

        StartCoroutine(ScaleOverTime(new Vector3(originalScale.x, originalScale.y * jumpScaleFactor, originalScale.z), scaleSpeed)); // Make monster stretchy when jumping

        Quaternion targetRotation = Quaternion.LookRotation(bounceDirection, Vector3.up);  // Rotate the monster to face the bounce direction smoothly
        StartCoroutine(SmoothRotate(targetRotation, 0.5f)); // Use a coroutine for smooth rotation

        // Instantiate the dust prefab at the current position
        if (currentDustCount < maxDustPrefabs) // Just makes sure we don't have too many dust prefabs nearby...
        {
            // Make a dust prefab at the location of the jump but a little closer to the floor
            Instantiate(dustPrefab, new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z), Quaternion.identity); 
            currentDustCount++; // Add it to the current dust count
        }

        // Play random bounce sound (start the playbouncesound)
        PlayBounceSound();

        // Start the cooldown of the bouncing
        canBounce = false;
        StartCoroutine(BounceCooldown());
    }

    private IEnumerator SmoothRotate(Quaternion targetRotation, float duration)  // Smooth rotation coroutine, we use the targetrotation over the set duration
    {
        float timeElapsed = 0f; // Here we start to track how much time has passed
        Quaternion initialRotation = transform.rotation;  // Store the initial rotation of the monster to start at

        while (timeElapsed < duration)  // Rotate until the duration is reached
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeElapsed / duration); // Rotation smoothly between the initial and target rotations
            timeElapsed += Time.deltaTime; // Increase the timeElapsed by time that has passed since last deltatime/frame
            yield return null; // Wait for next frame before continuing
        }

        transform.rotation = targetRotation; // Ensure we end at the target rotation
    }

    private void PlayBounceSound() // Playing the sound when the monster bounces
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

        // Check if the monster has hit a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            TurnAround();
        }
    }

    private void TurnAround()
    {
        // Random angle between -30 and 30 degrees
        float randomAngle = Random.Range(-30f, 30f);

        // Rotation to 180 degrees plus the random angle
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f + randomAngle, 0);
    }

    private void ScaleBackToOriginal()
    {
        StartCoroutine(ScaleOverTime(originalScale, scaleSpeed));
    }
}