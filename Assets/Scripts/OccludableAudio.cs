using UnityEngine;

public class OccludableAudio : MonoBehaviour
{
    private AudioSource m_Source;

    public Transform Listener;
    public float OccludedVolume = 0.0f; // Volume when occluded
    public float FullVolume = 1.0f; // Volume when not occluded
    public float FadeSpeed = 10.0f; // Speed of volume change
    public LayerMask Mask; // Layer mask for occlusion

    void Start()
    {
        m_Source = GetComponent<AudioSource>();
        m_Source.volume = FullVolume; // Start at full volume
    }

    void Update()
    {
        // Check if the audio source is occluded by a wall
        bool isOccluded = Physics.Linecast(Listener.position, transform.position, Mask);

        // Determine target volume based on occlusion
        float targetVolume = isOccluded ? OccludedVolume : FullVolume;

        // Smoothly transition to the target volume
        m_Source.volume = Mathf.Lerp(m_Source.volume, targetVolume, Time.deltaTime * FadeSpeed);
    }
}