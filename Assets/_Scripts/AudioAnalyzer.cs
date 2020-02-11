using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmTool;

public class AudioAnalyzer : MonoBehaviour
{
    public RhythmData rhythmData;
    public RhythmAnalyzer analyzer;
    public AudioSource audioSource;
    public RhythmEventProvider eventProvider;

    private float prevTime;
    private RhythmPlayer songPlayer;
    private List<Beat> beats;
    private List<Segmenter> segmenters;

    void Awake()
    {
        beats = new List<Beat>();
        segmenters = new List<Segmenter>();
    }

    void Start()
    {
        //Get song from spleeter
        audioSource.clip = SpleeterProcess.S.inputSong;
        audioSource.Play();

        songPlayer = audioSource.GetComponent<RhythmPlayer>();
        eventProvider.Register<Beat>(OnBeat);
        rhythmData = analyzer.Analyze(audioSource.clip);
        songPlayer.rhythmData = rhythmData;
        Track<Beat> track = rhythmData.GetTrack<Beat>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float time = audioSource.time;
        beats.Clear();
        rhythmData.GetFeatures<Beat>(beats, prevTime, time);
        
    }

    void OnBeat(Beat beat)
    {
        Debug.Log(beat.bpm);
    }

    void OnDestroy()
    {
        eventProvider.Unregister<Beat>(OnBeat);
    }
}
