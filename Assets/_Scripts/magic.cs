using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magic : MonoBehaviour
{
    public ParticleSystem[] sparkPoints;
    //on some beat, requires much less processing power than the shake script
    public bool signalFromMusic = true;
    public float stretch=16f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (signalFromMusic)
        {
            foreach (ParticleSystem sparks in sparkPoints)
            {
                //get new stretch value
                //between 15 and 60 looks good, also between -15 and -60
                ParticleSystem.MainModule main =sparks.main;
                main.startSpeed = stretch;
                sparks.Play();
            }
        }
        else
        {
            foreach (ParticleSystem sparks in sparkPoints)
            {
                sparks.Stop();
            }

        }
    }
    


}
