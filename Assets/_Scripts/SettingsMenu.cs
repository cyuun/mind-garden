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

    void UpdateSettings()
    {
        UpdateVolume(volumeSlider.value);
    }
}
