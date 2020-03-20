using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static string inputSongPath;
    public static AudioClip inputSong;

    public static SongInfo currentSongInfo;
    private static BiomeType _biome;
    public static BiomeType currentBiome { get { return _biome; } set { _biome = value; } }
    
    public enum BiomeType
    {
        desert,
        forest,
        jungle,
        underwater
    };

    private static float _masterVolume = 1;
    public static float masterVolume { get { return _masterVolume; } set { _masterVolume = value; } }
    private static bool _callSpleeter = false;
    public static bool callSpleeter { get { return _callSpleeter; } set { _callSpleeter = value; } }
    private static bool _spleeterMode = false;
    public static bool spleeterMode { get { return _spleeterMode; } set { _spleeterMode = value; } }
    private static bool _playingGame = false;
    public static bool playingGame { get { return _playingGame; } set { _playingGame = value; } }
    private static bool _showHints = true;
    public static bool showHints { get { return _showHints; } set { _showHints = value; } }
    
    public static bool CompareFloats(float f1, float f2, float tolerance)
    {
        return (f1 <= f2 + tolerance && f1 >= f2 - tolerance);
    }
}
