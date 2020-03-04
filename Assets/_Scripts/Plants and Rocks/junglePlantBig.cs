using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class junglePlantBig: Plant
{
    public GameObject piece1;
    public GameObject piece2;
    public GameObject piece3;
    public GameObject piece4;
    Animator anim;
    bool start = false;
    public bool isSpawningPrefab;

    //0 is unmoving, 40 is pretty fast/kinda seizure-y
    float mainMusicVariable = 10f;

    private void Start()
    {
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (!start && anim)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("deadAnim"))
            {
                anim.enabled = false;
                start = true;
            }
        }

        else if (start || !isSpawningPrefab)
        {
            float v1 = Mathf.Lerp(0.015f, 0.035f, _audioPeer._audioBandBuffer[1] + 0.15f);
            float v2 = Mathf.Lerp(0.013f, 0.035f, _audioPeer._audioBandBuffer[2] + 0.15f);
            float v3 = Mathf.Lerp(0.01f, 0.035f, _audioPeer._audioBandBuffer[3] + 0.15f);
            float v4 = Mathf.Lerp(0.011f, 0.035f, _audioPeer._audioBandBuffer[2] + 0.15f);

            /*Vector3 vec1 = new Vector3(0.00428168f, 0.00428168f, 0.0264475f + (mainMusicVariable * _audioPeer._amplitudeBuffer) * 0.004f);
            Vector3 vec2 = new Vector3(0.00428168f, 0.00428168f, 0.01418361f + (mainMusicVariable * _audioPeer._amplitudeBuffer) * 0.005f);
            Vector3 vec3 = new Vector3(0.00428168f, 0.00428168f, 0.01715916f + (mainMusicVariable * _audioPeer._amplitudeBuffer) * 0.004f);
            Vector3 vec4 = new Vector3(0.00428168f, 0.00428168f, 0.02068897f + (mainMusicVariable * _audioPeer._amplitudeBuffer) * 0.006f);*/

            piece1.transform.localScale = new Vector3(0.00428168f, 0.00428168f, v1);
            piece2.transform.localScale = new Vector3(0.00428168f, 0.00428168f, v2);
            piece3.transform.localScale = new Vector3(0.00428168f, 0.00428168f, v3);
            piece4.transform.localScale = new Vector3(0.00428168f, 0.00428168f, v4);
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
