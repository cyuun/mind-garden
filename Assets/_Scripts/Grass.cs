using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [Range(0, 1)] public float grassPatchSize;
    public TerrainScript terrainScript;

    private System.Random _rng;
    private Vector2 _offsets;

    public void GenerateGrass()
    {
        // get perlin
        // create mesh wherever perlin is less than grass patch size
    }

    private void Start()
    {
        
    }
}
