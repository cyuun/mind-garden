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
        if (inputSong.name != oldName && !File.Exists(Path.Combine(Path.GetDirectoryName(inputSongPath), inputSong.name + Path.GetExtension(inputSongPath))))
        {
            File.Move(inputSongPath, Path.Combine(Path.GetDirectoryName(inputSongPath), inputSong.name + Path.GetExtension(inputSongPath))); //Renames song without illegal characters
        }
        inputSongPath = Path.Combine(Path.GetDirectoryName(inputSongPath), inputSong.name + Path.GetExtension(inputSongPath));

        Process process = new Process();
        // Configure the process using the StartInfo properties.
        string filePath = Application.streamingAssetsPath + "/spleeter/spleeter/"; //Current Directory plus song path
        //string outputPath = Application.persistentDataPath + "/Spleets/"; //Use this output for actual song imports
        string outputPath = Application.dataPath + "/Resources/Spleets/"; //Use this output when importing song resources
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
            string filePath = "";
            if (MenuController.S.songNames.Contains(song.songName)) filePath = "Assets/Resources";
            else filePath = Application.persistentDataPath;
            for (int i = 0; i < audioSources.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        song.melody = filePath + "/Spleets/" + inputSong.name + "/other.wav";
                        break;
                    case 1:
                        song.bass = filePath + "/Spleets/" + inputSong.name + "/bass.wav";
                        break;
                    case 2:
                        song.vocals = filePath + "/Spleets/" + inputSong.name + "/vocals.wav";
                        break;
                    case 3:
                        song.drums = filePath + "/Spleets/" + inputSong.name + "/drums.wav";
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
        //Copy file for resource access
        if (Directory.Exists("Assets/Resources/Spleets/" + inputSong.name) && !File.Exists(Path.Combine("Assets/Resources/Spleets/" + inputSong.name, inputSong.name + Path.GetExtension(inputSongPath))))
        {
            File.Copy(inputSongPath, Path.Combine("Assets/Resources/Spleets/" + inputSong.name, inputSong.name + Path.GetExtension(inputSongPath)));
        }
        if (Directory.Exists(Application.persistentDataPath + "/Spleets/" + inputSong.name) && !File.Exists(Path.Combine(Application.persistentDataPath + "/Spleets/" + inputSong.name, inputSong.name + Path.GetExtension(inputSongPath))))
        {
            File.Copy(inputSongPath, Path.Combine(Application.persistentDataPath + "/Spleets/" + inputSong.name, inputSong.name + Path.GetExtension(inputSongPath)));
        }

        MenuController.S.loadScreen.SetActive(false);
        Global.currentSongInfo = LoadSongTracks(inputSongPath, inputSong);
        MenuController.S.AddSong(Global.currentSongInfo);
        yield return null;
    }

}

