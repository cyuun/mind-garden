using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [Range(0, 1)] public float grassPatchSize = 0.5f;
    public float grassNoiseScale = 10;
    public TerrainScript terrainScript;

    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    
    private System.Random _rng;
    private Vector2 _offsets;

    public void GenerateGrass()
    {
        _rng = new System.Random(terrainScript.seed);
        _offsets = new Vector2(_rng.Next(Mathf.CeilToInt(terrainScript.xMax) / 2, 10000), _rng.Next(Mathf.CeilToInt(terrainScript.zMax) / 2, 10000));
        
        List<Vector3> tempVertices = new List<Vector3>();
        List<int> tempTriangles = new List<int>();

        Vector3[] terrainVertices = terrainScript.GetVertices();

        for (int i = 0; i < terrainVertices.Length; i++)
        {
            float xPerlin = terrainVertices[i].x / grassNoiseScale + _offsets.x;
            float zPerlin = terrainVertices[i].z / grassNoiseScale + _offsets.y;

            if (grassPatchSize < Mathf.PerlinNoise(xPerlin, zPerlin))
            {
                tempVertices.Add(new Vector3(terrainVertices[i].x, terrainScript.GetTerrainHeight(terrainVertices[i].x, terrainVertices[i].z), terrainVertices[i].z));
                
                // add triangles
            }
        }
    }

    private void Start()
    {
        _mesh = new Mesh();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
}
