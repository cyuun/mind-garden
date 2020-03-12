using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    public static ColorController S;
    
    public ColorPalette[] colorPalettes;

    private int _paletteIndex;
    private int _colorBase;
    private int _colorIndex;

    private GameObject _activeHead;
    private GameObject _terrain;
    private TerrainScript _terrainScript;
    private List<MeshRenderer> _rock1Renderers;
    private List<MeshRenderer> _rock2Renderers;
    private List<MeshRenderer> _rock3Renderers;
    private List<MeshRenderer> _rock4Renderers;
    private List<MeshRenderer> _rock5Renderers;
    private List<MeshRenderer> _plantBaseRenderers;
    private List<MeshRenderer> _plant1Renderers;
    private List<MeshRenderer> _plant2Renderers;
    private List<MeshRenderer> _plant3Renderers;
    
    public enum Biome
    {
        desert,
        forest,
        jungle,
        underwater
    };

    private Biome _biome;

    public int biome
    {
        get { return (int)_biome; }
        set { _biome = (Biome)value; }
    }

    public void ChangeBase()
    {
        _colorBase = (_colorBase + 2) % 6;
    }
    
    public void SetActiveHead(GameObject activeHead)
    {
        _colorIndex = 0;
        _colorBase = 0;

        _rock1Renderers.Clear();
        _rock2Renderers.Clear();
        _rock3Renderers.Clear();
        _rock4Renderers.Clear();
        _rock5Renderers.Clear();
        _plantBaseRenderers.Clear();
        _plant1Renderers.Clear();
        _plant2Renderers.Clear();
        _plant3Renderers.Clear();

        _activeHead = activeHead;
        _terrain = _activeHead.transform.Find("Terrain").gameObject;
        _terrainScript = _terrain.GetComponent<TerrainScript>();
        _terrainScript.SetBasePaint(colorPalettes[_paletteIndex].terrain[6]);

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
        
        switch (_biome)
        {
            case Biome.desert:
                foreach (Transform plant in _terrain.transform.Find("_TreeParent"))
                {
                    if (plant.name.Contains("Plant1"))
                    {
                        Transform animLayer = plant.Find("Anim");
                        foreach (Transform capsule in animLayer)
                        {
                            _plant1Renderers.Add(capsule.GetComponent<MeshRenderer>());
                            foreach (Transform cone in capsule)
                            {
                                _plantBaseRenderers.Add(cone.GetComponent<MeshRenderer>()); //removing this might increase performance
                            }
                        }
                    }

                    if (plant.name.Contains("Plant2"))
                    {
                        Transform animLayer = plant.Find("Anim");
                        foreach (Transform child in animLayer)
                        {
                            if (child.name.Contains("Capsule"))
                            {
                                _plant2Renderers.Add(child.GetComponent<MeshRenderer>());
                            }
                            else if (child.name.Contains("Cone"))
                            {
                                _plantBaseRenderers.Add(child.GetComponent<MeshRenderer>()); //removing this might increase performance
                            }
                        }
                    }

                    if (plant.name.Contains("Plant3"))
                    {
                        Transform animLayer = plant.Find("Anim");
                        MeshRenderer[] mainRenderers = animLayer.Find("Gems").GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer renderer in mainRenderers)
                        {
                            _plant3Renderers.Add(renderer);
                        }
                        _plantBaseRenderers.Add(animLayer.Find("Branches").GetComponent<MeshRenderer>());
                    }
                }
                break;
            
            case Biome.forest:
                break;
        }
    }

    public void ChangeColors()
    {
        if (_activeHead == null)
        {
            return;
        }
        
        _terrainScript.paintColor =
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
        foreach (var plantBaseRenderer in _plantBaseRenderers)
        {
            plantBaseRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].plantBase[_colorIndex]);
            plantBaseRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plantBase[(_colorIndex + 1) % 2]);
        }
        foreach (var plantRenderer in _plant1Renderers)
        {
            plantRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].plant1[_colorBase + _colorIndex]);
            plantRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant1[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var plantRenderer in _plant2Renderers)
        {
            plantRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].plant2[_colorBase + _colorIndex]);
            plantRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant2[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var plantRenderer in _plant3Renderers)
        {
            plantRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].plant3[_colorBase + _colorIndex]);
            plantRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant3[_colorBase + (_colorIndex + 1) % 2]);
        }
        
        _colorIndex = (_colorIndex + 1) % 2;
    }

    private void Start()
    {
        _paletteIndex = 1;
        _colorBase = 0;
        _colorIndex = 0;
        
        _rock1Renderers = new List<MeshRenderer>();
        _rock2Renderers = new List<MeshRenderer>();
        _rock3Renderers = new List<MeshRenderer>();
        _rock4Renderers = new List<MeshRenderer>();
        _rock5Renderers = new List<MeshRenderer>();
        _plantBaseRenderers = new List<MeshRenderer>();
        _plant1Renderers = new List<MeshRenderer>();
        _plant2Renderers = new List<MeshRenderer>();
        _plant3Renderers = new List<MeshRenderer>();

        S = this;
    }
}
