using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class junglePlantBig: MonoBehaviour
{
    public GameObject piece1;
    public GameObject piece2;
    public GameObject piece3;
    public GameObject piece4;
    Animator anim;
    bool start = false;
    public bool isSpawningPrefab;

    //0 is unmoving, 40 is pretty fast/kinda seizure-y
    float mainMusicVariable = 10f;

    private void Start()
    {
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (!start && anim)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("deadAnim"))
            {
                anim.enabled = false;
                start = true;
            }
        }

        else if (start || !isSpawningPrefab)
        {
            Vector3 vec1 = new Vector3(0.00428168f, 0.00428168f, 0.0264475f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.01f)) * 0.0035f));
            Vector3 vec2 = new Vector3(0.00428168f, 0.00428168f, 0.01418361f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.08f)) * 0.008f));
            Vector3 vec3 = new Vector3(0.00428168f, 0.00428168f, 0.01715916f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.04f)) * 0.004f));
            Vector3 vec4 = new Vector3(0.00428168f, 0.00428168f, 0.02068897f + (Mathf.Sin(Time.time * (mainMusicVariable + 0.012f)) * 0.006f));

            piece1.transform.localScale = vec1;
            piece2.transform.localScale = vec2;
            piece3.transform.localScale = vec3;
            piece4.transform.localScale = vec4;
        }
    }

    // void Update()
    //{
    //piece1.transform.localScale+= new Vector3(0, 0, mainMusicVariable * scaleSpeed1 * Time.deltaTime);
    //piece2.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed2 * Time.deltaTime);
    //piece3.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed3 * Time.deltaTime);
    //piece4.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed4 * Time.deltaTime);
    //piece5.transform.localScale += new Vector3(0, 0, mainMusicVariable * scaleSpeed5 * Time.deltaTime);
    //}
}
