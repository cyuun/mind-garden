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

public class MenuController : MonoBehaviour
{
    public static MenuController S;
    float firstScreenPos, secondScreenPos, thirdScreenPos;
    public Button fileSelection, play, gallery, settings;
    public Text errorMessage;
    bool _errorFading = false;
    bool _cameraMoving = false;
    public float moveDuration = 1;
    public float moveDistance = 20;

    public GameObject head;
    public AudioSource backgroundMusic;



    void Awake()
    {
        S = this;

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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExploreFiles()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void StartGame()
    {
        if (Global.inputSong != null && Global.inputSongPath != null)
        {
            SceneManager.LoadScene(1); //Loads Main Game Scene
        }
        else
        {
            if (!_errorFading)
            {
                StartCoroutine(ShowErrorMessage());
            }
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
            backgroundMusic.Play();
        }

    }

}
