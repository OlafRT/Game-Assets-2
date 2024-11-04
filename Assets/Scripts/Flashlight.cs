using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // Super simple script, just activates a spotlight when you press F

public class Flashlight : MonoBehaviour
{
    public GameObject spotlight;  // Public variable to assign the spotlight in the inspector

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the spotlight is initially disabled
        if (spotlight != null)
        {
            spotlight.SetActive(false); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if F key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Turn the damn light on!!!
            if (spotlight != null)
            {
                spotlight.SetActive(!spotlight.activeSelf);
            }
        }
    }
}
