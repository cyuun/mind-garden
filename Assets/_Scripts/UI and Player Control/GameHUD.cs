using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour
{
    public static GameHUD S;
    public GameObject startMessage;

    void Start()
    {
        S = this;
        if (Global.showHints)
        {
            startMessage.SetActive(true);
            StartCoroutine(FadeStartMessage());
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

    IEnumerator FadeStartMessage()
    {
        yield return new WaitForSeconds(3f);

        Color textColor = startMessage.GetComponentInChildren<Text>().color;
        Color imageColor = startMessage.GetComponentInChildren<Image>().color;
        while(textColor.a > 0)
        {
            float alpha = textColor.a - (Time.deltaTime / 2);
            textColor = new Color(0, 0, 0, alpha);
            imageColor = new Color(0, 0, 0, alpha);
            startMessage.GetComponentInChildren<Text>().color = textColor;
            startMessage.GetComponentInChildren<Image>().color = imageColor;
            yield return null;
        }
    }

    IEnumerator FadeExit()
    {
        startMessage.SetActive(true);
        startMessage.GetComponentInChildren<Text>().gameObject.SetActive(false);
        Image bg = startMessage.GetComponentInChildren<Image>();
        bg.color = new Color(0, 0, 0, 0);
        while (bg.color.a < 1)
        {
            float alpha = bg.color.a + Time.deltaTime;
            bg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        SceneManager.LoadScene(0);
    }
}
