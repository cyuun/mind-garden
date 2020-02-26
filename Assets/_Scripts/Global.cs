using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static string inputSongPath;
    public static AudioClip inputSong;

    public static SongInfo currentSongInfo;
    


    private static float _masterVolume = 1;
    public static float masterVolume { get { return _masterVolume; } set { _masterVolume = value; } }
}
