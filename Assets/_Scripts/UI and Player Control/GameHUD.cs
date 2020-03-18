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
    public Color greenFlash;
    public Color redFlash;
    public Color whiteFlash;

    [SerializeField]
    public Dictionary<string, Color> curtainPalette;

    void Start()
    {
        S = this;
        startMessage.SetActive(false);
        StartCoroutine(FadeIn());
        if (Global.showHints)
        {
            startMessage.SetActive(true);
        }
        curtainPalette = new Dictionary<string, Color>() {
            { "Green", greenFlash },
            { "Red", redFlash },
            { "White", whiteFlash },
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        Cursor.lockState = CursorLockMode.None;
        SettingsMenu.gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = SettingsMenu.S.fixedDeltaTime * Time.timeScale;
        Global.playingGame = false;
        //TODO: Reinitialize certain static variables for next game
        OrbScript.biomeChosen = false;
        Global.currentSongInfo = null;
        Global.inputSong = null;
        Global.inputSongPath = null;
        if (smallTree.allSmallTrees != null) smallTree.allSmallTrees.Clear();
        if (desertPlantSmall.allSmallTrees != null) desertPlantSmall.allSmallTrees.Clear();

        StartCoroutine(FadeExit());
    }

    public void FlashColor(string color)
    {
        if (curtainPalette.ContainsKey(color))
        {
            StartCoroutine(Flash(curtainPalette[color], .2f, .4f));

        }
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

    IEnumerator Flash(Color color, float attack, float decay)
    {
        float _time = 0;
        float limit = .5f;
        color.a = 0;
        curtain.GetComponentInChildren<Image>().color = color;
        curtain.SetActive(true);
        while(_time < attack)
        {
            float alpha = Mathf.Lerp(0f, limit, _time / attack);
            color.a = alpha;
            curtain.GetComponentInChildren<Image>().color = color;
            _time += Time.deltaTime;
            yield return null;
        }

        _time = 0;
        while (_time < decay)
        {
            float alpha = Mathf.Lerp(limit, 0f, _time / decay);
            color.a = alpha;
            curtain.GetComponentInChildren<Image>().color = color;
            _time += Time.deltaTime;
            yield return null;
        }

        curtain.SetActive(false);
    }
}
