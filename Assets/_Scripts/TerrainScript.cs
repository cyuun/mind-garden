using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    //lattice variables
    public float xMax = 10;          //the max x size of the canvas for the outline to be "drawn" upon
    public float zMax = 10;          // "   "  z   "   "  "    "     "   "     "     "  "    "     "
    public float resolution = 1;     //increase this number to increase the number of lattice points per meter
    public float xScale = 1;         //larger = x axis is longer
    public float zScale = 1;         //  "    " z   "   "   "
    public bool liveEditing = false; //this can potentially be dangerous for performance, definitely not intended for use in running final game

    //mesh bounds variables for a circle
    public int radius = 10; //must be less than or equal to the min of xMax and zMax

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

    private void Update()
    {
        if (liveEditing)
        {
            InitializeTerrain();
            UpdateTerrainMesh();
        }
    }

    private void InitializeTerrain()
    {
        List<Vector3> tempVertices = new List<Vector3>();
        List<int> tempTriangles = new List<int>();

        float xStep = (1 / resolution) * xScale;
        float zStep = (Mathf.Sqrt(3) / (2 * resolution)) * zScale;

        int v = 0, vPrevious = 0, c = 0;
        for (float z = -zMax/2; z <= zMax/2; z += zStep)
        {
            //int zz = z * z;
            
            if (v != 0)
            {
                vPrevious = v;
                v = 0;
                c++;
            }

            for (float x = -xMax/2; x <= xMax/2; x += xStep)
            {
                //int xx = x * x;
                
                if (x < -xMax/2 + xStep/4)
                {
                    if (c % 2 == 1)
                    {
                        x -= xStep / 2;
                    }
                }
                
                tempVertices.Add(new Vector3(x, 0, z));
                v++;

                if (vPrevious != 0)
                {
                    int vTotal = tempVertices.Count - 1;
                    if (v > 1 || x > tempVertices[vTotal - vPrevious].x)
                    {
                        if (x < tempVertices[vTotal - vPrevious + 1].x)
                        {
                            tempTriangles.Add(vTotal - 0);
                            tempTriangles.Add(vTotal - vPrevious + 1);
                            tempTriangles.Add(vTotal - vPrevious);
                        }
                        if (v > 1)
                        {
                            tempTriangles.Add(vTotal - 0);
                            tempTriangles.Add(vTotal - vPrevious);
                            tempTriangles.Add(vTotal - 1);
                        }
                    }
                    else
                    {
                        vPrevious++;
                    }
                }
            }
        }

        _vertices = tempVertices.ToArray();
        _triangles = tempTriangles.ToArray();
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
