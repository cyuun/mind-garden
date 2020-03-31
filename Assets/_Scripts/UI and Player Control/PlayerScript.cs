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
    public bool spawned = false;
    TerrainScript terrain;

    private Vector3 _respawnLocation = Vector3.zero;

    void Awake()
    {
        S = this;

        _respawnLocation = transform.position;
    }

    private void Start()
    {
        controller = GetComponent<FirstPersonController>();
        StartCoroutine(SpawnTimer(1f));
        terrain = AudioPeerRoot.S.terrainScript;
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

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Water":
                water = other.transform;
                StartCoroutine(SplishSplash(water));
                break;

            case "Respawn":
                StartCoroutine(Respawn());
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

    IEnumerator SpawnTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        spawned = true;
    }

    private IEnumerator Respawn()
    {
        GameHUD.S.RespawnFade();
        yield return new WaitForSeconds(1);
        transform.position = _respawnLocation;
    }
}
