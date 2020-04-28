using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desertPlantBig : Plant
{
    public GameObject piece1;
    public GameObject piece2;
    Animator anim;
    public bool isSpawningPrefab;
    //0 is unmoving, 40 is pretty fast/kinda seizure-y
    float mainMusicVariable = 10f;
    float[] bandBuffers;
    bool start = false;
    private void Start()
    {
        if(GetComponent<Animator>()!= null)
        {
            anim = GetComponent<Animator>();
        }
        
    }

    void Update()
    {
        if(!start && anim)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("deadAnim"))
            {
                anim.enabled = false;
                start = true;
            }
        }
        
        else if (start || !isSpawningPrefab)
        {
            bandBuffers = _audioPeer._audioBandBuffer;

            float v1 = Mathf.Lerp(0.001f, 0.0044f, bandBuffers[1] + bandBuffers[3]);
            float v2 = Mathf.Lerp(0.001f, 0.0044f, bandBuffers[0] + bandBuffers[2]);
            float v3 = Mathf.Lerp(.9f, 1.5f, bandBuffers[0] + bandBuffers[3]);

            piece1.transform.localScale = new Vector3(0.0034f, 0.0034f, v1);
            piece2.transform.localScale = new Vector3(0.0034f, 0.0034f, v2);
            transform.localScale = new Vector3(1, v3, 1);

        }

    }

    // void Update()
    //{
    //piece1.transform.localScale+= new Vector3(0, 0, mainMusicVariable * scaleSpeed1 * Time.deltaTime);
    //piece2.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed2 * Time.deltaTime);
    //piece3.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed3 * Time.deltaTime);
    //piece4.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed4 * Time.deltaTime);
    //piece5.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed5 * Time.deltaTime);
    //}
}
