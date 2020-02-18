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
    private List<MeshRenderer> _rock1Renderers;
    private List<MeshRenderer> _rock2Renderers;
    private List<MeshRenderer> _rock3Renderers;
    private List<MeshRenderer> _rock4Renderers;
    private List<MeshRenderer> _rock5Renderers;

    private int _mainColorID;
    private int _celColorID;

    public void SetActiveHead(GameObject activeHead)
    {
        _activeHead = activeHead;
        _terrain = _activeHead.transform.Find("Terrain").gameObject;

        foreach (Transform rock in _terrain.transform.Find("_RockParent"))
        {
            MeshRenderer[] rockRenderers;
            switch (rock.name)
            {
                case "Rock1(Clone)":
                    rockRenderers = rock.GetComponentsInChildren<MeshRenderer>();
                    foreach (var rockRenderer in rockRenderers)
                    {
                        _rock1Renderers.Add(rockRenderer);
                    }
                    break;
                
                case "Rock2(Clone)":
                    rockRenderers = rock.GetComponentsInChildren<MeshRenderer>();
                    foreach (var rockRenderer in rockRenderers)
                    {
                        _rock2Renderers.Add(rockRenderer);
                    }
                    break;
                
                case "Rock3(Clone)":
                    rockRenderers = rock.GetComponentsInChildren<MeshRenderer>();
                    foreach (var rockRenderer in rockRenderers)
                    {
                        _rock3Renderers.Add(rockRenderer);
                    }
                    break;
                
                case "Rock4(Clone)":
                    rockRenderers = rock.GetComponentsInChildren<MeshRenderer>();
                    foreach (var rockRenderer in rockRenderers)
                    {
                        _rock4Renderers.Add(rockRenderer);
                    }
                    break;
                
                case "Rock5(Clone)":
                    rockRenderers = rock.GetComponentsInChildren<MeshRenderer>();
                    foreach (var rockRenderer in rockRenderers)
                    {
                        _rock5Renderers.Add(rockRenderer);
                    }
                    break;
            }
        }
    }

    public void ChangeColors()
    {
        if (_activeHead == null)
        {
            return;
        }
        
        _terrain.GetComponent<TerrainScript>().paintColor =
            colorPalettes[_paletteIndex].terrain[_colorBase + _colorIndex];
        foreach (var rockRenderer in _rock1Renderers)
        {
            rockRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].rock1[_colorBase + _colorIndex]);
            rockRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock1[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var rockRenderer in _rock2Renderers)
        {
            rockRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].rock2[_colorBase + _colorIndex]);
            rockRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock2[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var rockRenderer in _rock3Renderers)
        {
            rockRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].rock3[_colorBase + _colorIndex]);
            rockRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock3[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var rockRenderer in _rock4Renderers)
        {
            rockRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].rock4[_colorBase + _colorIndex]);
            rockRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock4[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var rockRenderer in _rock5Renderers)
        {
            rockRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].rock5[_colorBase + _colorIndex]);
            rockRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock5[_colorBase + (_colorIndex + 1) % 2]);
        }
        
        _colorIndex = (_colorIndex + 1) % 2;
    }

    private void Start()
    {
        _rock1Renderers = new List<MeshRenderer>();
        _rock2Renderers = new List<MeshRenderer>();
        _rock3Renderers = new List<MeshRenderer>();
        _rock4Renderers = new List<MeshRenderer>();
        _rock5Renderers = new List<MeshRenderer>();
        
        S = this;
    }
}
