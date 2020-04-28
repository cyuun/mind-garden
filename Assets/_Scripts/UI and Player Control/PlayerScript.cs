using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Valve.VR;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript S;
    public SteamVR_Action_Boolean takeShot=null;
    public SteamVR_Action_Vibration vibrate;
    public FirstPersonController controller;

    Transform water;
    bool splashing = false;

    void Awake()
    {
        S = this;
    }

    private void Start()
    {
        controller = GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!SettingsMenu.S.gameIsPaused)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1) || takeShot.GetState(SteamVR_Input_Sources.LeftHand))
            {
                vibrate.Execute(0, 0.1f, 150, 75, SteamVR_Input_Sources.LeftHand);
                //SteamVR_Controller.Input([the index of the controller you want to vibrate]).TriggerHapticPulse([length in microseconds as ushort]);
                CameraScreenshot.TakeScreenshot_Static(Screen.width, Screen.height);
            }
        }

        if(water && controller.moving && !splashing)
        {
            splashing = true;
            StartCoroutine(SplishSplash(water));
        }
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
