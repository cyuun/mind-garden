using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainScript : MonoBehaviour
{
    //lattice variables
    [Header("Lattice")]
    public float xMax = 100;          //the max x size of the canvas for the outline to be "drawn" upon
    public float zMax = 100;          // "   "  z   "   "  "    "     "   "     "     "  "    "     "
    public float resolution = 1;     //increase this number to increase the number of lattice points per meter
    public float skew = 1;           //increase greater than 1 to skew towards x, less than 1 to elongate z

    //terrain generation variables
    [Header("Terrain Generation")]
    public float noiseScale = 1;
    public float noiseAmplitude = 1;
    public   int noiseOctaves = 1;
    [Range(0,1)]
    public float noisePersistance = 0.5f;
    public float noiseLacunarity = 1.5f;
    public int seed = 1;

    //pond variables
    [Header("Center Pond")]
    public float pondRadius = 10;
   
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;

    private System.Random _rng;
    private Vector2[] _offsets;

    private float _seaLevel;

    public void RefreshTerrain()
    {
        UpdateRandom();
        UpdateSeaLevel();
        
        InitializeTerrain();
        UpdateTerrainMesh();
    }

    public float GetTerrainHeight(float x, float z)
    {
        float height = GetPerlinHeight(x, z) - _seaLevel;

        if (x * x + z * z <= Mathf.Sqrt(1.5f) * pondRadius * pondRadius)
        {
            height = height * (Mathf.Sqrt(x * x + z * z) / (1.5f * pondRadius));
        }

        return height;
    }
    
    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = new Mesh();

        RefreshTerrain();
    }

    private void OnValidate()
    {
        if (xMax < 1)
        {
            xMax = 1;
        }
        if (zMax < 1)
        {
            zMax = 1;
        }
        if (resolution <= 0)
        {
            resolution = 0.00001f;
        }
        if (skew <= 0)
        {
            skew = 0.00001f;
        }
        if (noiseScale <= 0)
        {
            skew = 0.00001f;
        }
        if (noiseOctaves < 0)
        {
            noiseOctaves = 0;
        }
        if (noiseLacunarity < 1)
        {
            noiseLacunarity = 1;
        }

        if (_mesh != null)
        {
            RefreshTerrain();
        }
    }

    private void InitializeTerrain()
    {
        List<Vector3> tempVertices = new List<Vector3>();             //list to temporarily store vertices in because we don't know actual total count
        List<int> tempTriangles = new List<int>();                    // "    "     "         "   triangles...

        float xStep = skew / resolution;                              //the horizontal distance from one grid point to another within the same row
        float zStep = Mathf.Sqrt(3) / (2 * resolution * skew);     //vertical      "       "   "    "    "    "   "       "     "    "  column

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

                if ((x * x) / (xMax * xMax) + (z * z) / (zMax * zMax) <= 0.1) //checking if the point is within the ellipse defined by the x and z maxes
                {
                    tempVertices.Add(new Vector3(x, GetTerrainHeight(x, z), z));         //add new vertex
                    v++;                                                      //and increase this row's vertex counter

                    if (vPrevious > 0)
                    {
                        int vTotal = tempVertices.Count - 1;                  //the index of the last vertex added
                        int rowOffsetL = CheckVerticesL(vTotal, vPrevious, 2, xStep, tempVertices);
                        int rowOffsetR = CheckVerticesR(vTotal, vPrevious, 2, xStep, tempVertices);
                        if (rowOffsetL > 0 && rowOffsetR > 0)                 //if both vertices from the previous line are good to go
                        {                                                     //then add the "first" triangle
                            tempTriangles.Add(vTotal - 0);              //by listing vertices in clockwise order
                            tempTriangles.Add(vTotal - rowOffsetR + 1);
                            tempTriangles.Add(vTotal - rowOffsetR);
                            
                            if (v > 1)                                       //if more than one vertex has been added this row
                            {                                                //then add the "second" triangle of the vertex
                                tempTriangles.Add(vTotal - 0);
                                tempTriangles.Add(vTotal - rowOffsetL);
                                tempTriangles.Add(vTotal - 1);
                            }
                        }
                        else if (rowOffsetL > 0 && rowOffsetR == 0)          //if there's no vertex down one row and to the right
                        {                                                    //then just add the "second" triangle
                            tempTriangles.Add(vTotal - 0);
                            tempTriangles.Add(vTotal - rowOffsetL);
                            tempTriangles.Add(vTotal - 1);
                        }
                    }
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
        
        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
    }

    //CheckVerticesL returns 0 if there is no vertex down one row and to the left of the vertex at 'index'
    //otherwise it returns the proper number of indices back the aforementioned vertex is.
    private int CheckVerticesL(int index, int offset, int correctionSearchRadius, float xStep, List<Vector3> vertices)
    {
        float x = vertices[index].x;

        int returnOffset = 0;
        for (int tempOffset = offset - correctionSearchRadius; tempOffset <= offset + correctionSearchRadius; tempOffset++)
        {
            if (tempOffset > 0 && index - tempOffset >= 0)
            {
                float xCheck = vertices[index - tempOffset].x;

                if (CompareFloats(xCheck, x - xStep / 2, xStep / 4))
                {
                    returnOffset = tempOffset;
                }
            }
        }

        return returnOffset;
    }

    //CheckVerticesR returns 0 if there is no vertex down one row and to the right of the vertex at 'index'
    //otherwise it returns the proper number of indices back the aforementioned vertex is.
    private int CheckVerticesR(int index, int offset, int correctionSearchRadius, float xStep, List<Vector3> vertices)
    {
        float x = vertices[index].x;

        int returnOffset = 0;
        for (int tempOffset = offset - correctionSearchRadius; tempOffset <= offset + correctionSearchRadius; tempOffset++)
        {
            if (tempOffset > 0 && index - tempOffset + 1 >= 0)
            {
                float xCheck = vertices[index - tempOffset + 1].x;

                if (CompareFloats(xCheck, x + xStep / 2, xStep / 4))
                {
                    returnOffset = tempOffset;
                }
            }
        }

        return returnOffset;
    }

    private bool CompareFloats(float f1, float f2, float tolerance)
    {
        return (f1 <= f2 + tolerance && f1 >= f2 - tolerance);
    }

    private float GetPerlinHeight(float x, float z)
    {
        float amplitude = noiseAmplitude;
        float frequency = 1;
        float height = 0;
        
        for (int i = 0; i < noiseOctaves; i++)
        {
            float xPerlin = x / noiseScale * frequency + _offsets[i].x;
            float zPerlin = z / noiseScale * frequency + _offsets[i].y;

            height += Mathf.PerlinNoise(xPerlin, zPerlin) * amplitude;

            amplitude *= noisePersistance;
            frequency *= noiseLacunarity;
        }

        return height;
    }
    
    private void UpdateRandom()
    {
        _rng = new System.Random(seed);
        
        _offsets = new Vector2[noiseOctaves];
        for (int i = 0; i < noiseOctaves; i++)
        {
            float xOffset = _rng.Next(Mathf.CeilToInt(xMax) / 2, 10000);
            float zOffset = _rng.Next(Mathf.CeilToInt(zMax) / 2, 10000);
            _offsets[i] = new Vector2(xOffset, zOffset);
        }
    }

    private void UpdateSeaLevel()
    {
        int rSteps = 20;
        int thetaSteps = 360;
        float sum = 0;

        for (int rIndex = 0; rIndex < rSteps; rIndex++)
        {
            float r = rIndex * pondRadius / rSteps;
            for (int thetaIndex = 0; thetaIndex < thetaSteps; thetaIndex++)
            {
                float theta = thetaIndex * 360 / thetaSteps;
                float x = r * Mathf.Cos(theta);
                float z = r * Mathf.Sin(theta);

                sum += GetPerlinHeight(x, z);
            }
        }

        _seaLevel = sum / (rSteps * thetaSteps);
    }

    private float AdjustHeightForPond()
    {
        float avgHeight = 0;
        int numVertices = 0;
        for (int i = 0; i < _vertices.Length; i++)
        {
            float x = _vertices[i].x;
            float y = _vertices[i].y;
            float z = _vertices[i].z;
            if (x * x + z * z <= pondRadius * pondRadius)
            {
                numVertices++;
                avgHeight += y;
            }
        }

        if (numVertices > 0)
        {
            avgHeight /= numVertices;
        }

        return avgHeight;
    }
}
