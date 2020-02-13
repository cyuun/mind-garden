using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyFractal : MonoBehaviour
{
    public static SkyFractal S;

    public List<GameObject> outlineList;
    public GameObject currentOutline;
    int outline_N = -1;
    public int outlineIndex { get { return outline_N; } set { outline_N = value; } }

    void Start()
    {
        if (!S) S = this;
        if (currentOutline) currentOutline.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeOutline()
    {
        if(currentOutline) currentOutline.SetActive(false);
        if (outline_N >= outlineList.Count) outline_N = outlineList.Count;
        currentOutline = outlineList[outline_N];
        currentOutline.SetActive(true);
        outline_N++;
    }

    public void ChangeOutline(int o)
    {
        if (currentOutline) currentOutline.SetActive(false);
        if (o >= outlineList.Count) o = outlineList.Count-1;
        else if (o < 0) o = 0;
        currentOutline = outlineList[o];
        currentOutline.SetActive(true);
        outline_N = o;
    }
}
