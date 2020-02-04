using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public AudioPeer _audioPeer;
    public TreeSpawner spawner;
    public float y_offset;

    private void Awake()
    {
        //Assign audio peer
        if(_audioPeer == null)
        {
            AudioSource[] audioSources = SpleeterProcess.S.orbs;
            _audioPeer = audioSources[Random.Range(0, audioSources.Length)].GetComponent<AudioPeer>(); ;
        }



    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        print("Reset");

        switch (tag)
        {
            case "Rocks":
                Vector3 offsetFromOrb = Vector3.zero;
                while (offsetFromOrb.magnitude < spawner.spawnRadiusMin)
                {
                    offsetFromOrb = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * Random.Range(1, spawner.spawnRadiusMax);
                }
                Vector3 pos = transform.position + offsetFromOrb;
                pos.y = TerrainScript.S.GetTerrainHeight(pos.x, pos.z);
                transform.position = pos;
                break;
        }
    }
}
