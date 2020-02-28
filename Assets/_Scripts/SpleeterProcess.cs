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
    public AudioSource[] audioSources;
    public bool callSpleeter;
    public bool playOnAwake;

    private Process theProcess;
    bool processing;

    void Awake()
    {
        S = this;

        if (callSpleeter) 
        {
            CallSpleeter();
        }

        //DontDestroyOnLoad(this.gameObject);

        //Play
        /*if (playOnAwake)
        {
            foreach (AudioSource audio in Global.orbs)
            {
                audio.Play();
            }

        }*/
    }

    private void Update()
    {
        if (theProcess != null && theProcess.HasExited)
        {
            StartCoroutine(ExitSpleeter());
            theProcess = null;
        }
    }

    public void CallSpleeter()
    {
        if (Global.inputSongPath != null && Global.inputSong != null)
        {
            inputSongPath = Global.inputSongPath;
            inputSong = Global.inputSong;
            inputSong.name = Path.GetFileNameWithoutExtension(inputSongPath);
        }
        else
        {
            //inputSongPath = AssetDatabase.GetAssetPath(inputSong); //Comment out in actual build
        }

        //Use Regex to parse out illegal characters
        string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        string oldName = inputSong.name;
        inputSong.name = r.Replace(inputSong.name, "");
        inputSong.name = inputSong.name.Replace(" ", string.Empty);
        if (inputSong.name != oldName)
        {
            File.Move(inputSongPath, Path.Combine(Path.GetDirectoryName(inputSongPath), inputSong.name + Path.GetExtension(inputSongPath))); //Renames song without illegal characters
        }
        inputSongPath = Path.Combine(Path.GetDirectoryName(inputSongPath), inputSong.name + Path.GetExtension(inputSongPath));


        Process process = new Process();
        // Configure the process using the StartInfo properties.
        string filePath = Application.streamingAssetsPath + "/spleeter/spleeter/"; //Current Directory plus song path
        //string outputPath = Application.persistentDataPath + "/Spleets/";
        string outputPath = Application.dataPath + "/Resources/Spleets/";
        process.StartInfo.FileName = filePath + "spleeter.exe";
        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        process.StartInfo.Arguments = "separate -i " + inputSongPath + " -p spleeter:4stems -o \"" + outputPath + "\""; //Shell executable
        process.EnableRaisingEvents = true; //Needed for detecting exited event
        //process.Exited += Process_Exited;
        MenuController.S.loadScreen.SetActive(true);
        theProcess = process;
        process.Start();
        //process.WaitForExit();

    }

    private void Process_Exited(object sender, System.EventArgs e)
    {
        MenuController.S.loadScreen.SetActive(false);
        throw new System.NotImplementedException();
    }

    public SongInfo LoadSongTracks(string songPath, AudioClip inputSong)
    {
        if(songPath != null && inputSong)
        {
            SongInfo song = new SongInfo();
            song.inputSongPath = songPath;
            song.inputSong = inputSong;
            song.songName = inputSong.name;
            string track = "";
            string url = "";
            for (int i = 0; i < audioSources.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/other.wav";
                        song.melody = url;
                        track = "Melody";
                        break;
                    case 1:
                        url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/bass.wav";
                        song.bass = url;
                        track = "Bass";
                        break;
                    case 2:
                        url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/vocals.wav";
                        song.vocals = url;
                        track = "Vocals";
                        break;
                    case 3:
                        url = Application.persistentDataPath + "/Spleets/" + inputSong.name + "/drums.wav";
                        song.drums = url;
                        track = "Drums";
                        break;
                }


            }
            return song;

        }
        else
        {
            return new SongInfo();
        }

    }

    IEnumerator ExitSpleeter()
    {
        MenuController.S.loadScreen.SetActive(false);
        Global.currentSongInfo = LoadSongTracks(inputSongPath, inputSong);
        MenuController.S.AddSong(Global.currentSongInfo);
        yield return null;
    }

}

