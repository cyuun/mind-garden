﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;


public class SongListItem : MonoBehaviour
{
    public static SongListItem currentSongPlaying;

    public SongInfo songInfo;
    public bool spleeterOn;
    const string RESOURCE_PATH = "Assets/Resources/Spleets/";

    void Start()
    {
        if(songInfo.inputSongPath != null)
        {
            if (songInfo.inputSongPath.Contains("Resources"))
            {
                songInfo.inputSongPath = Application.streamingAssetsPath + "/" + songInfo.inputSongPath;
            }
            songInfo.inputSong = LoadInputSong(songInfo.inputSongPath, Path.GetFileNameWithoutExtension(Path.GetFileName(songInfo.inputSongPath)));
            songInfo.inputSong.name = Path.GetFileNameWithoutExtension(Path.GetFileName(songInfo.inputSongPath));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadThisSong()
    {
        if(currentSongPlaying != this)
        {
            if (spleeterOn) Global.currentSongInfo = SpleeterProcess.S.LoadSongTracks(songInfo.inputSongPath, songInfo.inputSong);
            else Global.currentSongInfo = SpleeterProcess.S.LoadSong(songInfo.inputSongPath, songInfo.inputSong);
            MenuController.S.backgroundMusic.clip = songInfo.inputSong;
            MenuController.S.backgroundMusic.transform.parent.gameObject.SetActive(true); //Turn on orb if off
            MenuController.S.backgroundMusic.Play();

            MenuController.S.silentMusic.clip = songInfo.inputSong;
            MenuController.S.silentMusic.Play();
            AudioAnalyzer.S.AnalyzeClip(songInfo.inputSong);

            LevelSelect.S.SetTitle(songInfo.inputSong.name);
            currentSongPlaying = this;
        }
        else
        {
            Global.currentSongInfo = null;
            MenuController.S.backgroundMusic.transform.parent.gameObject.SetActive(false); //Turn on orb if off
            MenuController.S.backgroundMusic.Stop();
            currentSongPlaying = null;
        }
    }

    AudioClip LoadInputSong(string inputSongPath, string name)
    {
        AudioClip inputSong;
        if (Path.GetExtension(inputSongPath) == ".mp3")
        {
            
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(inputSongPath);
            AudioClip audioClip = NAudioPlayer.FromMp3Data(bytes);
            inputSong = audioClip;
            return inputSong;

        }
        else if (Path.GetExtension(inputSongPath) == ".wav")
        {
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(inputSongPath);
            WAV wav = new WAV(bytes);

            AudioClip audioClip;
            audioClip = AudioClip.Create(name, wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
            inputSong = audioClip;
            return inputSong;

        }
        return null;
    }
}
