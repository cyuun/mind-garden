using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript S;

    void Awake()
    {
        S = this;
    }

    private void Start()
    {
        Global.playingGame = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            CameraScreenshot.TakeScreenshot_Static(Screen.width, Screen.height);
        }
    }
}
