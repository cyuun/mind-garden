using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public static LevelSelect S;

    public Dropdown biomeOptions;
    public Dropdown colorOptions;
    public CanvasGroup startBackButtons;
    public CanvasGroup mainCanvas;

    bool showing;
    Button activeLevelButton;
    public InputField inputSeed;
    public Toggle lerpToggle;
    public Color activeTextColor;

    public Material[] skyboxMats;

    public GameObject terrain;

    private void Awake()
    {
        S = this;
    }

    void Start()
    {
        QuickUpdate();
        mainCanvas = GetComponent<CanvasGroup>();
        mainCanvas.alpha = 0;
        /*biomeOptions.alpha = 0;
        startBackButtons.alpha = 0;*/
        showing = false;
    }

    public void Fade()
    {
        if (showing) StartCoroutine(FadeOut());
        else StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        showing = true;
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            mainCanvas.alpha = a;
            yield return null;
        }
        mainCanvas.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        showing = false;
        float a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime;
            mainCanvas.alpha = a;
            yield return null;
        }
        mainCanvas.alpha = 0;
    }

    public void ChooseForest()
    {
        Global.currentBiome = Global.BiomeType.forest;
        Global.biomeChosen = true;
    }

    public void ChooseOcean()
    {
        Global.currentBiome = Global.BiomeType.underwater;
        Global.biomeChosen = true;
    }

    public void ChooseDesert()
    {
        Global.currentBiome = Global.BiomeType.desert;
        Global.biomeChosen = true;
    }

    public void ChooseJungle()
    {
        Global.currentBiome = Global.BiomeType.jungle;
        Global.biomeChosen = true;
    }

    public void ChooseRandom()
    {
        Global.biomeChosen = false;
    }

    public void SetBiome(int biome)
    {
        switch (biome)
        {
            case 0: //Random
                ChooseRandom();
                SetSkybox(0);
                break;
            case 1: //Forest
                ChooseForest();
                SetSkybox(1);
                break;
            case 2: //Ocean
                ChooseOcean();
                SetSkybox(2);
                break;
            case 3: //Jungle
                ChooseJungle();
                SetSkybox(3);
                break;
            case 4: //Desert
                ChooseDesert();
                SetSkybox(4);
                break;
        }
    }

    public void SetSeed()
    {
        string seedString = inputSeed.textComponent.text;
        if(seedString.Length == 0)
        {
            Global.seed = Mathf.FloorToInt(Random.value * Random.Range(10,1000));
            return;
        }

        List<char> legalChars = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        int index = 0;
        foreach(char c in seedString)
        {
            if (!legalChars.Contains(c))
            {
                seedString.Replace(c, '0');
            }
            index++;
        }
        int seed;
        bool gotSeed = int.TryParse(seedString, out seed);
        if (gotSeed) Global.seed = seed;
    }

    public void SetColors(int palette)
    {
        if(palette == 0)
        {
            Global.colorPalette = Random.Range(0, 6);
        }
        else
        {
            palette--;
            Global.colorPalette = palette;
        }
    }

    public void SetLerp(bool lerp)
    {
        Global.lerp = lerp;
    }

    public void SetSkybox(int selectedBiome)
    {
        switch (selectedBiome)
        {
            case 0:
                Camera.main.GetComponent<Skybox>().material = skyboxMats[0];
                break;
            case 1:
                Camera.main.GetComponent<Skybox>().material = skyboxMats[1];
                break;
            case 2:
                Camera.main.GetComponent<Skybox>().material = skyboxMats[2];
                break;
            case 3:
                Camera.main.GetComponent<Skybox>().material = skyboxMats[3];
                break;
            case 4:
                Camera.main.GetComponent<Skybox>().material = skyboxMats[4];
                break;
        }
    }

    public void QuickUpdate()
    {
        SetBiome(biomeOptions.value);
        SetColors(colorOptions.value);
        SetSeed();
        SetLerp(lerpToggle.isOn);
    }
}
