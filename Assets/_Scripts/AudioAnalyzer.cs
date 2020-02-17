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
    private List<Value> segments;
    private Track<Value> segmenter;

    void Awake()
    {
        beats = new List<Beat>();
        segments = new List<Value>();
    }

    void Start()
    {
        //Get song from spleeter
        audioSource.clip = SpleeterProcess.S.inputSong;
        audioSource.Play();

        songPlayer = audioSource.GetComponent<RhythmPlayer>();
        eventProvider.Register<Beat>(OnBeat);
        eventProvider.Register<Value>(OnSegment);
        rhythmData = analyzer.Analyze(audioSource.clip);
        songPlayer.rhythmData = rhythmData;
        Track<Beat> beatTrack = rhythmData.GetTrack<Beat>();
        segmenter = rhythmData.GetTrack<Value>("Segments");
    }

    // Update is called once per frame
    void Update()
    {
        float time = audioSource.time;
        beats.Clear();
        segments.Clear();
        rhythmData.GetFeatures<Beat>(beats, prevTime, time);
        segmenter.GetFeatures(segments, prevTime, time);
        if (segments.Count > 0) //Implement Segment change functions here
        {
            if (SkyFractal.S) SkyFractal.S.ChangeOutline();
            if (MainSpawner.S)  MainSpawner.S.ChangeSpawner();
        }

        prevTime = time;
    }

    void OnBeat(Beat beat)
    {
        //Debug.Log(beat.bpm);
    }

    void OnSegment(Value val)
    {
        //print(val.value);
    }

    void OnDestroy()
    {
        eventProvider.Unregister<Beat>(OnBeat);
        eventProvider.Unregister<Value>(OnSegment);
    }
}
