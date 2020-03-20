using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grass : MonoBehaviour
{
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    public void SetMesh(Vector3[] vertices, int[] triangles)
    {
        _mesh.Clear();
        
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();

        _meshFilter.mesh = _mesh;
    }

    public void Initialize()
    {
        _mesh = new Mesh();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
    }
}
