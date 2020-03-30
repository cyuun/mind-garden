using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using SimpleFileBrowser;
using System.Text.RegularExpressions;

public class MenuController : MonoBehaviour
{
    public static MenuController S;
    public Button fileSelection, play, gallery, settings;
    public Text errorMessage;
    bool _errorFading = false;
    public bool _cameraMoving = false;
    public float moveDuration = 1;
    public float moveDistance = 20;
    public GameObject songList;
    [Header("Resource Folders to Import")]
    [SerializeField]
    public List<string> songNames = new List<string>();
    public GameObject songItemPrefab;
    public GameObject head;
    public GameObject loadScreen;
    public GameObject startButton;
    public AudioSource backgroundMusic;
    public AudioSource silentMusic;


    void Awake()
    {
        S = this;
        Cursor.lockState = CursorLockMode.None;

        if(!fileSelection || !play || !gallery || !settings || !errorMessage)
        {
            //TODO: These finds don't work
            fileSelection = transform.Find("Text").GetComponent<Button>();
            play = transform.Find("Play").GetComponent<Button>();
            gallery = transform.Find("Gallery").GetComponent<Button>();
            settings = transform.Find("Settings").GetComponent<Button>();
            errorMessage = transform.Find("Error").GetComponent<Text>();
        }

    }

    void Start()
    {

        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Music", ".mp3", ".wav"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".mp3");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        songList = SongLibrary.S.listObject;
        if(!SongLibrary.libraryCreated) StartCoroutine(LoadSongLibrary());
        else SongLibrary.S.gameObject.SetActive(true);


        if (Global.playingGame == true)
        {
            SettingsMenu.S.UpdateSettings();
            Global.playingGame = false;
        }
        else
        {
            SettingsMenu.S.UpdateGlobalSettings();
        }
    }

    public void AddSong(SongInfo info, bool spleeter)
    {
        int totalSongs = songList.transform.childCount;
        Vector3 firstSong = songList.transform.GetChild(totalSongs - 1).position;
        foreach (Transform t in songList.transform)
        {
            Vector3 pos = t.localPosition;
            pos.y -= t.GetComponent<RectTransform>().rect.height;
            t.localPosition = pos;
        }
        GameObject newSong = Instantiate(songItemPrefab, firstSong, Quaternion.identity, songList.transform);

        string spleeterIcon = "";
        if (spleeter)
        {
            spleeterIcon = "[S]";
            newSong.GetComponent<SongListItem>().spleeterOn = true;
        }
        newSong.GetComponent<Text>().text = spleeterIcon + info.songName;
        newSong.GetComponent<SongListItem>().songInfo = info;

        silentMusic.clip = info.inputSong;
        silentMusic.Play();
        AudioAnalyzer.S.AnalyzeClip(info.inputSong);
    }

    public void ExploreFiles()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
        
    }

    public void Play()
    {
        if (Global.currentSongInfo != null && !_cameraMoving)
        {
            LevelSelect.S.Fade();
            StartCoroutine(ZoomCamera(moveDuration));
            //StartCoroutine(ShowStartButton());
            StartCoroutine(ZoomHead(moveDuration));
        }
        else
        {
            if (!_errorFading)
            {
                StartCoroutine(ShowErrorMessage());
            }
        }
    }

    public void StartGame()
    {
        if(Global.currentSongInfo != null)
        {
            Global.playingGame = true;
            LevelSelect.S.QuickUpdate();
            SongLibrary.S.gameObject.SetActive(false);
            startButton.SetActive(false);
            loadScreen.SetActive(true);
            StartCoroutine(LoadBar());
        }
    }

    IEnumerator LoadBar()
    {
        yield return new WaitForSeconds(.1f);
        LoadingBar.S.Show(SceneManager.LoadSceneAsync(1));

    }

    public void GoBack()
    {
        if (!_cameraMoving)
        {
            LevelSelect.S.Fade();
            StartCoroutine(BackUpCamera(moveDuration));
            //StartCoroutine(HideStartButton());
            StartCoroutine(BackUpHead(moveDuration));
        }
    }

    public void Home()
    {
        if(!_cameraMoving) StartCoroutine(ViewHome(moveDuration));

    }

    public void Gallery()
    {
        if (!_cameraMoving) StartCoroutine(ViewGallery(moveDuration));
    }

    public void Settings()
    {
        if (!_cameraMoving) StartCoroutine(ViewSettings(moveDuration));
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void ChangeMusic(string path)
    {
        fileSelection.GetComponent<Text>().text = path;
    }

    IEnumerator ViewHome(float duration)
    {
        _cameraMoving = true;
        Transform camTransform = Camera.main.transform;
        float startingRot = camTransform.eulerAngles.y;
        if (startingRot > 180) startingRot -= 360;
        float endRot = 0f;
        float tempRot = startingRot;
        for (float t = 0; t <= 1; t += (Time.deltaTime / duration))
        {
            tempRot = Mathf.SmoothStep(startingRot, endRot, t);
            Camera.main.transform.rotation = Quaternion.Euler(0, tempRot, 0);
            yield return null;
        }
        Camera.main.transform.rotation = Quaternion.Euler(0, endRot, 0);
        _cameraMoving = false;

        /*float startingPos = transform.position.x;
        float endPos = 0;
        Vector3 tempPos = transform.position;
        for (float t = 0; t < 1; t += (.01f / duration))
        {
            tempPos.x = Mathf.SmoothStep(startingPos, endPos, t); //Negative target value because we're really offseting the entire canvas by that value
            transform.position = tempPos;
            yield return new WaitForSeconds(.01f);
        }*/
    }

    IEnumerator ZoomCamera(float duration)
    {
        _cameraMoving = true;
        Transform camTransform = Camera.main.transform;
        float startPos = camTransform.position.z;
        float endPos = startPos + moveDistance;
        float tempPos = startPos;
        for (float t = 0; t <= 1; t += (Time.deltaTime / duration))
        {
            tempPos = Mathf.SmoothStep(startPos, endPos, t);
            Camera.main.transform.position = new Vector3(camTransform.position.x, camTransform.position.y, tempPos);
            yield return null;
        }
        Camera.main.transform.position = new Vector3(camTransform.position.x, camTransform.position.y, endPos);
        _cameraMoving = false;
    }

    IEnumerator ZoomHead(float duration)
    {
        Transform headT = head.transform;
        float start = headT.position.z;
        Quaternion startRot = headT.rotation;
        float rot1 = startRot.eulerAngles.x;
        float end = start - 300;
        float rot2 = rot1 + 10;
        float temp = start;
        for (float t = 0; t <= 1; t += (Time.deltaTime / duration))
        {
            temp = Mathf.SmoothStep(start, end, t);
            head.transform.position = new Vector3(headT.position.x, headT.position.y, temp);
            temp = Mathf.SmoothStep(rot1, rot2, t);
            head.transform.rotation = Quaternion.Euler(temp, startRot.eulerAngles.y, startRot.eulerAngles.z);
            yield return null;
        }
        head.transform.position = new Vector3(headT.position.x, headT.position.y, end);
        head.transform.rotation = Quaternion.Euler(rot2, startRot.eulerAngles.y, startRot.eulerAngles.z);
    }

    IEnumerator BackUpCamera(float duration)
    {
        _cameraMoving = true;
        Transform camTransform = Camera.main.transform;
        float startPos = camTransform.position.z;
        float endPos = startPos - moveDistance;
        float tempPos = startPos;
        for (float t = 0; t <= 1; t += (Time.deltaTime / duration))
        {
            tempPos = Mathf.SmoothStep(startPos, endPos, t);
            Camera.main.transform.position = new Vector3(camTransform.position.x, camTransform.position.y, tempPos);
            yield return null;
        }
        Camera.main.transform.position = new Vector3(camTransform.position.x, camTransform.position.y, endPos);
        _cameraMoving = false;
    }

    IEnumerator BackUpHead(float duration)
    {
        Transform headT = head.transform;
        float start = headT.position.z;
        Quaternion startRot = headT.rotation;
        float rot1 = startRot.eulerAngles.x;
        float end = start + 300;
        float rot2 = rot1 - 10;
        float temp = start;
        for (float t = 0; t <= 1; t += (Time.deltaTime / duration))
        {
            print(head.transform.rotation.eulerAngles);
            temp = Mathf.SmoothStep(start, end, t);
            head.transform.position = new Vector3(headT.position.x, headT.position.y, temp);
            temp = Mathf.SmoothStep(rot1, rot2, t);
            head.transform.rotation = Quaternion.Euler(temp, startRot.eulerAngles.y, startRot.eulerAngles.z);
            yield return null;
        }
        head.transform.position = new Vector3(headT.position.x, headT.position.y, end);
        head.transform.rotation = Quaternion.Euler(rot2, startRot.eulerAngles.y, startRot.eulerAngles.z);
    }

    IEnumerator ViewGallery(float duration)
    {
        _cameraMoving = true;
        Transform camTransform = Camera.main.transform;
        float startingRot = camTransform.eulerAngles.y;
        float endRot = 90f;
        float tempRot = startingRot;
        for (float t = 0; t <= 1; t += (Time.deltaTime/ duration))
        {
            tempRot = Mathf.SmoothStep(startingRot, endRot, t);
            Camera.main.transform.rotation = Quaternion.Euler(0, tempRot, 0);
            yield return null;
        }
        Camera.main.transform.rotation = Quaternion.Euler(0, endRot, 0);
        _cameraMoving = false;

        /*float startingPos = transform.position.x;
        float endPos = -moveDistance;
        Vector3 tempPos = transform.position;
        for (float t = 0; t < 1; t += (.01f / duration))
        {
            tempPos.x = Mathf.SmoothStep(startingPos, endPos, t); //Negative target value because we're really offseting the entire canvas by that value
            transform.position = tempPos;
            yield return new WaitForSeconds(.01f);
        }*/
    }

    IEnumerator ViewSettings(float duration)
    {
        _cameraMoving = true;
        Transform camTransform = Camera.main.transform;
        float startingRot = camTransform.eulerAngles.y;
        float endRot = -90f;
        float tempRot = startingRot;
        for (float t = 0; t <= 1; t += (Time.deltaTime / duration))
        {
            tempRot = Mathf.SmoothStep(startingRot, endRot, t);
            Camera.main.transform.rotation = Quaternion.Euler(0, tempRot, 0);
            yield return null;
        }
        Camera.main.transform.rotation = Quaternion.Euler(0, endRot, 0);
        _cameraMoving = false;

        /*float startingPos = transform.position.x;
        float endPos = moveDistance;
        Vector3 tempPos = transform.position;
        for (float t = 0; t < 1; t += (.01f / duration))
        {
            tempPos.x = Mathf.SmoothStep(startingPos, endPos, t); //Negative target value because we're really offseting the entire canvas by that value
            transform.position = tempPos;
            yield return new WaitForSeconds(.01f);
        }*/
    }

    IEnumerator ShowStartButton()
    {
        Transform tr = startButton.transform;
        float startPos = tr.position.y;
        float endPos = startPos + 6;
        float tempPos = startPos;
        for (float t = 0; t <= 1; t += (Time.deltaTime / 1))
        {
            tempPos = Mathf.SmoothStep(startPos, endPos, t);
            startButton.transform.position = new Vector3(tr.position.x, tempPos, tr.position.z);
            yield return null;
        }
        startButton.transform.position = new Vector3(tr.position.x, endPos, tr.position.z);
    }

    IEnumerator HideStartButton()
    {
        Transform tr = startButton.transform;
        float startPos = tr.position.y;
        float endPos = startPos - 6;
        float tempPos = startPos;
        for (float t = 0; t <= 1; t += (Time.deltaTime / 1))
        {

            tempPos = Mathf.SmoothStep(startPos, endPos, t);
            startButton.transform.position = new Vector3(tr.position.x, tempPos, tr.position.z);
            yield return null;
        }
        startButton.transform.position = new Vector3(tr.position.x, endPos, tr.position.z);
    }

    IEnumerator ShowErrorMessage()
    {
        _errorFading = true;
        errorMessage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(2f);
        errorMessage.CrossFadeColor(new Color(0, 0, 0, 0), 1f, true, true);
        yield return new WaitForSeconds(1f);
        errorMessage.GetComponent<CanvasRenderer>().SetAlpha(1);
        errorMessage.color = new Color(0, 0, 0, 0);
        _errorFading = false;
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        UnityEngine.Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (FileBrowser.Success)
        {
            Global.inputSongPath = FileBrowser.Result;
            // If a file was chosen, read its bytes via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            if (Path.GetExtension(Global.inputSongPath) == ".mp3")
            {
                byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result);
                backgroundMusic.clip = NAudioPlayer.FromMp3Data(bytes);
            }
            else if (Path.GetExtension(Global.inputSongPath) == ".wav")
            {
                byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result);
                WAV wav = new WAV(bytes);

                AudioClip audioClip;
                audioClip = AudioClip.Create("Audio File Name", wav.SampleCount, 1, wav.Frequency, false);
                audioClip.SetData(wav.LeftChannel, 0);
                backgroundMusic.clip = audioClip;
            }
            else
            {
                UnityEngine.Debug.Log("Can't Read File");
                yield break;
            }
            backgroundMusic.transform.parent.gameObject.SetActive(true); //Turn on orb if off
            Global.inputSong = backgroundMusic.clip;

            if (Global.inputSongPath != null && Global.inputSong != null && Global.callSpleeter)
            {
                SpleeterProcess.S.CallSpleeter();
                backgroundMusic.Play();

            }
            else if (Global.inputSongPath != null && Global.inputSong != null)
            {
                print("Spleeter Disabled: Game will be played in normal mode");
                AudioClip inputSong = Global.inputSong;
                string songPath = Global.inputSongPath;
                inputSong.name = Path.GetFileNameWithoutExtension(songPath);
                //Use Regex to parse out illegal characters
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                string oldName = inputSong.name;
                inputSong.name = r.Replace(inputSong.name, "");
                inputSong.name = inputSong.name.Replace(" ", string.Empty);
                if (inputSong.name != oldName && !File.Exists(Path.Combine(Path.GetDirectoryName(songPath), inputSong.name + Path.GetExtension(songPath))))
                {
                    File.Move(songPath, Path.Combine(Path.GetDirectoryName(songPath), inputSong.name + Path.GetExtension(songPath))); //Renames song without illegal characters
                }
                songPath = Path.Combine(Path.GetDirectoryName(songPath), inputSong.name + Path.GetExtension(songPath));

                string outputPath = Application.persistentDataPath + "/Songs/";
                if (Directory.Exists(outputPath))
                {
                    if (!File.Exists(Path.Combine(outputPath, inputSong.name + Path.GetExtension(songPath))))
                    {
                        File.Copy(songPath, Path.Combine(outputPath, inputSong.name + Path.GetExtension(songPath)));

                    }
                }
                else 
                {
                    print("Directory not found");
                    Directory.CreateDirectory(outputPath);
                    File.Copy(songPath, Path.Combine(outputPath, inputSong.name + Path.GetExtension(songPath)));

                }
                AddSong(Global.currentSongInfo, false);

                Global.currentSongInfo = SpleeterProcess.S.LoadSong(songPath, inputSong);
                backgroundMusic.Play();

            }
        }

    }

    IEnumerator LoadSongLibrary()
    {
        loadScreen.SetActive(true);
        loadScreen.transform.Find("Cancel").gameObject.SetActive(false);
        //Load Resources (songs prepared with spleeter)
        string dir = "Assets/Resources/Spleets/";
        /*if (Directory.Exists(dir))
        {
            foreach (string file in Directory.EnumerateDirectories(dir))
            {
                string songName = "";
                foreach (string f in Directory.GetFiles(file))
                {
                    if (file.Substring(dir.Length) == Path.GetFileNameWithoutExtension(f))
                    {
                        songName = Path.GetFileName(f);
                    }
                }
                SongInfo info = new SongInfo();
                info.inputSongPath = Path.Combine(file, songName);
                if (Path.GetExtension(info.inputSongPath) == ".mp3")
                {
                    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(info.inputSongPath);
                    AudioClip audioClip = NAudioPlayer.FromMp3Data(bytes);
                    info.inputSong = audioClip;
                }
                else if (Path.GetExtension(info.inputSongPath) == ".wav")
                {
                    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(info.inputSongPath);
                    WAV wav = new WAV(bytes);

                    AudioClip audioClip;
                    audioClip = AudioClip.Create(songName, wav.SampleCount, 1, wav.Frequency, false);
                    audioClip.SetData(wav.LeftChannel, 0);
                    info.inputSong = audioClip;
                }

                info.inputSong.name = Path.GetFileNameWithoutExtension(Path.GetFileName(info.inputSongPath));
                info.songName = info.inputSong.name;
                info.melody = Path.Combine(dir, songName) + "/other.wav";
                info.bass = Path.Combine(dir, songName) + "/bass.wav";
                info.vocals = Path.Combine(dir, songName) + "/vocals.wav";
                info.drums = Path.Combine(dir, songName) + "/drums.wav";
                AddSong(info);
            }

        }*/

        //Load Imports from Persistent Data Path
        dir = Application.persistentDataPath + "/Spleets/";
        if (Directory.Exists(dir))
        {
            foreach (string file in Directory.EnumerateDirectories(dir))
            {
                string songName = "";
                foreach (string f in Directory.GetFiles(file))
                {
                    if (file.Substring(dir.Length) == Path.GetFileNameWithoutExtension(f))
                    {
                        songName = f;
                    }
                }
                SongInfo info = new SongInfo();
                info.inputSongPath = Path.Combine(file, songName);

                yield return null;

                if (Path.GetExtension(info.inputSongPath) == ".mp3")
                {
                    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(info.inputSongPath);
                    AudioClip audioClip = NAudioPlayer.FromMp3Data(bytes);
                    info.inputSong = audioClip;
                }
                else if (Path.GetExtension(info.inputSongPath) == ".wav")
                {
                    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(info.inputSongPath);
                    WAV wav = new WAV(bytes);

                    AudioClip audioClip;
                    audioClip = AudioClip.Create(songName, wav.SampleCount, 1, wav.Frequency, false);
                    audioClip.SetData(wav.LeftChannel, 0);
                    info.inputSong = audioClip;
                }

                yield return null;

                info.inputSong.name = Path.GetFileNameWithoutExtension(Path.GetFileName(info.inputSongPath));
                info.songName = info.inputSong.name;
                info.melody = Path.Combine(dir, songName) + "/other.wav";
                info.bass = Path.Combine(dir, songName) + "/bass.wav";
                info.vocals = Path.Combine(dir, songName) + "/vocals.wav";
                info.drums = Path.Combine(dir, songName) + "/drums.wav";
                AddSong(info, true);
                yield return null;

            }
        }

        dir = Application.persistentDataPath + "/Songs/";
        if (Directory.Exists(dir))
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                SongInfo info = new SongInfo();
                info.inputSongPath = file;
                string songName = Path.GetFileNameWithoutExtension(file);

                yield return null;

                if (Path.GetExtension(info.inputSongPath) == ".mp3")
                {
                    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(info.inputSongPath);
                    AudioClip audioClip = NAudioPlayer.FromMp3Data(bytes);
                    info.inputSong = audioClip;
                }
                else if (Path.GetExtension(info.inputSongPath) == ".wav")
                {
                    byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(info.inputSongPath);
                    WAV wav = new WAV(bytes);

                    AudioClip audioClip;
                    audioClip = AudioClip.Create(songName, wav.SampleCount, 1, wav.Frequency, false);
                    audioClip.SetData(wav.LeftChannel, 0);
                    info.inputSong = audioClip;
                }

                yield return null;

                info.inputSong.name = Path.GetFileNameWithoutExtension(Path.GetFileName(info.inputSongPath));
                info.songName = info.inputSong.name;
                AddSong(info, false);
                yield return null;

            }

        }
        loadScreen.transform.Find("Cancel").gameObject.SetActive(true);
        loadScreen.SetActive(false);
        SongLibrary.libraryCreated = true;
        SongLibrary.S.gameObject.SetActive(true);
    }

}
