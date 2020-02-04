using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bigTree : Tree
{
    [Range(0, 2)]
    public float _amplitudeScale;

    public Transform piece1;
    public Transform piece2;
    public Transform piece3;
    public Transform piece4;
    public Transform piece5;

    //0 is unmoving, 40 is pretty fast/kinda seizure-y
    float mainMusicVariable = 10f;

    void Update()
    {
        if (_audioPeer)
        {
            piece1.localScale = new Vector3(0.0034f, 0.0034f, 0.0045f + (_audioPeer._amplitudeBuffer * _amplitudeScale * 0.015f));
            piece2.localScale = new Vector3(0.0034f, 0.0034f, 0.003f + (_audioPeer._amplitudeBuffer * _amplitudeScale * 0.015f));
            piece3.localScale = new Vector3(0.0034f, 0.0034f, 0.003f + (_audioPeer._amplitudeBuffer * _amplitudeScale * 0.015f));
            piece4.localScale = new Vector3(0.0034f, 0.0034f, 0.0035f + (_audioPeer._amplitudeBuffer * _amplitudeScale * 0.015f));
            piece5.localScale = new Vector3(0.0034f, 0.0034f, 0.0035f + (_audioPeer._amplitudeBuffer * _amplitudeScale * 0.015f));
        }
        else
        {
            piece1.localScale = new Vector3(0.0034f, 0.0034f, 0.0045f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.01f)) * 0.0009f));
            piece2.localScale = new Vector3(0.0034f, 0.0034f, 0.003f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.08f)) * 0.001f));
            piece3.localScale = new Vector3(0.0034f, 0.0034f, 0.003f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.04f)) * 0.001f));
            piece4.localScale = new Vector3(0.0034f, 0.0034f, 0.0035f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.012f)) * 0.001f));
            piece5.localScale = new Vector3(0.0034f, 0.0034f, 0.0035f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.06f)) * 0.001f));
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
