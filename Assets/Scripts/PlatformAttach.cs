using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        // Check if the player entered the platform's collider
        if (other.CompareTag("Player")) {
            // Make the player a child of the platform
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        // Check if the player exited the platform's collider
        if (other.CompareTag("Player")) {
            // Remove the player from the platform's parent
            other.transform.SetParent(null);
        }
    }
}
