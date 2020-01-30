using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbScript : MonoBehaviour
{
    bool soundOn = true;
    bool fading = false;
    float currentTime;
    float fadeDuration = 2f;

    AudioSource audioTrack;
    public GameObject audio;
    public Image buttonLabel;

    // Start is called before the first frame update
    void Start()
    {
        audioTrack = audio.GetComponent<AudioSource>();
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
        if(Vector3.Distance(this.transform.position, Camera.main.transform.position) < 10f)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                //TODO: Follow Player

                //TODO: Toggle Particles


                //ToggleVolume();
            }
           //ShowButtonLabel();
        }
    }

    private void OnMouseExit()
    {
        //buttonLabel.gameObject.SetActive(false);
    }

    void ShowButtonLabel()
    {
        Vector3 labelPos = Camera.main.WorldToScreenPoint(this.transform.position);
        buttonLabel.transform.position = labelPos + (new Vector3(1,1,0) * 30);
        buttonLabel.gameObject.SetActive(true);
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
