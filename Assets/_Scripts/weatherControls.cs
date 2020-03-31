using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weatherControls : MonoBehaviour
{
    public ParticleSystem clouds;
    public ParticleSystem rain;
    public ParticleSystem snow;
    [Range(0, 1)]
    public float fogProbability = 1f;
    [Range(0, 1)]
    public float rainProbability = .5f;
    [Range(0, 1)]
    public float snowProbability = 1f;


    // Start is called before the first frame update
    void Start()
    {
        Reset();
        StartCoroutine(DelayWeather());
    }

    IEnumerator DelayWeather()
    {
        yield return new WaitForSeconds(.1f);
        //Desert
        if (Global.currentBiome == Global.BiomeType.desert)
        {
            clouds.gameObject.SetActive(true);

            RenderSettings.fogColor = new Color(0.8f, 0.5f, 0.2f, 0.3f);
            RenderSettings.fogDensity = 0.0125f;
            RenderSettings.fog = true;
            
            if(magic.S) magic.S.gameObject.SetActive(false);
        }
        //Underwater
        if (Global.currentBiome == Global.BiomeType.underwater)
        {
            clouds.gameObject.SetActive(true);

            RenderSettings.fogColor = new Color(0.1f, 0.4f, 1f, 0.6f);
            RenderSettings.fogDensity = 0.03f;
            RenderSettings.fog = true;
            
            if(magic.S) magic.S.gameObject.SetActive(false);
        }
        //Rain
        if (Global.currentBiome != Global.BiomeType.underwater)
        {
            if (Random.Range(0f, 1f) <= rainProbability)
            {
                clouds.gameObject.SetActive(true);
                rain.gameObject.SetActive(true);

                clouds.Play();
                rain.Play();
            }
        }
        //Snow
        if (!rain.isPlaying && Global.currentBiome != Global.BiomeType.underwater && Global.currentBiome != Global.BiomeType.desert && Global.currentBiome != Global.BiomeType.jungle)
        {
            if (Random.Range(0f, 1f) <= snowProbability)
            {
                clouds.gameObject.SetActive(true);
                snow.gameObject.SetActive(true);

                clouds.Play();
                snow.Play();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            //desert fog
            RenderSettings.fogColor = new Color(0.8f, 0.5f, 0.2f, 0.3f);
            RenderSettings.fogDensity = 0.0125f;
            RenderSettings.fog = true;
            
            if(magic.S) magic.S.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            //underwater "fog"
            RenderSettings.fogColor = new Color(0.1f, 0.4f, 1f, 0.6f);
            RenderSettings.fogDensity = 0.03f;
            RenderSettings.fog = true;
            
            if(magic.S) magic.S.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            //rain
            clouds.gameObject.SetActive(true);
            rain.gameObject.SetActive(true);

            clouds.Play();
            rain.Play();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            //stop rain
            clouds.Stop();
            rain.Stop();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            //snow
            clouds.gameObject.SetActive(true);
            snow.gameObject.SetActive(true);

            clouds.Play();
            snow.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            //stop snow
            clouds.Stop();
            snow.Stop();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            Reset();
        }

    }

    void Reset()
    {
        clouds.gameObject.SetActive(false);
        snow.gameObject.SetActive(false);
        rain.gameObject.SetActive(false);
            
        if(magic.S) magic.S.gameObject.SetActive(true);

    }
}
