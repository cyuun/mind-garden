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
    [SerializeField]
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
        
        
        bool plant1Attached = false;
        bool plant2Attached = false;
        bool plant3Attached = false;
        bool plantBaseAttached = false;
        
        switch (biomeType)
        {
            case Global.BiomeType.desert:
                foreach (Transform plant in _terrain.transform.Find("_TreeParent"))
                {
                    Transform animLayer = plant.Find("Anim");
                    
                    if (!(plant1Attached && plantBaseAttached) && plant.name.Contains("Plant1"))
                    {
                        foreach (Transform plantPart in animLayer)
                        {
                            if (!plantBaseAttached && plantPart.name.Contains("Cone"))
                            {
                                _plantBaseMaterial = plantPart.GetComponent<MeshRenderer>().sharedMaterial;
                                plantBaseAttached = true;
                            }
                            else if (!plant1Attached && plantPart.name.Contains("Capsule"))
                            {
                                _plant1Material = plantPart.GetComponent<MeshRenderer>().sharedMaterial;
                                plant1Attached = true;
                            }
                        }
                    }
                    else if (!plant2Attached && plant.name.Contains("Plant2"))
                    {
                        foreach (Transform plantPart in animLayer)
                        {
                            if (!plant2Attached && plantPart.name.Contains("Capsule"))
                            {
                                _plant2Material = plantPart.GetComponent<MeshRenderer>().sharedMaterial;
                                plant2Attached = true;
                            }
                        }
                    }
                    else if (!plant3Attached && plant.name.Contains("Plant3"))
                    {
                        _plant3Material = animLayer.Find("Gems").GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        plant3Attached = true;
                    }
                }
                break;
            
            case Global.BiomeType.forest:
                foreach (Transform plant in _terrain.transform.Find("_TreeParent"))
                {
                    Transform animLayer = plant.Find("Anim");

                    if (!(plant1Attached && plantBaseAttached) && plant.name.Contains("Plant1"))
                    {
                        _plantBaseMaterial = animLayer.Find("Branches").GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        _plant1Material = animLayer.Find("Leaves").GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        plant1Attached = true;
                        plantBaseAttached = true;
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
                }
                break;
            
            case Global.BiomeType.jungle:
                foreach (Transform plant in _terrain.transform.Find("_TreeParent"))
                {
                    Transform animLayer = plant.Find("Anim");
                    
                    if (!(plant1Attached && plantBaseAttached) && plant.name.Contains("Plant1"))
                    {
                        foreach (Transform plantPart in animLayer)
                        {
                            if (!plantBaseAttached && plantPart.name.Contains("Cone"))
                            {
                                _plantBaseMaterial = plantPart.GetComponent<MeshRenderer>().sharedMaterial;
                                plantBaseAttached = true;
                            }
                            else if (!plant1Attached && plantPart.name.Contains("Leaves"))
                            {
                                _plant1Material = plantPart.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                                plant1Attached = true;
                            }
                        }
                    }
                    else if (!plant2Attached && plant.name.Contains("Plant2"))
                    {
                        foreach (Transform plantPart in animLayer)
                        {
                            if (!plant2Attached && plantPart.name.Contains("Main"))
                            {
                                _plant2Material = plantPart.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                                plant2Attached = true;
                            }
                        }
                    }
                    else if (!plant3Attached && plant.name.Contains("Plant3"))
                    {
                        _plant3Material = animLayer.Find("Plant").GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                        plant3Attached = true;
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
        
        _plantBaseMaterial.SetColor("_Color", colorPalettes[_paletteIndex].plantBase[_colorIndex]);
        _plantBaseMaterial.SetColor("_ColorDim", colorPalettes[_paletteIndex].plantBase[(_colorIndex + 1) % 2]);

        _plant1Material.SetColor("_Color", colorPalettes[_paletteIndex].plant1[_colorBase + _colorIndex]);
        _plant1Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant1[_colorBase + (_colorIndex + 1) % 2]);
        
        _plant2Material.SetColor("_Color", colorPalettes[_paletteIndex].plant2[_colorBase + _colorIndex]);
        _plant2Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant2[_colorBase + (_colorIndex + 1) % 2]);
        
        _plant3Material.SetColor("_Color", colorPalettes[_paletteIndex].plant3[_colorBase + _colorIndex]);
        _plant3Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant3[_colorBase + (_colorIndex + 1) % 2]);

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
