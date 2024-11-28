using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInAndOutText : MonoBehaviour
{
    public Text textToFade;
    public float fadeInDuration = 2f;
    public float fadeOutDelay = 10f;
    public float fadeOutDuration = 2f;

    private float startTime;
    private bool fadeInComplete = false;
    private bool fadeOutStarted = false;

    void Start()
    {
        startTime = Time.time;
        Color startColor = textToFade.color;
        startColor.a = 0f;
        textToFade.color = startColor;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;

        if (!fadeInComplete)
        {
            if (elapsedTime < fadeInDuration)
            {
                // Calculate alpha value based on time
                float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);

                // Fade in the text
                Color textColor = textToFade.color;
                textColor.a = alpha;
                textToFade.color = textColor;
            }
            else
            {
                fadeInComplete = true;
                startTime = Time.time + fadeOutDelay; // Start delay for fade-out
            }
        }
        else if (!fadeOutStarted)
        {
            if (elapsedTime >= fadeOutDelay)
            {
                fadeOutStarted = true;
                startTime = Time.time; // Start fade-out immediately
            }
        }
        else
        {
            // Calculate alpha value for fade-out
            float fadeOutElapsedTime = Time.time - startTime;
            if (fadeOutElapsedTime < fadeOutDuration)
            {
                float alpha = 1 - Mathf.Clamp01(fadeOutElapsedTime / fadeOutDuration);

                // Fade out the text
                Color textColor = textToFade.color;
                textColor.a = alpha;
                textToFade.color = textColor;
            }
            else
            {
                // Fade-out complete, destroy the text object
                Destroy(gameObject);
            }
        }
    }
}




