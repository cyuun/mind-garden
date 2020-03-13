using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public AudioPeer _audioPeer;
    public TreeSpawner spawner;
    public float y_offset;

    protected bool _spawnComplete = false;
    public bool spawnComplete { get { return _spawnComplete; } set { _spawnComplete = value; } }
    
    public TerrainScript terrainScript;

    private void Awake()
    {
        //Assign audio peer
        if(_audioPeer == null)
        {
            AudioSource[] audioSources = AudioPeerRoot.S.audioPeers;
            //_audioPeer = audioSources[Random.Range(0, audioSources.Length)].GetComponent<AudioPeer>(); ;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        switch (tag)
        {
            case "Rocks":
                Vector3 offsetFromOrb = Vector3.zero;
                Transform rock = other.transform.parent;
                offsetFromOrb = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * rock.GetComponent<Rock>().radius * rock.localScale.x;
                Vector3 pos = other.transform.position + offsetFromOrb;
                if (terrainScript == null)
                    terrainScript = AudioPeerRoot.S.terrainScript;
                pos.y = terrainScript.GetTerrainHeight(pos.x, pos.z);
                transform.position = pos;
                break;
            case "Pond":
                Vector3 newPos = transform.position;
                newPos = spawner.GetTreePos(newPos);
                transform.position = newPos;
                Debug.Log("dodge pond");
                break;
        }
    }
}
