using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public float y_offset;
    public AudioPeer _audioPeer;

    private void Awake()
    {
        //Assign audio peer
        if(_audioPeer == null)
        {
            AudioSource[] audioSources = SpleeterProcess.S.orbs;
            _audioPeer = audioSources[Random.Range(0, audioSources.Length)].GetComponent<AudioPeer>(); ;
        }



    }
}
