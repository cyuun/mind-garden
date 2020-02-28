using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioPeerRoot : MonoBehaviour
{
    public static AudioPeerRoot S;

    public AudioClip defaultSong;
    public AudioSource[] audioPeers; //Orb audio peer gameobjects

    private void Awake()
    {
        if(S==null) S = this;
    }

    void Start()
    {
        if (Global.currentSongInfo != null)
        {
            GetComponent<AudioSource>().clip = Global.currentSongInfo.inputSong;
            if (Global.callSpleeter)
            {
                for (int i = 0; i < audioPeers.Length; i++)
                {
                    audioPeers[i].clip = LoadAudioClip(i);
                }
            }
            else
            {
                for (int i = 0; i < audioPeers.Length; i++)
                {
                    //TOTO: Load in song to every orb and drop song output to 0%
                    //      Every orb picked up raises the volume by 25%
                }
            }
        }
        foreach(var orb in audioPeers)
        {
            orb.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    AudioClip LoadAudioClip(int track)
    {
        string url = "";
        string name = "";
        switch (track)
        {
            case 0:
                url = Global.currentSongInfo.melody;
                name = "Melody";
                break;
            case 1:
                url = Global.currentSongInfo.bass;
                name = "Bass";
                break;
            case 2:
                url = Global.currentSongInfo.vocals;
                name = "Vocals";
                break;
            case 3:
                url = Global.currentSongInfo.drums;
                name = "Drums";
                break;
            default:
                break;
        }

        var bytes = File.ReadAllBytes(url);
        WAV wav = new WAV(bytes); //Use WAV class to convert audio file
        AudioClip audioClip = AudioClip.Create(name, wav.SampleCount, 1, wav.Frequency, false);
        audioClip.SetData(wav.LeftChannel, 0);

        return audioClip;
    }
}
