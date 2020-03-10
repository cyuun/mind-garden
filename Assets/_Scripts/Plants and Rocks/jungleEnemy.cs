using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jungleEnemy : MonoBehaviour
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
        musicTemp = AudioAnalyzer.BPM;
        if (musicTemp != musicMultiplier && secondAnim)
        {
            anim.speed = musicTemp / 60;
            //musicTemp=musicMultiplier;
        }

        
    }
    IEnumerator firstAnim()
    {
        yield return new WaitForSeconds(1f);
        secondAnim = true;
        anim = GetComponent<Animator>();
        musicTemp = musicMultiplier;
        anim.speed = musicMultiplier;
    }
}
