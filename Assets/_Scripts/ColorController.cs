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
    private List<MeshRenderer> _trunkRenderers;
    private List<MeshRenderer> _tree1Renderers;
    private List<MeshRenderer> _tree2Renderers;
    private List<MeshRenderer> _tree3Renderers;

    private int _mainColorID;
    private int _celColorID;

    public void SetActiveHead(GameObject activeHead)
    {
        _colorIndex = 0;
        _colorBase = 0;

        _rock1Renderers.Clear();
        _rock2Renderers.Clear();
        _rock3Renderers.Clear();
        _rock4Renderers.Clear();
        _rock5Renderers.Clear();
        _trunkRenderers.Clear();
        _tree1Renderers.Clear();
        _tree2Renderers.Clear();
        _tree3Renderers.Clear();

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

        foreach (Transform tree in _terrain.transform.Find("_TreeParent"))
        {
            MeshRenderer[] leavesRenderers;
            switch (tree.name)
            {
                case "Tree1(Clone)":
                    Transform t = tree.GetChild(0);
                    leavesRenderers = t.Find("Capsules").GetComponentsInChildren<MeshRenderer>();
                    foreach (var leavesRenderer in leavesRenderers)
                    {
                        _tree1Renderers.Add(leavesRenderer);
                    }
                    _trunkRenderers.Add(t.Find("DeadTree21.fbx").GetComponentInChildren<MeshRenderer>());
                    break;
                
                case "Tree2(Clone)":
                    t = tree.GetChild(0);
                    leavesRenderers = t.Find("Pieces").GetComponentsInChildren<MeshRenderer>();
                    foreach (var leavesRenderer in leavesRenderers)
                    {
                        _tree2Renderers.Add(leavesRenderer);
                    }
                    //_trunkRenderers.Add((MeshRenderer)tree.Find("P3D_DeadTree002").GetComponent<MeshRenderer>());
                    break;
                
                case "Tree3(Clone)":
                    t = tree.GetChild(0);
                    leavesRenderers = t.Find("crstalbois").GetComponentsInChildren<MeshRenderer>();
                    foreach (var leavesRenderer in leavesRenderers)
                    {
                        _tree3Renderers.Add(leavesRenderer);
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
        foreach (var trunkRenderer in _trunkRenderers)
        {
            trunkRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].treeTrunks[_colorIndex]);
            trunkRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].treeTrunks[(_colorIndex + 1) % 2]);
        }
        foreach (var treeRenderer in _tree1Renderers)
        {
            treeRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].tree1[_colorBase + _colorIndex]);
            treeRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].tree1[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var treeRenderer in _tree2Renderers)
        {
            treeRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].tree2[_colorBase + _colorIndex]);
            treeRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].tree2[_colorBase + (_colorIndex + 1) % 2]);
        }
        foreach (var treeRenderer in _tree3Renderers)
        {
            treeRenderer.material.SetColor("_Color", colorPalettes[_paletteIndex].tree3[_colorBase + _colorIndex]);
            treeRenderer.material.SetColor("_ColorDim", colorPalettes[_paletteIndex].tree3[_colorBase + (_colorIndex + 1) % 2]);
        }
        
        _colorIndex = (_colorIndex + 1) % 2;
    }

    private void Start()
    {
        _mainColorID = Shader.GetGlobalInt("_Color");
        _celColorID = Shader.GetGlobalInt("_ColorDim");

        _rock1Renderers = new List<MeshRenderer>();
        _rock2Renderers = new List<MeshRenderer>();
        _rock3Renderers = new List<MeshRenderer>();
        _rock4Renderers = new List<MeshRenderer>();
        _rock5Renderers = new List<MeshRenderer>();
        _trunkRenderers = new List<MeshRenderer>();
        _tree1Renderers = new List<MeshRenderer>();
        _tree2Renderers = new List<MeshRenderer>();
        _tree3Renderers = new List<MeshRenderer>();

        S = this;
    }
}
