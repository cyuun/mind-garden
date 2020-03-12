using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public GameObject startMessage;

    void Start()
    {
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

    IEnumerator FadeStartMessage()
    {
        yield return new WaitForSeconds(3f);

        Color textColor = startMessage.GetComponentInChildren<Text>().color;
        Color imageColor = startMessage.GetComponentInChildren<Image>().color;
        while(textColor.a > 0)
        {
            float alpha = textColor.a - (Time.deltaTime / 2);
            textColor = new Color(0, 0, 0, alpha);
            imageColor = new Color(1, 1, 1, alpha);
            startMessage.GetComponentInChildren<Text>().color = textColor;
            startMessage.GetComponentInChildren<Image>().color = imageColor;
            yield return null;
        }
    }
}
