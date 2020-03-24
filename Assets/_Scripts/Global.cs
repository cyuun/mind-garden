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
    private static bool _spleeterMode = true;
    public static bool spleeterMode { get { return _spleeterMode; } set { _spleeterMode = value; } }
    private static bool _playingGame = false;
    public static bool playingGame { get { return _playingGame; } set { _playingGame = value; } }
    private static bool _showHints = true;
    public static bool showHints { get { return _showHints; } set { _showHints = value; } }
    private static int _colorPalette = 0;
    public static int colorPalette { get { return _colorPalette; } set { _colorPalette = value; } }
    private static bool _smoothColorController = true;
    public static bool smoothColorController { get { return _smoothColorController; } set { _smoothColorController = value; } }

    public static bool CompareFloats(float f1, float f2, float tolerance)
    {
        return (f1 <= f2 + tolerance && f1 >= f2 - tolerance);
    }
    private static float _pitch = 1;
    public static float pitch { get { return _pitch; } set { _pitch = value; } }
    private static bool _biomeChosen = false;
    public static bool biomeChosen { get { return _biomeChosen; } set { _biomeChosen = value; } }
}
