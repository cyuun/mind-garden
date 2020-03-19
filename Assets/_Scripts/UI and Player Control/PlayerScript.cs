using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript S;

    public FirstPersonController controller;

    Transform water;
    bool splashing = false;

    void Awake()
    {
        S = this;
    }

    private void Start()
    {
        Global.playingGame = true;
        controller = GetComponent<FirstPersonController>();
        StartCoroutine(DelayedSettingsUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        if (!SettingsMenu.S.gameIsPaused)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1))
            {
                CameraScreenshot.TakeScreenshot_Static(Screen.width, Screen.height);
            }
        }

        if(water && controller.moving && !splashing)
        {
            splashing = true;
            StartCoroutine(SplishSplash(water));
        }
    }

    IEnumerator DelayedSettingsUpdate()
    {
        yield return new WaitForSeconds(1);
        if (SettingsMenu.S) SettingsMenu.S.UpdateSettings();
        else yield return new WaitForSeconds(.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Water":
                water = other.transform;
                StartCoroutine(SplishSplash(water));
                break;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Water":
                water = null;
                break;

        }
    }

    IEnumerator SplishSplash(Transform water)
    {
        WaterScript w = water.GetComponent<WaterScript>();
        w.Splash(transform.position.x, transform.position.z);

        yield return new WaitForSeconds(.4f);
        if (this.water && controller.moving) StartCoroutine(SplishSplash(water));
        else splashing = false;
    }
}
