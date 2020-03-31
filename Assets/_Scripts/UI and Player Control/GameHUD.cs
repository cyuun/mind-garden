using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour
{
    public static GameHUD S;
    public GameObject startMessage;
    public GameObject spleeterHint;
    public GameObject curtain;
    public Text screenshotMessage;
    public Color greenFlash;
    public Color redFlash;
    public Color whiteFlash;

    [SerializeField]
    public Dictionary<string, Color> curtainPalette;

    private bool showingMessage = false;

    void Start()
    {
        S = this;
        startMessage.SetActive(false);
        StartCoroutine(FadeIn());
        if (Global.showHints)
        {
            startMessage.SetActive(true);
            if (!Global.spleeterMode)
            {
                spleeterHint.SetActive(false);
            }
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
        Time.timeScale = 1;
        Time.fixedDeltaTime = SettingsMenu.S.fixedDeltaTime * Time.timeScale;
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

    public void ScreenshotMessage()
    {
        if (!showingMessage)
        {
            StartCoroutine(ScreenhotCaptured(4f));
        }
    }

    IEnumerator FadeStartMessage(float wait)
    {
        yield return new WaitForSeconds(wait);
        float a = GetComponent<CanvasGroup>().alpha;
        while(a > 0)
        {
            a -= Time.deltaTime;
            GetComponent<CanvasGroup>().alpha = a;
            yield return null;
        }
        GetComponent<CanvasGroup>().alpha = 0;
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
        SettingsMenu.S.gameIsPaused = false;
        LoadingBar.S.gameObject.SetActive(true);
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

    IEnumerator ScreenhotCaptured(float time)
    {
        showingMessage = true;
        Color color = screenshotMessage.color;
        color.a = 1;
        screenshotMessage.color = color;
        yield return new WaitForSeconds(time);

        while (color.a > 0)
        {
            color.a -= (Time.deltaTime / (time * 2));
            screenshotMessage.color = color;
            yield return null;
        }
        showingMessage = false;
    }
}
