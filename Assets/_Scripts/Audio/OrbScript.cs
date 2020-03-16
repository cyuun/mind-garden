using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OrbScript : MonoBehaviour
{
    public static Global.BiomeType biomeSpawner;
    public static bool biomeChosen = false;

    AudioPeer audioPeer;
    bool following = false;
    bool moving = false;
    bool glowing = false;
    Transform target;
    Vector3 rotPerSecond;
    Rigidbody rb;
    Vector3 velocity;

    public bool _interactable = true;
    public bool _matchTerrainHeight = true;
    public bool soundOn;
    public AudioSource audioTrack;
    Image buttonLabel;
    public ParticleSystem particles;
    public ParticleSystem burst;
    public ParticleSystem glow;
    public float rotationSpeed;
    public float y_offset;
    public float speed = 10f;

    //Amplitude Flash
    public Gradient _colorGrad;
    public float _colorMultiplier;
    public Vector3 _targetScale;

    private Color _startColor, _endColor;
    private Color _emissionColor;
    private Vector3 _scale;
    private Renderer rend;

    //Biome Spawners
    public GameObject[] biomeSpawners;
    public bool spawnBiome;
    
    public TerrainScript terrainScript;
    public bool active = false;
    public bool flashOn = false;

    private GameObject spawner;
    private AudioMixer mixer;

    public void SpawnRocks()
    {
        if (!biomeChosen)
        {
            Random.InitState((int)System.DateTime.Now.Ticks); //Ensures randomness
            biomeSpawner = Global.BiomeType.jungle;//(Global.BiomeType)Random.Range(0, biomeSpawners.Length);
            Global.currentBiome = biomeSpawner;
            biomeChosen = true;
        }

        spawner = Instantiate(biomeSpawners[(int)biomeSpawner], transform.position, Quaternion.identity, transform);
        RockSpawner rockSpawner = spawner.GetComponent<RockSpawner>();
        rockSpawner.terrainScript = terrainScript;
        rockSpawner.SetParent();
        rockSpawner.GenerateRocks();

    }

    public void SpawnBiome()
    {
        if (spawner == null) SpawnRocks();
        ColorController.S.biomeType = biomeSpawner;
        TreeSpawner treeSpawner = spawner.GetComponent<TreeSpawner>();
        CreatureSpawner creatureSpawner = spawner.GetComponent<CreatureSpawner>();
        Skybox skybox = spawner.GetComponent<Skybox>();

        treeSpawner.terrainScript = terrainScript;
        treeSpawner.SetParent();
        treeSpawner.GenerateTrees();
        if (creatureSpawner != null)
        {
            creatureSpawner.terrainScript = terrainScript;
            creatureSpawner.SetParent();
            creatureSpawner.SpawnCreatures();
        }
        Camera.main.GetComponent<Skybox>().material = skybox.material;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocity = Vector3.one * speed;
        audioPeer = audioTrack.GetComponent<AudioPeer>();
        if (!soundOn)
        {
            audioTrack.volume = 0;
        }
        rotPerSecond = new Vector3(Random.Range(.5f, 2f), Random.Range(.5f, 2f), Random.Range(.5f, 2f));

        //Set Orb height to match terrain
        if (_matchTerrainHeight && active)
        {
            float y_pos = terrainScript.GetTerrainHeight(transform.position.x, transform.position.y);
            transform.position = new Vector3(transform.position.x, y_pos, transform.position.z);
        }

        //Set Up Amplitude Flash
        _startColor = new Color(0, 0, 0, 0);
        _endColor = new Color(0, 0, 0, 1);
        _scale = transform.localScale;
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Spin
        transform.rotation = Quaternion.Euler(rotPerSecond * Time.time * rotationSpeed);
        //Flash
        if (audioPeer && (following || flashOn)) Flash();

        if (following)
        {

            float distance = Vector3.Distance(transform.position, target.position);
            if(distance > 3)
            {
                transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, 1f);

            }
            if (!moving)
            {
                moving = true;
                StartCoroutine(MoveOrb());
            }
        }

        if (_matchTerrainHeight && active)
        {
            float y = terrainScript.GetTerrainHeight(transform.position.x, transform.position.z) + y_offset;
            if (transform.position.y < y)
            {
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
            }
        }
    }

    private void OnMouseOver()
    {
        if(Vector3.Distance(this.transform.position, Camera.main.transform.position) < 15f)
        {
            if (!glowing && !following)
            {
                glow.Play();
            }
            if (Input.GetKeyUp(KeyCode.Mouse0) && _interactable)
            {
                target = PlayerScript.S.transform;
                ToggleParticles();
                ToggleFollow();
                glow.Stop();
                glowing = false;
            }
            //ShowButtonLabel();
        }
    }

    private void OnMouseExit()
    {
        //buttonLabel.gameObject.SetActive(false);
        if (glowing)
        {
            glow.Stop();
            glowing = false;
            print("noglow");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pond" && _interactable)
        {
            audioTrack.transform.SetParent(target);
            audioTrack.spatialBlend = 0;
            SkyFractal.S.ChangeOutline();
            ColorController.S.ChangeBase();
            target = other.transform;

            if (!Global.callSpleeter)
            {
                print("Test:" + Global.callSpleeter);
                StartCoroutine(LowerVolume());
                StartCoroutine(RaiseVolume(20f));
            }
        }
        else if (other.tag == "Player")
        {
            if (!following) target = other.transform;
            ToggleParticles();
            ToggleFollow();
            glow.Stop();
            glowing = false;
        }
        else if (other.tag == "Rocks" && other.GetComponent<Rock>())
        {
            Vector3 pos = transform.position;
            pos.y = other.transform.position.y + other.GetComponent<Rock>().radius;
            transform.position = pos;
        }
        else if (other.tag == "Tree" && !following)
        {
            Vector3 pos = transform.position;
            pos += new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z).normalized;
            if (active) pos.y = terrainScript.GetTerrainHeight(pos.x, pos.z);
            transform.position = pos;
        }
    }

    void Flash()
    {
        _emissionColor = _colorGrad.Evaluate(audioPeer._amplitude);
        Color colorLerp = Color.Lerp(_startColor, _emissionColor * _colorMultiplier, audioPeer._amplitudeBuffer);
        rend.material.SetColor("_EmissionColor", colorLerp);
        colorLerp = Color.Lerp(_startColor, _endColor, audioPeer._amplitudeBuffer);
        rend.material.SetColor("_Color", colorLerp);

        transform.localScale = Vector3.Lerp(_scale, _targetScale, audioPeer._amplitudeBuffer);

    }

    void ShowButtonLabel()
    {
        Vector3 labelPos = Camera.main.WorldToScreenPoint(this.transform.position);
        buttonLabel.transform.position = labelPos + (new Vector3(1,1,0) * 30);
        buttonLabel.gameObject.SetActive(true);
    }

    void ToggleParticles()
    {
        burst.Play();
        particles.Play();
    }

    void ToggleFollow()
    {
        following = true;
    }

    public void ChangePlayerColors()
    {
        /*Color[] colors = TerrainScript.S._paint;
        List<Color> colorList = new List<Color>();
        for(int i = 0; i < colors.Length; i++)
        {
            Color c = _colorGrad.Evaluate(Random.Range(0f, 1f));
            colorList.Add(c);
        }
        TerrainScript.S._paint = colorList.ToArray();*/
    }

    IEnumerator MoveOrb()
    {
        rb.AddForce(new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z).normalized * 10);
        yield return new WaitForSeconds(1f);
        moving = false;
    }

    IEnumerator LowerVolume()
    {
        while(audioTrack.volume > 0)
        {
            audioTrack.volume -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RaiseVolume(float dB)
    {
        float targetVolume;
        mixer.GetFloat("masterVol", out targetVolume);
        float currentVolume = targetVolume;
        targetVolume += dB;

        while (currentVolume < targetVolume)
        {
            mixer.SetFloat("masterVol", currentVolume);
            mixer.GetFloat("masterVol", out currentVolume);
            currentVolume += Time.deltaTime;
            yield return null;
        }
        mixer.SetFloat("masterVol", targetVolume);
    }
}
