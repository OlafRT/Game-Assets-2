using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ImpulseTrigger : MonoBehaviour //  This script will be attached to the Impulse Trigger object

{
    public CinemachineImpulseSource impulseSource; //  Reference to the Cinemachine Impulse Source component

    public float delay = 1.0f; //   Delay before the impulse is applied


    private bool isWaitingForEnterImpulse = false; //  Flag to check if we are waiting for the impulse to be applied

    private bool isWaitingForExitImpulse = false; //   Flag to check if we are waiting for the impulse to be applied


    private void OnTriggerEnter(Collider other) //  Trigger entered

    {
        // If waiting for enter impulse, ignore new triggers
        if (!isWaitingForEnterImpulse) //   If we are not waiting for the impulse to be applied on enter

        {
            isWaitingForEnterImpulse = true; //    Set the flag to true

            Invoke(nameof(TriggerEnterImpulse), delay); // Invoke the delay of the trigger impulse
        }
    }

    private void OnTriggerExit(Collider other) //   Trigger exited

    {
        // If waiting for exit impulse, ignore new triggers
        if (!isWaitingForExitImpulse) //    If we are not waiting for the impulse to be applied on exit

        {
            isWaitingForExitImpulse = true; //    Set the flag to true

            Invoke(nameof(TriggerExitImpulse), delay); // Invoke delay of the trigger impulse
        }
    }

    private void TriggerEnterImpulse()
    {
        impulseSource.GenerateImpulse(); //  Generate impulse when entering trigger

        isWaitingForEnterImpulse = false; // Reset after triggering
    }

    private void TriggerExitImpulse()
    {
        impulseSource.GenerateImpulse(); //   Generate impulse when exiting trigger

        isWaitingForExitImpulse = false; // Reset after triggering
    }
}
