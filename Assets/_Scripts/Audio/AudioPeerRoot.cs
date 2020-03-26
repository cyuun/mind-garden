using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPeerRoot : MonoBehaviour
{
    public static AudioPeerRoot S;

    public AudioClip defaultSong;
    public AudioSource[] audioPeers; //Orb audio peer gameobjects

    public TerrainScript terrainScript;
    public AudioMixer mixer;
    
    bool songPlaying = false;
    
    private GameObject _activeHead;
    private GameObject _terrain;
    
    public void SetActiveHead(GameObject activeHead)
    {
        _activeHead = activeHead;
        _terrain = _activeHead.transform.Find("Terrain").gameObject;
        terrainScript = _terrain.GetComponent<TerrainScript>();

        OrbScript orb1 = transform.Find("Orb").GetComponent<OrbScript>();
        OrbScript orb2 = transform.Find("Orb Variant").GetComponent<OrbScript>();
        OrbScript orb3 = transform.Find("Orb Variant 1").GetComponent<OrbScript>();
        OrbScript orb4 = transform.Find("Orb Variant 2").GetComponent<OrbScript>();

        //TODO: Randomize Orb Locations Here

        orb1.terrainScript = terrainScript;
        orb1.SpawnRocks();
        orb1.active = true;
        orb2.terrainScript = terrainScript;
        orb2.SpawnRocks();
        orb2.active = true;
        orb3.terrainScript = terrainScript;
        orb3.SpawnRocks();
        orb3.active = true;
        orb4.terrainScript = terrainScript;
        orb4.SpawnRocks();
        orb4.active = true;

        //Spawn biome after rocks to avoid tree & rock overlap
        orb1.SpawnBiome();
        orb2.SpawnBiome();
        orb3.SpawnBiome();
        orb4.SpawnBiome();

        if(Global.currentBiome == Global.BiomeType.desert)
        {
            activeHead.GetComponent<HeadScript>().grassController.gameObject.SetActive(false); //Turns off grass in desert
        }
    }

    private void Awake()
    {
        if(S==null) S = this;
    }

    void Start()
    {
        RandomizePositions();
        if (Global.currentSongInfo != null)
        {
            GetComponent<AudioSource>().clip = Global.currentSongInfo.inputSong;
            if (Global.spleeterMode)
            {
                for (int i = 0; i < audioPeers.Length; i++)
                {
                    audioPeers[i].clip = LoadAudioClip(i);
                }
            }
            else
            {
                print("no spleeter");
                for (int i = 0; i < audioPeers.Length; i++)
                {
                    //TOTO: Load in song to every orb and drop song output to 0%
                    //      Every orb picked up raises the volume by 25%
                    audioPeers[i].clip = Global.currentSongInfo.inputSong;
                    audioPeers[i].volume = .8f;
                    audioPeers[i].outputAudioMixerGroup = mixer.FindMatchingGroups("Master/FullSong")[0];
                }
            }
        }
        foreach(var orb in audioPeers)
        {
            orb.Play();
        }
        songPlaying = true;

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

    public void ToggleSong()
    {
        if (songPlaying)
        {
            foreach (AudioSource source in audioPeers)
            {
                source.Pause();
            }
            GetComponent<AudioSource>().Pause();
            songPlaying = false;
        }
        else
        {
            foreach (AudioSource source in audioPeers)
            {
                source.Play();
            }
            GetComponent<AudioSource>().Play();
            songPlaying = true;
        }
    }

    public void RandomizePositions()
    {
        foreach(AudioSource orb in audioPeers)
        {
        }
    }
}
