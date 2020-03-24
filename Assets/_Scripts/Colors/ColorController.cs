using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    public static ColorController S;
    
    public ColorPalette[] colorPalettes;
    public Global.BiomeType biomeType;

    public int pattern;

    private int _paletteIndex;
    private int _colorBase2;
    private int _colorBase3;
    private int[] _colorIndices2;
    private int[] _colorIndices3;
    private int[] _colorIndices3Tracker;

    private GameObject _activeHead;
    private GameObject _terrain;
    private TerrainScript _terrainScript;
    private Material _headMaterial;
    private Material _rock1Material;
    private Material _rock2Material;
    private Material _rock3Material;
    private Material _rock4Material;
    private Material _rock5Material;
    private Material _plantBaseMaterial;
    private Material _plant1Material;
    private Material _plant2Material;
    private Material _plant3Material;
    private Material _wings1Material;
    private Material _wings2Material;
    private Material _bugMaterial;
    private Material _fish1Material;
    private Material _fish2Material;

    public void ChangeBase()
    {
        _colorBase3 = (_colorBase3 + 3) % 9;
        _colorBase2 = (_colorBase2 + 2) % 6;
    }

    public void ChangePattern()
    {
        pattern++; 
        
        _colorIndices3[0] = 0; 
        _colorIndices3[1] = 1; 
        _colorIndices3[2] = 2;
        
        _colorIndices3Tracker[0] = 0; 
        _colorIndices3Tracker[1] = 1; 
        _colorIndices3Tracker[2] = 2;
    }
    
    public void SetActiveHead(GameObject activeHead)
    {
        _paletteIndex = Global.colorPalette;
        _colorBase2 = 0;
        _colorBase3 = 0;
        _colorIndices2[0] = 0;
        _colorIndices2[1] = 1;
        _colorIndices3[0] = 0;
        _colorIndices3[1] = 1;
        _colorIndices3[2] = 2;

        _activeHead = activeHead;
        _terrain = _activeHead.transform.Find("Terrain").gameObject;
        _terrainScript = _terrain.GetComponent<TerrainScript>();
        _terrainScript.SetBasePaint(colorPalettes[_paletteIndex].terrain[6]);

        _headMaterial = _activeHead.GetComponent<MeshRenderer>().material;
        
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
            
            case Global.BiomeType.underwater:
                foreach (Transform plant in _terrain.transform.Find("_TreeParent"))
                {
                    Transform animLayer = plant.Find("Anim");
                    
                    if (!(plant1Attached && plantBaseAttached) && plant.name.Contains("Plant1"))
                    {
                        foreach (Transform plantPart in animLayer)
                        {
                            if (!plantBaseAttached && plantPart.name.Contains("Sphere"))
                            {
                                _plantBaseMaterial = plantPart.GetComponent<MeshRenderer>().sharedMaterial;
                                plantBaseAttached = true;
                            }
                            else if (!plant1Attached && plantPart.name.Contains("Cylinder"))
                            {
                                _plant1Material = plantPart.GetComponent<MeshRenderer>().sharedMaterial;
                                plant1Attached = true;
                            }
                        }
                    }
                    else if (!plant2Attached && plant.name.Contains("Plant2"))
                    {
                        foreach (Transform plantPart in plant)
                        {
                            if (!plant2Attached && plantPart.name.Contains("Box"))
                            {
                                _plant2Material = plantPart.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                                plant2Attached = true;
                            }
                        }
                    }
                    else if (!plant3Attached && plant.name.Contains("Plant3"))
                    {
                        _plant3Material = animLayer.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        plant3Attached = true;
                    }
                }
                break;
        }

        bool wings1Attached = false;
        bool wings2Attached = false;
        bool bugBodyAttached = false;
        bool fish1Attached = false;
        bool fish2Attached = false;

        foreach (Transform creature in _terrain.transform.Find("_CreatureParent"))
        {
            if (!(wings1Attached && bugBodyAttached) && creature.name.Contains("BugFlock1"))
            {
                foreach (Transform child in creature)
                {
                    if (!(wings1Attached && bugBodyAttached) && child.name.Contains("Bug"))
                    {
                        _wings1Material = child.GetChild(0).GetComponent<MeshRenderer>()
                            .sharedMaterial;
                        wings1Attached = true;
                        _bugMaterial = child.GetComponent<MeshRenderer>()
                            .sharedMaterial;
                        bugBodyAttached = true; 
                    }
                }
            }
            else if (!wings2Attached && creature.name.Contains("BugFlock2"))
            {
                foreach (Transform child in creature)
                {
                    if (!wings2Attached && child.name.Contains("Bug"))
                    {
                        foreach (Transform bodyPart in child)
                        {
                            if (!wings2Attached && bodyPart.name.Contains("Back"))
                            {
                                _wings2Material = bodyPart.GetComponent<MeshRenderer>().sharedMaterial;
                                wings2Attached = true;
                            }
                        }
                    }
                }
            }
            else if (!fish1Attached && creature.name.Contains("Fish1"))
            {
                _fish1Material = creature.Find("Fish").GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                fish1Attached = true;
            }
            else if (!fish2Attached && creature.name.Contains("Fish2"))
            {
                _fish2Material = creature.Find("Fish").GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                fish2Attached = true;
            }
        }

        if (!wings1Attached)
        {
            _wings1Material = null;
        }
        if (!wings2Attached)
        {
            _wings2Material = null;
        }
        if (!bugBodyAttached)
        {
            _bugMaterial = null;
        }
        if (!fish1Attached)
        {
            _fish1Material = null;
        }
        if (!fish2Attached)
        {
            _fish2Material = null;
        }
    }

    public void ChangeColors()
    {
        if (_activeHead == null)
        {
            return;
        }
        
        _headMaterial.SetColor("_Color", colorPalettes[_paletteIndex].head[_colorIndices3[0]]);
        _headMaterial.SetColor("_ColorDim", colorPalettes[_paletteIndex].head[_colorIndices3[1]]);
        _headMaterial.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].head[_colorIndices3[2]]);
        
        _terrainScript.paintColor =
            colorPalettes[_paletteIndex].terrain[_colorBase2 + _colorIndices2[0]];
        
        _rock1Material.SetColor("_Color", colorPalettes[_paletteIndex].rock1[_colorBase3 + _colorIndices3[0]]);
        _rock1Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock1[_colorBase3 + _colorIndices3[1]]);
        _rock1Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].rock1[_colorBase3 + _colorIndices3[2]]);
        
        _rock2Material.SetColor("_Color", colorPalettes[_paletteIndex].rock2[_colorBase3 + _colorIndices3[0]]);
        _rock2Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock2[_colorBase3 + _colorIndices3[1]]);
        _rock2Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].rock2[_colorBase3 + _colorIndices3[2]]);
        
        _rock3Material.SetColor("_Color", colorPalettes[_paletteIndex].rock3[_colorBase3 + _colorIndices3[0]]);
        _rock3Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock3[_colorBase3 + _colorIndices3[1]]);
        _rock3Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].rock3[_colorBase3 + _colorIndices3[2]]);
        
        _rock4Material.SetColor("_Color", colorPalettes[_paletteIndex].rock4[_colorBase3 + _colorIndices3[0]]);
        _rock4Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock4[_colorBase3 + _colorIndices3[1]]);
        _rock4Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].rock4[_colorBase3 + _colorIndices3[2]]);
        
        _rock5Material.SetColor("_Color", colorPalettes[_paletteIndex].rock5[_colorBase3 + _colorIndices3[0]]);
        _rock5Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].rock5[_colorBase3 + _colorIndices3[1]]);
        _rock5Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].rock5[_colorBase3 + _colorIndices3[2]]);
        
        _plantBaseMaterial.SetColor("_Color", colorPalettes[_paletteIndex].plantBase[_colorIndices3[0]]);
        _plantBaseMaterial.SetColor("_ColorDim", colorPalettes[_paletteIndex].plantBase[_colorIndices3[1]]);
        _plantBaseMaterial.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].plantBase[_colorIndices3[2]]);

        _plant1Material.SetColor("_Color", colorPalettes[_paletteIndex].plant1[_colorBase3 + _colorIndices3[0]]);
        _plant1Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant1[_colorBase3 + _colorIndices3[1]]);
        _plant1Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].plant1[_colorBase3 + _colorIndices3[2]]);
        
        _plant2Material.SetColor("_Color", colorPalettes[_paletteIndex].plant2[_colorBase3 + _colorIndices3[0]]);
        _plant2Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant2[_colorBase3 + _colorIndices3[1]]);
        _plant2Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].plant2[_colorBase3 + _colorIndices3[2]]);
        
        _plant3Material.SetColor("_Color", colorPalettes[_paletteIndex].plant3[_colorBase3 + _colorIndices3[0]]);
        _plant3Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].plant3[_colorBase3 + _colorIndices3[1]]);
        _plant3Material.SetColor("_ColorDimExtra", colorPalettes[_paletteIndex].plant3[_colorBase3 + _colorIndices3[2]]);

        if (_wings1Material != null)
        {
            _wings1Material.SetColor("_Color", colorPalettes[_paletteIndex].wings1[_colorBase3 + _colorIndices3[0]]);
            _wings1Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].wings1[_colorBase3 + _colorIndices3[1]]);
            _wings1Material.SetColor("_ColorDimExtra",
                colorPalettes[_paletteIndex].wings1[_colorBase3 + _colorIndices3[2]]);
        }

        if (_wings2Material != null)
        {
            _wings2Material.SetColor("_Color", colorPalettes[_paletteIndex].wings2[_colorBase3 + _colorIndices3[0]]);
            _wings2Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].wings2[_colorBase3 + _colorIndices3[1]]);
            _wings2Material.SetColor("_ColorDimExtra",
                colorPalettes[_paletteIndex].wings2[_colorBase3 + _colorIndices3[2]]);
        }

        if (_bugMaterial != null)
        {
            _bugMaterial.SetColor("_Color", colorPalettes[_paletteIndex].bugBodies[_colorBase3 + _colorIndices3[0]]);
            _bugMaterial.SetColor("_ColorDim", colorPalettes[_paletteIndex].bugBodies[_colorBase3 + _colorIndices3[1]]);
            _bugMaterial.SetColor("_ColorDimExtra",
                colorPalettes[_paletteIndex].bugBodies[_colorBase3 + _colorIndices3[2]]);
        }
        
        if (_fish1Material != null)
        {
            _fish1Material.SetColor("_Color", colorPalettes[_paletteIndex].fish1[_colorBase3 + _colorIndices3[0]]);
            _fish1Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].fish1[_colorBase3 + _colorIndices3[1]]);
            _fish1Material.SetColor("_ColorDimExtra",
                colorPalettes[_paletteIndex].fish1[_colorBase3 + _colorIndices3[2]]);
        }
        
        if (_fish2Material != null)
        {
            _fish2Material.SetColor("_Color", colorPalettes[_paletteIndex].fish2[_colorBase3 + _colorIndices3[0]]);
            _fish2Material.SetColor("_ColorDim", colorPalettes[_paletteIndex].fish2[_colorBase3 + _colorIndices3[1]]);
            _fish2Material.SetColor("_ColorDimExtra",
                colorPalettes[_paletteIndex].fish2[_colorBase3 + _colorIndices3[2]]);
        }

        ChangeIndices();
    }

    private void ChangeIndices()
    {
        _colorIndices2[0] = (_colorIndices2[0] + 1) % 2;
        _colorIndices2[1] = (_colorIndices2[1] + 1) % 2;
        switch (pattern)
        {
            default:
                _colorIndices3[0] = (_colorIndices3[0] + 1) % 2;
                _colorIndices3[1] = (_colorIndices3[1] + 1) % 2;
                
                break;
            
            case 1:
                _colorIndices3[1] = _colorIndices3[1] % 2 + 1;
                _colorIndices3[2] = _colorIndices3[2] % 2 + 1;

                break;
            
            case 2:
                _colorIndices3[0] = (_colorIndices3[0] + 1) % 3;
                _colorIndices3[1] = (_colorIndices3[1] + 1) % 3;
                _colorIndices3[2] = (_colorIndices3[2] + 1) % 3;

                break;
            
            case 3:
                _colorIndices3Tracker[0] = (_colorIndices3Tracker[0] + 1) % 4;
                _colorIndices3Tracker[1] = (_colorIndices3Tracker[1] + 1) % 4;
                _colorIndices3Tracker[2] = (_colorIndices3Tracker[2] + 1) % 4;

                for (int i = 0; i < 3; i++)
                {
                    if (_colorIndices3Tracker[i] == 3)
                    {
                        _colorIndices3[i] = 1;
                    }
                    else
                    {
                        _colorIndices3[i] = _colorIndices3Tracker[i];
                    }
                }

                break;
            
            case 4:
                _colorIndices3Tracker[0] = (_colorIndices3Tracker[0] + 1) % 8;
                _colorIndices3Tracker[1] = (_colorIndices3Tracker[1] + 1) % 8;
                _colorIndices3Tracker[2] = (_colorIndices3Tracker[2] + 1) % 8;

                for (int i = 0; i < 3; i++)
                {
                    switch (_colorIndices3Tracker[i])
                    {
                        default:
                            _colorIndices3[i] = _colorIndices3Tracker[i];
                            
                            break;
                        
                        case 3:
                            _colorIndices3[i] = 1;
                            
                            break;
                        
                        case 4:
                            _colorIndices3[i] = 2;
                            
                            break;
                        
                        case 5:
                            _colorIndices3[i] = 1;
                            
                            break;
                        
                        case 6:
                            _colorIndices3[i] = 0;
                            
                            break;
                        
                        case 7:
                            _colorIndices3[i] = 1;
                            
                            break;
                    }
                }
                
                break;
        }
    }

    private void Start()
    {
        pattern = 0;
        
        _colorIndices2 = new int[2];
        _colorIndices3 = new int[3];
        _colorIndices3Tracker = new int[3];

        S = this;
    }
}
