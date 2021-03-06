﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplitudeRotate : MonoBehaviour
{
    public AudioPeer _audioPeer;
    public float _rotationSpeed;
    public int _direction;
    public 

    void Start()
    {
        if (_audioPeer == null)
        {
            AudioSource[] sources = AudioPeerRoot.S.audioPeers;
            _audioPeer = sources[Random.Range(0, sources.Length)].GetComponent<AudioPeer>();
        }

        while (_direction == 0)
        {
            _direction = Random.Range(-1, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up, (_audioPeer._amplitudeBuffer * Time.deltaTime * _rotationSpeed * _direction));
    }
}
