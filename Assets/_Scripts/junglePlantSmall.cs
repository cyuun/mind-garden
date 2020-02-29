using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class junglePlantSmall : Plant
{
    public GameObject piece1;
    public GameObject piece2;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    float mainMusicVariable = 10f;
    public float rotateSpeed1 = 10f;
    public float rotateSpeed2 = -8f;

    void Update()
    {
        // rotate around y-axis
        piece1.transform.Rotate(0, mainMusicVariable * rotateSpeed1 * Time.deltaTime, 0);
        piece2.transform.Rotate(0, mainMusicVariable * rotateSpeed2 * Time.deltaTime, 0);
    }
}
