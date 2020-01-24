using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class medTree : MonoBehaviour
{
    public GameObject piece1;
    public GameObject piece2;
    public GameObject piece3;
    public GameObject piece4;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    float mainMusicVariable = 10f;
    public float rotateSpeed1 = 10f;
    public float rotateSpeed2 = -8f;
    public float rotateSpeed3 = 4f;
    public float rotateSpeed4 = -12f;

    void Update()
    {
        // rotate around y-axis
        piece1.transform.Rotate(0, 0, mainMusicVariable*rotateSpeed1 * Time.deltaTime);
        piece2.transform.Rotate(0, 0, mainMusicVariable * rotateSpeed2 * Time.deltaTime);
        piece3.transform.Rotate(0, 0, mainMusicVariable * rotateSpeed3 * Time.deltaTime);
        piece4.transform.Rotate(0, 0, mainMusicVariable * rotateSpeed4 * Time.deltaTime);
    }
}
