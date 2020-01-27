using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    bool soundOn = true;
    bool fading = false;
    float currentTime;
    float fadeDuration = 2f;

    public AudioSource audioTrack;

    // Start is called before the first frame update
    void Start()
    {
        if (!audioTrack)
        {
            audioTrack = gameObject.GetComponentInChildren<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            ToggleVolume();
        }
    }

    void ToggleVolume()
    {
        if (!fading)
        {
            if (soundOn)
            {
                currentTime = 0;
                StartCoroutine(FadeOut());
                soundOn = false;
            }
            else
            {
                currentTime = 0;
                StartCoroutine(FadeIn());
                soundOn = true;
            }

        }
    }

    IEnumerator FadeIn()
    {
        audioTrack.volume = 0f;

        while (audioTrack.volume < 1f)
        {
            audioTrack.volume += Time.deltaTime;
            yield return null;
        }
        
    }

    IEnumerator FadeOut()
    {
        audioTrack.volume = 1f;

        while (audioTrack.volume > 0f)
        {
            audioTrack.volume -= Time.deltaTime;
            yield return null;

        }
    }
}
