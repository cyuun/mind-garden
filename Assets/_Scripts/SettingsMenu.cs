using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    //SETTINGS
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public Text volumeLabel;
    public Toggle spleeterToggle;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSettings();
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
        Global.callSpleeter = spleeterToggle.isOn;

    }

    void UpdateSettings()
    {
        UpdateVolume(volumeSlider.value);
        UpdateCallSpleeter(spleeterToggle.isOn);
    }
}
