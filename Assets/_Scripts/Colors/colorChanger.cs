using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorChanger : MonoBehaviour
{
    //use speedColor and speedShadows as music variables, may need (x2) multiplier for high values/beat speed and (x0.5) for low values/beat speed
    public float speedColor = 0.2f;
    public float speedShadows = 0.5f;
    public Color startColor;
    public Color endColor;
    public Color shadowStartColor;
    public Color shadowEndColor;
    public float shaderVar = 0.083f;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Sin((Time.time - startTime) * speedColor);
        float t2 = Mathf.Sin((Time.time - startTime) * speedShadows);
        float shaderVarr = Mathf.Lerp(shaderVar, 0.75f, t2);
        Color colorV = Color.Lerp(shadowStartColor, shadowEndColor, t2);
        GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);


        GetComponent<Renderer>().material.SetColor("_ColorDimExtra", colorV);
        GetComponent<Renderer>().material.SetFloat("_SelfShadingSizeExtra", shaderVarr);


    }
}
