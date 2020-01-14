using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public int xMax = 10;
    public int zMax = 10;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = new Mesh();

        InitializeTerrain();
        UpdateTerrainMesh();
        
        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
    }

    private void InitializeTerrain()
    {
        _vertices = new Vector3[(xMax + 1) * (zMax + 1)];

        for (int i = 0, z = 0; z <= zMax; z++)
        {
            for (int x = 0; x <= xMax; x++, i++)
            {
                _vertices[i] = new Vector3(x, 0, z);
            }
        }

        _triangles = new int[xMax * zMax * 6];

        for (int v = 0, t = 0, z = 0; z < zMax; z++, v++)
        {
            for (int x = 0; x < xMax; x++, v++, t+= 6)
            {
                _triangles[t + 0] = v + 0;
                _triangles[t + 1] = v + xMax + 1;
                _triangles[t + 2] = v + 1;
                _triangles[t + 3] = v + 1;
                _triangles[t + 4] = v + xMax + 1;
                _triangles[t + 5] = v + xMax + 2;
            }
        }
    }

    private void UpdateTerrainMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }
}
