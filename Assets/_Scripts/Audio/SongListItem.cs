using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;


public class SongListItem : MonoBehaviour
{
    public SongInfo songInfo;
    public bool spleeterOn;
    const string RESOURCE_PATH = "Assets/Resources/Spleets/";

    void Start()
    {
        if(songInfo.inputSongPath != null)
        {
            songInfo.inputSong = LoadInputSong(songInfo.inputSongPath, songInfo.songName);
            songInfo.inputSong.name = Path.GetFileNameWithoutExtension(Path.GetFileName(songInfo.inputSongPath));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadThisSong()
    {
        print(songInfo.inputSongPath);
        if(spleeterOn) Global.currentSongInfo = SpleeterProcess.S.LoadSongTracks(songInfo.inputSongPath, songInfo.inputSong);
        else Global.currentSongInfo = SpleeterProcess.S.LoadSong(songInfo.inputSongPath, songInfo.inputSong);
        MenuController.S.backgroundMusic.clip = songInfo.inputSong;
        MenuController.S.backgroundMusic.transform.parent.gameObject.SetActive(true); //Turn on orb if off
        MenuController.S.backgroundMusic.Play();
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
