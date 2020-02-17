using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishSpeed : MonoBehaviour
{
    public static List<fishSpeed> fishes;

    public AudioPeer audioPeer;
    public float musicMultiplier = 1.0f;
    public float maxSpeed = 3;
    float musicTemp;
    float originalSpeed;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if (fishes == null) fishes = new List<fishSpeed>();
        if (audioPeer == null)
        {
            AudioSource[] audioSources = SpleeterProcess.S.orbs;
            audioPeer = audioSources[Random.Range(0, audioSources.Length)].GetComponent<AudioPeer>(); ;
        }
        anim = GetComponent<Animator>();
        musicTemp = musicMultiplier;
        anim.speed = musicMultiplier;

        fishes.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        anim.speed = 1 + (musicMultiplier * audioPeer._amplitudeBuffer);
        
    }

    void SpeedUp()
    {
        StartCoroutine(IncreaseSpeed());
    }

    IEnumerator IncreaseSpeed()
    {
        originalSpeed = anim.speed;
        anim.speed = maxSpeed;
        /*while(anim.speed < maxSpeed)
        {
            anim.speed += Time.deltaTime;
        }*/
        yield return null;
        while(anim.speed > originalSpeed)
        {
            anim.speed -= Time.deltaTime * 3;
        }
    }

    public static void FishBoost()
    {
        foreach(fishSpeed fish in fishes)
        {
            fish.SpeedUp();
        }
    }
}
