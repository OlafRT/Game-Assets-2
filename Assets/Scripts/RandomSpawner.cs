using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Array of prefabs to spawn
    public float spawnInterval = 15f; // Time interval between spawns
    public GameObject plane; // The plane GameObject to use for spawning
    public float spawnHeightOffset = 2f; // Height above the plane to spawn objects

    private void Start()
    {
        // Start the spawning process
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }

    private void SpawnObject()
    {
        if (plane == null)
        {
            Debug.LogWarning("No plane assigned for spawning!!!!!");
            return;
        }

        // Get the plane's dimensions
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogWarning("The assigned plane does not have a Renderer component... Moron!");
            return;
        }

        // Calculate the size of the plane
        float planeWidth = planeRenderer.bounds.size.x;
        float planeDepth = planeRenderer.bounds.size.z;

        // Calculate the center position of the plane
        Vector3 planePosition = plane.transform.position;

        // Generate random x and z coordinates within the plane's dimensions
        float randomX = Random.Range(planePosition.x - (planeWidth / 2f), planePosition.x + (planeWidth / 2f));
        float randomZ = Random.Range(planePosition.z - (planeDepth / 2f), planePosition.z + (planeDepth / 2f));
        
        // Set the Y position to be above the plane
        float spawnY = planePosition.y + spawnHeightOffset; // Adjust spawn height above the plane
        Vector3 spawnPosition = new Vector3(randomX, spawnY, randomZ); // Use the calculated Y position

        // Choose a random object from the array
        if (objectsToSpawn.Length > 0)
        {
            int randomIndex = Random.Range(0, objectsToSpawn.Length);
            GameObject objectToSpawn = objectsToSpawn[randomIndex];

            // Instantiate the object at the random position
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No objects to spawn assigned in the array. So... you know, maybe put some?");
        }
    }
}
