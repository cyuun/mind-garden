using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desertPlantMedium : MonoBehaviour
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
        piece1.transform.Rotate(0, mainMusicVariable * rotateSpeed1 * Time.deltaTime, 0);
    }
}
