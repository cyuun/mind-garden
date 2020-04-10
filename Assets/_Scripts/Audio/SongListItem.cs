using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEditor;
using SimpleFileBrowser;


public class SongListItem : MonoBehaviour
{
    public static SongListItem currentSongPlaying;

    public SongInfo songInfo;
    public bool spleeterOn;
    const string RESOURCE_PATH = "Assets/Resources/Spleets/";
    
    //stuff used for importing on mac os
    public IntPtr handle_mpg;
    public IntPtr errPtr;
    public IntPtr rate;
    public IntPtr channels;
    public IntPtr encoding;
    public IntPtr id3v1;
    public IntPtr id3v2;
    public IntPtr done;
   
    public string mPath;
    public int x;
    public int intRate;
    public int intChannels;
    public int intEncoding;
    public int FrameSize;
    public int lengthSamples;
    public AudioClip myClip;  
   
    #region Consts: Standard values used in almost all conversions.
    private const float const_1_div_128_ = 1.0f / 128.0f;  // 8 bit multiplier
    private const float const_1_div_32768_ = 1.0f / 32768.0f; // 16 bit multiplier
    private const double const_1_div_2147483648_ = 1.0 / 2147483648.0; // 32 bit
    #endregion

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
            if (SystemInfo.operatingSystem.Contains("Windows"))
            {
                byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(inputSongPath);
                AudioClip audioClip = NAudioPlayer.FromMp3Data(bytes);
                inputSong = audioClip;
                return inputSong;
            }
            else
            {
                mPath = inputSongPath;
                return MacImport();
            }

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
    
    public AudioClip MacImport()
    {
        if(string.IsNullOrEmpty(mPath))
            mPath = EditorUtility.OpenFilePanel ("Open MP3", "", "mp3");
        
        
        MPGImport.mpg123_init ();
        handle_mpg = MPGImport.mpg123_new (null, errPtr);
        x = MPGImport.mpg123_open (handle_mpg, mPath);      
        MPGImport.mpg123_getformat (handle_mpg, out rate, out channels, out encoding);
        intRate = rate.ToInt32 ();
        intChannels = channels.ToInt32 ();
        intEncoding = encoding.ToInt32 ();
       
        MPGImport.mpg123_id3 (handle_mpg, out id3v1, out id3v2);      
        MPGImport.mpg123_format_none (handle_mpg);
        MPGImport.mpg123_format (handle_mpg, intRate, intChannels, 208);
       
        FrameSize = MPGImport.mpg123_outblock (handle_mpg);      
        byte[] Buffer = new byte[FrameSize];      
        lengthSamples = MPGImport.mpg123_length (handle_mpg);
               
        myClip = AudioClip.Create ("myClip", lengthSamples, intChannels, intRate, false, false);
       
        int importIndex = 0;
       
        while (0 == MPGImport.mpg123_read(handle_mpg, Buffer, FrameSize, out done)) {
               
           
            float[] fArray;
            fArray = ByteToFloat (Buffer);
                               
            myClip.SetData (fArray, (importIndex*fArray.Length)/2);
           
            importIndex++;                
        }          
       
        MPGImport.mpg123_close (handle_mpg);
        
        SongLibrary.libraryCreated = true;
        SongLibrary.S.gameObject.SetActive(true);
        
        return myClip;
    }
 
    public float[] IntToFloat (Int16[] from)
    {
        float[] to = new float[from.Length];
           
        for (int i = 0; i < from.Length; i++)
            to [i] = (float)(from [i] * const_1_div_32768_);
 
        return to;
    }
 
    public Int16[] ByteToInt16 (byte[] buffer)
    {
        Int16[] result = new Int16[1];
        int size = buffer.Length;
        if ((size % 2) != 0) {
            /* Error here */
            Console.WriteLine ("error");
            return result;
        } else {
            result = new Int16[size / 2];
            IntPtr ptr_src = Marshal.AllocHGlobal (size);
            Marshal.Copy (buffer, 0, ptr_src, size);
            Marshal.Copy (ptr_src, result, 0, result.Length);
            Marshal.FreeHGlobal (ptr_src);
            return result;
        }
    }
   
    public float[] ByteToFloat (byte[] bArray)
    {
        Int16[] iArray;      
           
        iArray = ByteToInt16 (bArray);
       
        return IntToFloat (iArray);
    }
}
