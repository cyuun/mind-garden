using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    public static ColorController S;
    
    public ColorPalette[] colorPalettes;
    public Global.BiomeType biomeType;

    private int _paletteIndex;
    private int _colorBase;
    private int _colorIndex;

    private GameObject _activeHead;
    private GameObject _terrain;
    private TerrainScript _terrainScript;
    private Material _rock1Material;
    private Material _rock2Material;
    private Material _rock3Material;
    private Material _rock4Material;
    private Material _rock5Material;
    private Material _plantBaseMaterial;
    private Material _plant1Material;
    private Material _plant2Material;
    private Material _plant3Material;

    public void ChangeBase()
    {
        _colorBase = (_colorBase + 2) % 6;
    }
    
    public void SetActiveHead(GameObject activeHead)
    {
        _colorIndex = 0;
        _colorBase = 0;

        _activeHead = activeHead;
        _terrain = _activeHead.transform.Find("Terrain").gameObject;
        _terrainScript = _terrain.GetComponent<TerrainScript>();
        _terrainScript.SetBasePaint(colorPalettes[_paletteIndex].terrain[6]);

        
        bool rock1Attached = false;
        bool rock2Attached = false;
        bool rock3Attached = false;
        bool rock4Attached = false;
        bool rock5Attached = false;
        
        foreach (Transform rock in _terrain.transform.Find("_RockParent"))
        {
            if (!rock1Attached && rock.name.Contains("Rock1"))
            {
                _rock1Material = rock.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                rock1Attached = true;
            }
            else if (!rock2Attached && rock.name.Contains("Rock2"))
            {
                _rock2Material = rock.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                rock2Attached = true;
            }
            else if (!rock3Attached && rock.name.Contains("Rock3"))
            {
                _rock3Material = rock.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                rock3Attached = true;
            }
            else if (!rock4Attached && rock.name.Contains("Rock4"))
            {
                _rock4Material = rock.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                rock4Attached = true;
            }
            else if (!rock5Attached && rock.name.Contains("Rock5"))
            {
                _rock5Material = rock.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                rock5Attached = true;
            }
        }
        
        switch (biomeType)
        {
            /*case Global.BiomeType.desert:
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
                                //_plantBaseRenderers.Add(cone.GetComponent<MeshRenderer>()); //removing this might increase performance
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
                                //_plantBaseRenderers.Add(child.GetComponent<MeshRenderer>()); //removing this might increase performance
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
                        mainRenderers = animLayer.Find("MoreGems").GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer renderer in mainRenderers)
                        {
                            _plant3Renderers.Add(renderer);
                        }
                        _plantBaseRenderers.Add(animLayer.Find("Branches").GetComponent<MeshRenderer>());
                    }
                }
                break;
            */
            case Global.BiomeType.forest:
                bool plant1Attached = false;
                bool plant2Attached = false;
                bool plant3Attached = false;
                bool plantBaseAttached = false;
                
                foreach (Transform plant in _terrain.transform.Find("_TreeParent"))
                {
                    Transform animLayer = plant.Find("Anim");

                    if (!plant1Attached && plant.name.Contains("Plant1"))
                    {
                        _plant1Material = animLayer.Find("Leaves").GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        plant1Attached = true;
                    }
                    else if (!plant2Attached && plant.name.Contains("Plant2"))
                    {
                        _plant2Material = animLayer.Find("Leaves").GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        plant2Attached = true;
                    }
                    else if (!plant3Attached && plant.name.Contains("Plant3"))
                    {
                        _plant3Material = animLayer.Find("Gems").GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        plant3Attached = true;
                    }
                    if (!plantBaseAttached && !plant.name.Contains("Plant2"))
                    {
                        _plantBaseMaterial = animLayer.Find("Branches").GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        plantBaseAttached = true;
                    }
                }
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
        
        _rock1Material.SetColor("_Color", colorPalettes[_paletteIndex].rock1[_colorBase + _colorIndex]);
        _rock1Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock1[_colorBase + (_colorIndex + 1) % 2]);
        
        _rock2Material.SetColor("_Color", colorPalettes[_paletteIndex].rock2[_colorBase + _colorIndex]);
        _rock2Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock2[_colorBase + (_colorIndex + 1) % 2]);
        
        _rock3Material.SetColor("_Color", colorPalettes[_paletteIndex].rock3[_colorBase + _colorIndex]);
        _rock3Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock3[_colorBase + (_colorIndex + 1) % 2]);
        
        _rock4Material.SetColor("_Color", colorPalettes[_paletteIndex].rock4[_colorBase + _colorIndex]);
        _rock4Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock4[_colorBase + (_colorIndex + 1) % 2]);
        
        _rock5Material.SetColor("_Color", colorPalettes[_paletteIndex].rock5[_colorBase + _colorIndex]);
        _rock5Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock5[_colorBase + (_colorIndex + 1) % 2]);
        
        if(_plant1Material != null) _plant1Material.SetColor("_Color", colorPalettes[_paletteIndex].plant1[_colorBase + _colorIndex]);
        if(_plant1Material != null) _plant1Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant1[_colorBase + (_colorIndex + 1) % 2]);
        
        if(_plant2Material != null) _plant2Material.SetColor("_Color", colorPalettes[_paletteIndex].plant2[_colorBase + _colorIndex]);
        if(_plant2Material != null) _plant2Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant2[_colorBase + (_colorIndex + 1) % 2]);
        
        if(_plant3Material != null) _plant3Material.SetColor("_Color", colorPalettes[_paletteIndex].plant3[_colorBase + _colorIndex]);
        if(_plant3Material != null) _plant3Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant3[_colorBase + (_colorIndex + 1) % 2]);
        
        _colorIndex = (_colorIndex + 1) % 2;
    }

    private void Start()
    {
        _paletteIndex = 1;
        _colorBase = 0;
        _colorIndex = 0;

        S = this;
    }
}
