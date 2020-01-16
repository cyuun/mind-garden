using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainScript : MonoBehaviour
{
    //lattice variables
    public float xMax = 10;          //the max x size of the canvas for the outline to be "drawn" upon
    public float zMax = 10;          // "   "  z   "   "  "    "     "   "     "     "  "    "     "
    public float resolution = 1;     //increase this number to increase the number of lattice points per meter
    public float xScale = 1;         //larger = x axis is longer
    public float zScale = 1;         //  "    " z   "   "   "
    public bool liveEditing = false; //this can potentially be dangerous for performance, definitely not intended for use in running final game

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
        List<Vector3> tempVertices = new List<Vector3>();             //list to temporarily store vertices in because we don't know actual total count
        List<int> tempTriangles = new List<int>();                    // "    "     "         "   triangles...

        float xStep = (1 / resolution) * xScale;                      //the horizontal distance from one grid point to another within the same row
        float zStep = (Mathf.Sqrt(3) / (2 * resolution)) * zScale; // "  vertical      "       "   "    "    "    "   "       "     "    "  column

        int v = 0, vPrevious = 0, c = 0;                              //v = number of vertices in the current row being filled
        for (float z = -zMax/2; z <= zMax/2; z += zStep)              //vPrevious = num  "      "  "  previous...
        {                                                             //c = number of rows that have been filled yet
            if (v != 0)
            {
                vPrevious = v;
                v = 0;
                c++;
            }

            for (float x = -xMax/2; x <= xMax/2; x += xStep)
            {
                if (x < -xMax/2 + xStep/4 && c % 2 == 1)              //if we are at the beginning of our potential to create
                {                                                     //an odd indexed row (not necessarily the beginning of the row itself)
                    x -= xStep / 2;                                   //then shift the row back by a half step
                }
                
                tempVertices.Add(new Vector3(x, 0, z));     //add new vertex
                v++;                                                  //and increase this row's vertex counter

                if (vPrevious != 0)
                {
                    int vTotal = tempVertices.Count - 1;                 //the index of the last vertex added
                    if (v > 1 || x > tempVertices[vTotal - vPrevious].x) //if more than one vertex has been added this row
                    {                                                    //or if the first vertex of the previous row is to the left of this one
                        if (x < tempVertices[vTotal - vPrevious + 1].x)  //then if the previous row contains a vertex to the right of this
                        {                                                //then add the "first" triangle of the vertex
                            tempTriangles.Add(vTotal - 0);         //listed clockwise by index of vertex in vertices array
                            tempTriangles.Add(vTotal - vPrevious + 1);
                            tempTriangles.Add(vTotal - vPrevious);
                        }
                        if (v > 1)                                       //if more than one vertex has been added this row
                        {                                                //then add the "second" triangle of the vertex
                            tempTriangles.Add(vTotal - 0);
                            tempTriangles.Add(vTotal - vPrevious);
                            tempTriangles.Add(vTotal - 1);
                        }
                    }
                    else                                                 //if no vertices have been placed this row and the first vertex of the previous row is to the right
                    {                                                    //then no triangles are created for this vertex right now
                        vPrevious++;                                     //and adjust the vPrevious counter so it's wrong
                    }                                                    //but so that all successive triangles this row are created properly
                }
            }
        }

        _vertices = tempVertices.ToArray();                           //convert the temporary vertex list to a proper array
        _triangles = tempTriangles.ToArray();                         //   "     "      "     triangle "  "  "   "      "
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
