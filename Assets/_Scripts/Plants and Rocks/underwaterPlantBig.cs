using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class underwaterPlantBig : Plant
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
    public GameObject piece10;
    public GameObject piece11;
    public GameObject piece12;
    public GameObject piece13;
    public float scaleFactor = 1.7f;

    Vector3 piece1Scale;
    Vector3 piece2Scale;
    Vector3 piece9Scale;
    Vector3 piece10Scale;
    float piece1Max;
    float piece2Max;
    float piece9Max;
    float piece10Max;
    float piece1Min;
    float piece2Min;
    float piece9Min;
    float piece10Min;
    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    float mainMusicVariable = 10f;
    public float rotateSpeed1 = 100f;
    public float rotateSpeed2 = -80f;
    public float rotateSpeed3 = 40f;
    public float rotateSpeed4 = -120f;

    void Update()
    {
        float buffer = _audioPeer._amplitudeBuffer;
        // rotate around y-axis
        piece1.transform.Rotate(0, 0, buffer * rotateSpeed1 * Time.deltaTime);
        piece2.transform.Rotate(0, 0, buffer * rotateSpeed2 * Time.deltaTime);
        piece3.transform.Rotate(0, 0, buffer * rotateSpeed3 * Time.deltaTime);
        piece4.transform.Rotate(0, 0, buffer * rotateSpeed4 * Time.deltaTime);
        piece5.transform.Rotate(0, 0, buffer * rotateSpeed1 * Time.deltaTime);
        piece6.transform.Rotate(0, 0, buffer * rotateSpeed2 * Time.deltaTime);
        piece7.transform.Rotate(0, 0, buffer * rotateSpeed3 * Time.deltaTime);
        piece8.transform.Rotate(0, 0, buffer * rotateSpeed4 * Time.deltaTime);
        piece9.transform.Rotate(0, 0, buffer * rotateSpeed1 * Time.deltaTime);
        piece10.transform.Rotate(0, 0, buffer * rotateSpeed2 * Time.deltaTime);
        piece11.transform.Rotate(0, 0, buffer * rotateSpeed3 * Time.deltaTime);
        piece12.transform.Rotate(0, 0, buffer * rotateSpeed4 * Time.deltaTime);
        piece13.transform.Rotate(0, 0, buffer * rotateSpeed1 * Time.deltaTime);

        
    }

    private void FixedUpdate()
    {
        if(initialized) ScalePieces();
    }

    void ScalePieces()
    {
        piece1Scale.x = Mathf.Lerp(piece1Min, piece1Max, _audioPeer._audioBandBuffer[0]);
        piece1Scale.y = piece1Scale.x;
        piece1.transform.localScale = piece1Scale;

        piece2Scale.x = Mathf.Lerp(piece2Min, piece2Max, _audioPeer._audioBandBuffer[1]);
        piece2Scale.y = piece2Scale.x;
        piece2.transform.localScale = piece2Scale;

        piece9Scale.x = Mathf.Lerp(piece9Min, piece9Max, _audioPeer._audioBandBuffer[2]);
        piece9Scale.y = piece9Scale.x;
        piece9.transform.localScale = piece9Scale;

        piece10Scale.x = Mathf.Lerp(piece10Min, piece10Max, _audioPeer._audioBandBuffer[3]);
        piece10Scale.y = piece10Scale.x;
        piece10.transform.localScale = piece10Scale;

    }

    public void InitializePieces()
    {
        initialized = true;
        piece1Scale = piece1.transform.localScale;
        piece2Scale = piece2.transform.localScale;
        piece9Scale = piece9.transform.localScale;
        piece10Scale = piece10.transform.localScale;
        piece1Max = piece1Scale.x * scaleFactor;
        piece2Max = piece2Scale.x * scaleFactor;
        piece9Max = piece9Scale.x * scaleFactor;
        piece10Max = piece10Scale.x * scaleFactor;
        piece1Min = piece1Scale.x;
        piece2Min = piece2Scale.x;
        piece9Min = piece9Scale.x;
        piece10Min = piece10Scale.x;
        print(piece1Scale);

    }
}
