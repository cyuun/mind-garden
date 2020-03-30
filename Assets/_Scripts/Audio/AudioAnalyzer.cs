using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RhythmTool;

public class AudioAnalyzer : MonoBehaviour
{
    public static AudioAnalyzer S;
    public static float BPM = 0f;

    public bool _active = true;

    public RhythmData rhythmData;
    public RhythmAnalyzer analyzer;
    public AudioSource audioSource;
    public RhythmEventProvider eventProvider;

    private float prevTime;
    private RhythmPlayer songPlayer;
    private List<Beat> beats;
    private List<Onset> onsets;
    private List<Value> segments;
    private Track<Value> segmenter;

    void Awake()
    {
        beats = new List<Beat>();
        onsets = new List<Onset>();
        segments = new List<Value>();
        //Get song from spleeter
        if (Global.currentSongInfo != null) audioSource.clip = Global.currentSongInfo.inputSong;
        else if(AudioPeerRoot.S) audioSource.clip = AudioPeerRoot.S.defaultSong;
        audioSource.Play();

    }

    void Start()
    {
        S = this;

        songPlayer = audioSource.GetComponent<RhythmPlayer>();
        eventProvider.Register<Beat>(OnBeat);
        eventProvider.Register<Onset>(OnOnset);
        eventProvider.Register<Value>(OnSegment);
        rhythmData = analyzer.Analyze(audioSource.clip);
        songPlayer.rhythmData = rhythmData;
        segmenter = rhythmData.GetTrack<Value>("Segments");
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            float time = audioSource.time;
            beats.Clear();
            onsets.Clear();
            segments.Clear();
            rhythmData.GetFeatures<Beat>(beats, prevTime, time);
            segmenter.GetFeatures(segments, prevTime, time);
            if (segments.Count > 0) //Implement Segment change functions here
            {
                if (SkyFractal.S) SkyFractal.S.ChangeOutline();
            }
            prevTime = time;
            if (time >= audioSource.clip.length)
            {
                EndGame();
            }

        }
    }

    void OnBeat(Beat beat)
    {
        if(ColorController.S) ColorController.S.ChangeColors();
        if(magic.S) magic.S.Stretch();
        BPM = beat.bpm * Global.pitch;
        Global.bpm = BPM;
    }

    void OnOnset(Onset onset)
    {
        smallTree.ShakeAllTrees();
        desertPlantSmall.ShakeAllTrees();
    }

    void OnSegment(Value val)
    {
        //print(val.value);
    }

    void OnDestroy()
    {
        eventProvider.Unregister<Beat>(OnBeat);
        eventProvider.Unregister<Onset>(OnOnset);
        eventProvider.Unregister<Value>(OnSegment);
    }

    public float GetBPM()
    {
        float t0 = audioSource.clip.length / 4;
        float t1 = audioSource.clip.length * 3 / 4;
        List<Beat> allBeats = new List<Beat>();
        rhythmData.GetFeatures<Beat>(allBeats, t0, t1);
        float BPM = 0f;
        if(allBeats.Count > 0)
        {
            foreach (Beat b in allBeats)
            {
                BPM += b.bpm;
            }
            BPM /= allBeats.Count;
        }
        return BPM;
    }

    public void AnalyzeClip(AudioClip clip)
    {
        rhythmData = analyzer.Analyze(audioSource.clip);
        songPlayer.rhythmData = rhythmData;
        segmenter = rhythmData.GetTrack<Value>("Segments");

    }

    void EndGame()
    {
        GameHUD.S.Exit();
    }
}
