using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainScript : MonoBehaviour
{
    public int xMax = 10;
    public int zMax = 10;
    public int radius = 20;

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
        int radius_squared = radius * radius;
        
        _vertices = new Vector3[GetInscribedPoints()];
        _triangles = new int[GetInscribedSquares() * 6];

        for (int i = 0, v = 0, t = 0, z = -radius/2; z < radius/2; z++)
        {
            for (int x = -radius/2; x < radius/2; x++)
            {
                if (x * x + z * z <= radius_squared)
                {
                    _vertices[i] = new Vector3(x, 0, z);
                    i++;
                    if (x * x + z * z != radius_squared || (x < 0 && z < 0))
                    {
                        //TODO: fill out triangles array
                    }
                }
            }
        }

        

        for (int v = 0, t = 0, z = -radius/2; z < radius/2; z++, v++)
        {
            for (int x = -radius / 2; x < radius / 2; x++, v++, t += 6)
            {
                if (x * x + z * z < radius_squared)
                {
                    //TODO: remove this once the above for loop is complete
                    _triangles[t + 0] = v + 0;
                    _triangles[t + 1] = v + xMax + 1;
                    _triangles[t + 2] = v + 1;
                    _triangles[t + 3] = v + 1;
                    _triangles[t + 4] = v + xMax + 1;
                    _triangles[t + 5] = v + xMax + 2;
                }
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

    private int GetInscribedPoints(int r)
    {
        int N_r = 0
        for (int n = 0; n <= r * r; n++)
        {
            N_r += SumOfTwoSquares(n);
        }

        return N_r;
    }

    private int SumOfTwoSquares(int n)
    {
        int d1 = 0;
        int d3 = 0;
        for (int i = 1; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0)
            {
                if (i % 4 == 1)
                {
                    d1++;
                }
                else if (i % 4 == 3)
                {
                    d3++;
                }
                
                if (n / i != i)
                {
                    if (n / i % 4 == 1)
                    {
                        d1++;
                    }
                    else if (n / i % 4 == 3)
                    {
                        d3++;
                    }
                }
            }
        }
        
        return 4 * (d1 - d3);
    }
    
    private int GetInscribedSquares(int r)
    {
        int n = 0;
        for (int i = 1; i <= r; i++)
        {
            n += (int)Mathf.Floor(Mathf.Sqrt(r * r - i * i));
        }

        return n;
    }
}
