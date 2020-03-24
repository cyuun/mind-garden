using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public static LevelSelect S;

    public CanvasGroup biomeOptions;
    public CanvasGroup startBackButtons;
    public CanvasGroup mainCanvas;

    bool showing;
    Button activeLevelButton;
    Color defaultTextColor;
    public Color activeTextColor;

    private void Awake()
    {
        S = this;
    }

    void Start()
    {
        mainCanvas = GetComponent<CanvasGroup>();
        mainCanvas.alpha = 0;
        /*biomeOptions.alpha = 0;
        startBackButtons.alpha = 0;*/
        showing = false;
        defaultTextColor = biomeOptions.transform.GetChild(0).GetComponent<Text>().color;
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
        activeLevelButton = biomeOptions.transform.Find("Forest").GetComponent<Button>();
        activeLevelButton.Select();
        Global.currentBiome = Global.BiomeType.forest;
        Global.biomeChosen = true;
    }

    public void ChooseOcean()
    {
        activeLevelButton = biomeOptions.transform.Find("Ocean").GetComponent<Button>();
        activeLevelButton.Select();
        Global.currentBiome = Global.BiomeType.underwater;
        Global.biomeChosen = true;
    }

    public void ChooseDesert()
    {
        activeLevelButton = biomeOptions.transform.Find("Desert").GetComponent<Button>();
        activeLevelButton.Select();
        Global.currentBiome = Global.BiomeType.desert;
        Global.biomeChosen = true;
    }

    public void ChooseJungle()
    {
        activeLevelButton = biomeOptions.transform.Find("Jungle").GetComponent<Button>();
        activeLevelButton.Select();
        Global.currentBiome = Global.BiomeType.jungle;
        Global.biomeChosen = true;
    }

    public void Random()
    {
        activeLevelButton = biomeOptions.transform.Find("Random").GetComponent<Button>();
        activeLevelButton.Select();
        Global.biomeChosen = false;
    }

}
