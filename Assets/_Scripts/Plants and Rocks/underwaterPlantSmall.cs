using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class underwaterPlantSmall : Plant
{
    public GameObject piece1;
    public GameObject piece2;
    public GameObject piece3;
    public GameObject piece4;
    public GameObject piece5;
    public GameObject piece6;
    public GameObject piece7;
    public GameObject piece8;
    public GameObject piece9;

    //0 is unmoving, 40 is pretty fast/kinda seizure-y
    float mainMusicVariable = 10f;

    void Update()
    {
        Vector3 vec1 = new Vector3(0.01f, 0.01f, 0.0085f+(Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[0]+ 0.01f)) * 0.0009f));
        Vector3 vec2 = new Vector3(0.01f, 0.01f, 0.008f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[1] + 0.08f)) * 0.001f));
        Vector3 vec3 = new Vector3(0.01171215f, 0.01171215f, 0.01f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[2] + 0.04f)) * -0.001f));
        Vector3 vec4 = new Vector3(0.01f, 0.01f, 0.0085f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[3] + 0.012f)) * 0.001f));
        Vector3 vec5 = new Vector3(-0.01171215f, -0.01171215f, -0.01f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[4] + 0.06f)) * 0.001f));

        Vector3 vec6 = new Vector3(0.00649583f, 0.00649583f, 0.004f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[1] + 0.08f)) * 0.001f));
        Vector3 vec7 = new Vector3(-0.00649583f, -0.00649583f, -0.005f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[2] + 0.04f)) * 0.001f));
        Vector3 vec8 = new Vector3(0.00649583f, 0.00649583f, 0.0045f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[3] + 0.012f)) * 0.001f));
        Vector3 vec9 = new Vector3(-0.00649583f, -0.00649583f, -0.0045f + (Mathf.Sin(Time.time * (_audioPeer._audioBandBuffer[4] + 0.06f)) * 0.001f));

        piece1.transform.localScale = vec1;
        piece2.transform.localScale = vec2;
        piece3.transform.localScale = vec3;
        piece4.transform.localScale = vec4;
        piece5.transform.localScale = vec5;
        piece6.transform.localScale = vec6;
        piece7.transform.localScale = vec7;
        piece8.transform.localScale = vec8;
        piece9.transform.localScale = vec9;
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
