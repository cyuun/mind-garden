using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
    private static CameraScreenshot S;
    private Camera myCamera;
    private bool takeScreenshot;
    private int screenshotNum;

    void Awake()
    {
        S = this;
        myCamera = gameObject.GetComponent<Camera>();
    }

    void Start()
    {
        screenshotNum = (Directory.GetFiles(Application.persistentDataPath + "/Screenshots/")).Length;
    }

    private void OnPostRender()
    {
        if (takeScreenshot)
        {
            takeScreenshot = false;
            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            renderResult.ReadPixels(rect, 0, 0);

            if(!Directory.Exists(Application.persistentDataPath + "/Screenshots/"))
            { 
                Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots/");
            }
            byte[] bytes = renderResult.EncodeToPNG();
            string output = Application.persistentDataPath + "/Screenshots/" + System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".png";
            File.WriteAllBytes(output, bytes);
            screenshotNum++;

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
            if (!SettingsMenu.S.resumed)
            {
                GameHUD.S.FlashColor("White");
                SettingsMenu.S.resumed = false;

            }
            else
            {
                SettingsMenu.S.resumed = false;
            }
        }
    }

    private void TakeScreenshot(int width, int height)
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshot = true;
    }

    public static void TakeScreenshot_Static(int width, int height)
    {
        S.TakeScreenshot(width, height);
    }
}
