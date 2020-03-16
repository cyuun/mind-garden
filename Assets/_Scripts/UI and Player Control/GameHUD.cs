using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour
{
    public static GameHUD S;
    public GameObject startMessage;
    public GameObject curtain;

    void Start()
    {
        S = this;
        startMessage.SetActive(false);
        StartCoroutine(FadeIn());
        if (Global.showHints)
        {
            startMessage.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        StartCoroutine(FadeExit());
    }

    IEnumerator FadeStartMessage(float wait)
    {
        yield return new WaitForSeconds(wait);

        Color textColor = startMessage.GetComponentInChildren<Text>().color;
        Color imageColor = startMessage.GetComponentInChildren<Image>().color;
        while(textColor.a > 0)
        {
            float alpha = textColor.a - (Time.deltaTime / 2);
            textColor = new Color(1, 1, 1, alpha);
            imageColor = new Color(0, 0, 0, alpha);
            startMessage.GetComponentInChildren<Text>().color = textColor;
            startMessage.GetComponentInChildren<Image>().color = imageColor;
            yield return null;
        }
    }

    IEnumerator FadeExit()
    {
        curtain.SetActive(true);
        Image bg = curtain.GetComponentInChildren<Image>();
        bg.color = new Color(0, 0, 0, 0);
        while (bg.color.a < 1)
        {
            float alpha = bg.color.a + Time.deltaTime;
            bg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        bg.color = new Color(0, 0, 0, 1);
        LoadingBar.S.Show(SceneManager.LoadSceneAsync(0));
    }

    IEnumerator FadeIn()
    {
        curtain.SetActive(true);
        Image bg = curtain.GetComponentInChildren<Image>();
        bg.color = new Color(0, 0, 0, 1);
        while (bg.color.a > 0)
        {
            float alpha = bg.color.a - Time.deltaTime;
            bg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        StartCoroutine(FadeStartMessage(2f));
    }
}
