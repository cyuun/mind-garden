using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public GameObject waterSplashPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Splash(float x, float z)
    {
        GameObject splash = Instantiate(waterSplashPrefab, new Vector3(x, 6, z), Quaternion.identity, this.transform);

    }
}
