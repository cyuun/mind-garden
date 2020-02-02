using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class medTree : Tree
{
    public GameObject piece1;
    public GameObject piece2;
    public GameObject piece3;
    public GameObject piece4;
    // Start is called before the first frame update
    void Start()
    {

    }
    float mainMusicVariable = 100f;
    public float rotateSpeed1;
    public float rotateSpeed2;
    public float rotateSpeed3;
    public float rotateSpeed4;

    void Update()
    {
        if (_audioPeer)
        {
            piece1.transform.Rotate(0, 0, mainMusicVariable * _audioPeer._amplitudeBuffer * rotateSpeed1 * Time.deltaTime);
            piece2.transform.Rotate(0, 0, mainMusicVariable * _audioPeer._amplitudeBuffer * rotateSpeed2 * Time.deltaTime);
            piece3.transform.Rotate(0, 0, mainMusicVariable * _audioPeer._amplitudeBuffer * rotateSpeed3 * Time.deltaTime);
            piece4.transform.Rotate(0, 0, mainMusicVariable * _audioPeer._amplitudeBuffer * rotateSpeed4 * Time.deltaTime);

        }
        else
        {
            piece1.transform.Rotate(0, 0, mainMusicVariable * rotateSpeed1 * Time.deltaTime);
            piece2.transform.Rotate(0, 0, mainMusicVariable * rotateSpeed2 * Time.deltaTime);
            piece3.transform.Rotate(0, 0, mainMusicVariable * rotateSpeed3 * Time.deltaTime);
            piece4.transform.Rotate(0, 0, mainMusicVariable * rotateSpeed4 * Time.deltaTime);
        }
    }
}
