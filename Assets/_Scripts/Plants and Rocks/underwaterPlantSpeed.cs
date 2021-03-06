﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class underwaterPlantSpeed : Plant
{
    public float musicMultiplier = 1.0f;
    float musicTemp;
    Animator anim;
    bool secondAnim = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(firstAnim());
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (musicTemp != musicMultiplier && secondAnim)
        {
            anim.speed = musicTemp;
            musicTemp=musicMultiplier;
        }

        
    }
    IEnumerator firstAnim()
    {
        yield return new WaitForSeconds(2.1f);
        secondAnim = true;
        anim = GetComponent<Animator>();
        musicTemp = musicMultiplier;
        anim.speed = musicMultiplier;
    }
}
