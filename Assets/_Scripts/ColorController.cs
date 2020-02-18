using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    public static ColorController S;
    
    public ColorPalette[] colorPalettes;

    private int _paletteIndex = 0;
    private int _colorBase = 0;
    private int _colorIndex = 0;

    private GameObject _activeHead;
    private GameObject _terrain;

    public void SetActiveHead(GameObject activeHead)
    {
        _activeHead = activeHead;
        _terrain = _activeHead.transform.Find("Terrain").gameObject;
        StartCoroutine("AutoChangeColors");
    }

    public void ChangeColors()
    {
        _terrain.GetComponent<TerrainScript>().paintColor =
            colorPalettes[_paletteIndex].terrain[_colorBase + _colorIndex];
        _colorIndex = (_colorIndex + 1) % 2;
        StartCoroutine("AutoChangeColors");
    }

    private void Start()
    {
        S = this;
    }

    private IEnumerator AutoChangeColors()
    {
        yield return new WaitForSeconds(1);
        ChangeColors();
    }
}
