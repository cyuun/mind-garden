using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu S;

    //SETTINGS
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public Text volumeLabel;
    public Toggle spleeterToggle;
    public Toggle hintToggle;
    public GameObject pauseMenu;

    public float fixedDeltaTime;
    public static bool gameIsPaused = false;

    private PlayerScript player;

    // Start is called before the first frame update
    void Start()
    {
        S = this;
        player = PlayerScript.S;
        UpdateSettings();
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (Global.playingGame)
        {
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.Return))
            {
                if (gameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
#else
             if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (gameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
#endif
        }

    }

    public void Exit()
    {
        //TODO: Fade to black coroutine
        GameHUD.S.Exit();
    }

    public void Resume()
    {
        AudioPeerRoot.S.ToggleSong();
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameIsPaused = false;
    }

    void Pause()
    {
        AudioPeerRoot.S.ToggleSong();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        gameIsPaused = true;
    }

    public void UpdateVolume(float value)
    {
        float dB = value * 100;
        Global.masterVolume = -80 + dB;
        mainMixer.SetFloat("masterVol", Global.masterVolume);
        volumeLabel.text = (Mathf.Floor(dB)).ToString();

    }

    public void UpdateCallSpleeter(bool value)
    {
        Global.callSpleeter = value;

    }

    public void UpdateHints(bool value)
    {
        Global.showHints = value;
    }

    void UpdateSettings()
    {
        UpdateVolume(volumeSlider.value);
        UpdateCallSpleeter(spleeterToggle.isOn);
        UpdateHints(hintToggle.isOn);
    }
}
