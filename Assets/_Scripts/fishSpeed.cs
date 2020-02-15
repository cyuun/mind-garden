using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishSpeed : MonoBehaviour
{
    public float musicMultiplier = 1.0f;
    float musicTemp;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        musicTemp = musicMultiplier;
        anim.speed = musicMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if (musicTemp != musicMultiplier)
        {
            anim.speed = musicTemp;
            musicTemp=musicMultiplier;
        }

        
    }
}
