using UnityEngine;

public class SyncFirstPersonModel : MonoBehaviour
{
    public Animator fullModelAnimator; // Animator component of the full character that we can see in the mirror
    public Animator firstPersonArmsAnimator; // Animator component of the first-person arms

    void Update()
    {
        // Sync Animator states
        SyncAnimationStates();
    }

    void SyncAnimationStates()
    {
        // Transfer the current state of the full character's animator to the first-person arms animator, The fullPathHash is used to identify the animation state and normalizedTime is used to start the animation at the same point as the full model
        firstPersonArmsAnimator.Play(fullModelAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, fullModelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        
        // Sync parameters (like speed)
        float speed = fullModelAnimator.GetFloat("Speed");
        firstPersonArmsAnimator.SetFloat("Speed", speed);
    }
}