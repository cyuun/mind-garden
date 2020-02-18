using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class SpleeterProcess : MonoBehaviour
{
    public static SpleeterProcess S;

    string inputSongPath;
    public AudioClip inputSong;
    public AudioSource[] orbs;
    public bool callSpleeter;
    public bool playOnAwake;

    void Awake()
    {
        S = this;

        if (callSpleeter) 
        {
            if (Global.inputSongPath != null && Global.inputSong != null)
            {
                inputSongPath = Global.inputSongPath;
                inputSong = Global.inputSong;
                inputSong.name = Path.GetFileNameWithoutExtension(inputSongPath);
            }
            else
            {
                inputSongPath = AssetDatabase.GetAssetPath(inputSong);
            }

            //Use Regex to parse out illegal characters
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            string oldName = inputSong.name;
            inputSong.name = r.Replace(inputSong.name, "");
            inputSong.name = inputSong.name.Replace(" ", string.Empty);
            if(inputSong.name != oldName)
            {
                File.Move(inputSongPath, Path.Combine(Path.GetDirectoryName(inputSongPath), inputSong.name + Path.GetExtension(inputSongPath))); //Renames song without illegal characters
            }
            inputSongPath = Path.Combine(Path.GetDirectoryName(inputSongPath), inputSong.name + Path.GetExtension(inputSongPath));


            Process process = new Process();
            // Configure the process using the StartInfo properties.
            string filePath = Application.streamingAssetsPath + "/spleeter/spleeter/"; //Current Directory plus song path
            string outputPath = Application.persistentDataPath + "/Spleets/";
            process.StartInfo.FileName = filePath + "spleeter.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.StartInfo.Arguments = "separate -i " + inputSongPath + " -p spleeter:4stems -o \"" + outputPath + "\""; //Shell executable

            process.Start();
            process.WaitForExit();

            LoadSongTracks();
        }

        //Play
        if (playOnAwake)
        {
            foreach (AudioSource audio in orbs)
            {
                audio.Play();
            }

        }
    }

    void Start()
    {
        GetComponent<AudioSource>().clip = inputSong;
    }

    void LoadSongTracks()
    {
        string track;
        string url = "";
        for (int i = 0; i < orbs.Length; i++)
        {
            track = orbs[i].name;

            switch (track)
            {
                case "Melody":
                    url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/other.wav"; //TODO: Parse file url for invalid chars and rename if necessary
                    break;
                case "Bass":
                    url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/bass.wav"; 
                    break;
                case "Vocals":
                    url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/vocals.wav"; 
                    break;
                case "Drums":
                    url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/drums.wav";
                    break;
            }

            var bytes = File.ReadAllBytes(url);
            WAV wav = new WAV(bytes); //Use WAV class to convert audio file
            AudioClip audioClip = AudioClip.Create(track, wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);

            orbs[i].clip = audioClip;
        }
    }

}

