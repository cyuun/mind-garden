using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongListItem : MonoBehaviour
{
    public SongInfo songInfo;

    void Start()
    {
        //songInfo = new SongInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadThisSong()
    {
        Global.currentSongInfo = SpleeterProcess.S.LoadSongTracks(songInfo.inputSongPath, songInfo.inputSong);
        MenuController.S.backgroundMusic.clip = songInfo.inputSong;
        MenuController.S.backgroundMusic.transform.parent.gameObject.SetActive(true); //Turn on orb if off
        MenuController.S.backgroundMusic.Play();
    }
}
