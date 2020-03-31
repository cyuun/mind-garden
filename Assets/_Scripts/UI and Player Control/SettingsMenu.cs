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
    public Slider pitchSlider;
    public Text pitchLabel;
    public GameObject pauseMenu;
    public GameObject grassSelect;
    public GameObject colorToggle;

    public float fixedDeltaTime;
    public bool gameIsPaused = false;
    public bool resumed = false;

    private bool _advancedSettingsActive = false;

    private void Awake()
    {
        S = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Global.playingGame == true)
        {
            //StartCoroutine(DelayedUpdate(.1f));
            UpdateSettings();
        }
        else
        {
            //StartCoroutine(DelayedGlobalUpdate(.1f));
            UpdateGlobalSettings();
        }
        this.fixedDeltaTime = Time.fixedDeltaTime;

        if (mainMixer)
        {
            if (Global.spleeterMode)
            {
                mainMixer.SetFloat("songVol", -80);
            }
            else
            {
                mainMixer.SetFloat("songVol", 0);
            }
        }
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
        //StartCoroutine(ResumeTime());
        Time.timeScale = 1;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        resumed = true;
        gameIsPaused = false;
    }

    void Pause()
    {
        AudioPeerRoot.S.ToggleSong();
        pauseMenu.SetActive(true);
        //StartCoroutine(PauseTime());
        Time.timeScale = 0;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        gameIsPaused = true;
    }

    public void UpdateVolume(float value)
    {
        float dB = (value * 80);
        Global.masterVolume = -80 + dB;
        mainMixer.SetFloat("masterVol", Global.masterVolume);
        volumeLabel.text = (Mathf.Floor(value * 100)).ToString();

    }

    public void UpdateCallSpleeter(bool value)
    {
        Global.callSpleeter = value;

    }

    public void UpdateHints(bool value)
    {
        Global.showHints = value;
    }

    public void UpdatePitch(float value)
    {
        float pitch = 80 + (value * 40);
        if(pitch > 98 && pitch < 102.9)
        {
            pitch = 100;
            pitchSlider.handleRect.localPosition = Vector3.zero;
            pitchSlider.fillRect.anchorMax = new Vector2(.5f, 0);
        }
        else
        {
            float x = (value * 200) - 100;
            pitchSlider.handleRect.localPosition = new Vector3(x, 0, 0);
            pitchSlider.fillRect.anchorMax = new Vector2(value, 0);

        }

        Global.pitch = pitch/100;
        mainMixer.SetFloat("masterPitch", Global.pitch);
        pitchLabel.text = (Mathf.Floor(pitch)).ToString();
    }

    public void UpdatePerformance(int value)
    {
        switch (value)
        {
            case 0:
                if (_advancedSettingsActive)
                {
                    DeactivateAdvancedSettings();
                }
                Global.smoothColorController = true;
                Global.grassLevel = 2;
                break;
            
            case 1:
                if (_advancedSettingsActive)
                {
                    DeactivateAdvancedSettings();
                }
                Global.smoothColorController = false;
                Global.grassLevel = 1;
                break;
            
            case 2:
                if (!_advancedSettingsActive)
                {
                    ActivateAdvancedSettings();
                }
                break;
        }
        if (ColorController.S != null)
        {
            ColorController.S.UpdateFromGlobalValues();
        }
    }

    public void UpdateGrassLevel(int value)
    {
        switch (value)
        {
            case 0:
                Global.grassLevel = 2;
                break;
            
            case 1:
                Global.grassLevel = 1;
                break;
            
            case 2:
                Global.grassLevel = 0;
                break;
        }
    }

    public void UpdateLerp(bool value)
    {
        Global.smoothColorController = value;
    }

    public void UpdateGlobalSettings()
    {
        UpdateVolume(volumeSlider.value);
        UpdateCallSpleeter(spleeterToggle.isOn);
        UpdateHints(hintToggle.isOn);
        UpdatePitch(pitchSlider.value);
    }

    public void UpdateSettings()
    {
        volumeSlider.value = (Global.masterVolume + 80) / 80;
        spleeterToggle.isOn = Global.callSpleeter;
        hintToggle.isOn = Global.showHints;
        pitchSlider.value = ((Global.pitch * 100) - 80) / 40;
        UpdateVolume(volumeSlider.value);
        UpdateCallSpleeter(spleeterToggle.isOn);
        UpdateHints(hintToggle.isOn);
        UpdatePitch(pitchSlider.value);
    }

    private void ActivateAdvancedSettings()
    {
        grassSelect.SetActive(true);
        colorToggle.SetActive(true);
        UpdateGrassLevel(grassSelect.GetComponentInChildren<Dropdown>().value);
        UpdateLerp(colorToggle.GetComponentInChildren<Toggle>().isOn);
        _advancedSettingsActive = true;
    }

    private void DeactivateAdvancedSettings()
    {
        grassSelect.SetActive(false);
        colorToggle.SetActive(false);
        _advancedSettingsActive = false;
    }

    IEnumerator DelayedGlobalUpdate(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateGlobalSettings();
    }

    IEnumerator DelayedUpdate(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateSettings();
    }

    IEnumerator PauseTime()
    {
        while(Time.timeScale > 0)
        {
            Time.timeScale -= Time.deltaTime;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            yield return null;
        }
        Time.timeScale = 0;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    IEnumerator ResumeTime()
    {
        while (Time.timeScale < 0)
        {
            Time.timeScale -= Time.deltaTime;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            yield return null;
        }
        Time.timeScale = 1;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

}
