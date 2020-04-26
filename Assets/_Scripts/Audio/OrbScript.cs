using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OrbScript : MonoBehaviour
{
    public static Global.BiomeType biomeSpawner;
    public static bool biomeChosen = false;
    public static GameObject magic;
    public static GameObject lightRing;
    public static int orbsFound = 0;

    AudioPeer audioPeer;
    bool following = false;
    bool moving = false;
    bool glowing = false;
    Transform target;
    Vector3 rotPerSecond;
    Rigidbody rb;
    Vector3 velocity;

    bool found = false;
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
    public Transform pondSpot;

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
    public AudioMixer mixer;

    public void SpawnRocks()
    {
        if (!biomeChosen)
        {
            Random.InitState((int)System.DateTime.Now.Ticks); //Ensures randomness
            if (!Global.biomeChosen) biomeSpawner = (Global.BiomeType)Random.Range(0, biomeSpawners.Length);
            else biomeSpawner = Global.currentBiome;
            Global.currentBiome = biomeSpawner;
            biomeChosen = true;
        }
        spawner = Instantiate(biomeSpawners[(int)biomeSpawner], transform.position, Quaternion.identity, transform);
        if(magic == null) magic = Instantiate(spawner.GetComponent<MagicSpawner>().magicPrefab, AudioPeerRoot.S.transform);
        RockSpawner rockSpawner = spawner.GetComponent<RockSpawner>();
        rockSpawner.terrainScript = terrainScript;
        rockSpawner.SetParent();
        rockSpawner.GenerateRocks();

    }

    public void SpawnBiome()
    {
        if (spawner == null) spawner = Instantiate(biomeSpawners[(int)biomeSpawner], transform.position, Quaternion.identity, transform);
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

    public void RandomizePosition()
    {
        float minDistance = 50f;
        float minRand = 40f;
        float maxRand = 110f;
        Vector3 offset = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y).normalized;
        offset *= Random.Range(minRand, maxRand);
        Vector3 pos = AudioPeerRoot.S.transform.position + offset;
        float distanceToEdge = (Mathf.Pow(pos.x, 2) / Mathf.Pow(terrainScript.xMax, 2) +
                                Mathf.Pow(pos.z, 2) / Mathf.Pow(terrainScript.zMax, 2)) * 10;
        if (distanceToEdge >= 0.9f)
        {
            pos *= (0.9f / distanceToEdge);
        }

        foreach (AudioSource orb in AudioPeerRoot.S.audioPeers)
        {
            if (Vector3.Distance(orb.transform.position, pos) < minDistance)
            {
                RandomizePosition();
                return;
            }
        }

        transform.position = pos;
    }

    void Start()
    {
        if(_interactable) RandomizePosition();

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
        
        //Create Light Ring
        lightRing = spawner.GetComponent<MagicSpawner>().ringPrefab;
        GameObject ring = Instantiate(lightRing, AudioPeerRoot.S.transform);
        Vector3 pos = transform.position;
        pos.y = terrainScript.GetTerrainHeight(pos.x, pos.z) + 1;
        ring.transform.position = pos;
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pond" && _interactable)
        {
            audioTrack.transform.SetParent(target);
            audioTrack.spatialBlend = 0;
            SkyFractal.S.ChangeOutline();
            ColorController.S.ChangePattern();

            print(OrbScript.orbsFound);
            target = WaterScript.orbPositions[OrbScript.orbsFound];
            OrbScript.orbsFound++;

            if (!Global.spleeterMode)
            {
                StartCoroutine(LowerVolume());
            }
            
        }
        else if (other.tag == "Player" && !found)
        {
            if (!following) target = other.transform;
            found = true;
            WaterScript.ring.SetActive(true);
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
        GameHUD.S.FlashColor("Green");
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
        while (audioTrack.volume > .25f)
        {
            audioTrack.volume -= Time.deltaTime;
            yield return null;
        }
        if(orbsFound == 4)
        {
            mixer.SetFloat("songVol", 1.1f);
        }
    }

    IEnumerator RaiseVolume(float dB)
    {
        float targetVolume;
        mixer.GetFloat("songVol", out targetVolume);
        float currentVolume = targetVolume;
        targetVolume += dB;

        while (currentVolume < targetVolume)
        {
            mixer.SetFloat("songVol", currentVolume);
            mixer.GetFloat("songVol", out currentVolume);
            currentVolume += Time.deltaTime;
            yield return null;
        }
        mixer.SetFloat("songVol", targetVolume);
    }
}
