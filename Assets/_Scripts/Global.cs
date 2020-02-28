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
    private static bool _callSpleeter = true;
    public static bool callSpleeter { get { return _callSpleeter; } set { _callSpleeter = value; } }
    private static bool _playingGame = true;
    public static bool playingGame { get { return _playingGame; } set { _playingGame = value; } }
    private static bool _showHints = true;
    public static bool showHints { get { return _showHints; } set { _showHints = value; } }
}
