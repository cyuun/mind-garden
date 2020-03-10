using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desertPlantMedium : Plant
{
    public GameObject piece1;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    float mainMusicVariable = 10f;
    public float rotateSpeed1 = 10f;

    void Update()
    {
        // rotate around y-axis
        piece1.transform.Rotate(0, _audioPeer._amplitudeBuffer * rotateSpeed1, 0);

        float v1 = Mathf.LerpUnclamped(0.5f, 0.95f, _audioPeer._amplitudeBuffer + 1);
        transform.localScale = new Vector3(v1, 0.80699f, v1);
    }
}
