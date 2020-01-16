using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    //lattice variables
    public int xMax = 10;
    public int zMax = 10;
    public float resolution = 1;
    public int xScale = 1;
    public int zScale = 1;
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

    private void InitializeTerrain()
    {
        List<Vector3> tempVertices = new List<Vector3>();
        List<int> tempTriangles = new List<int>();

        float xStep = (1 / resolution) * xScale;
        float zStep = (Mathf.Sqrt(3) / (2 * resolution)) * zScale;

        int v = 0, vPrevious = 0;
        for (float z = 0; z <= zMax; z += zStep)
        {
            //int zz = z * z;
            
            if (v != 0)
            {
                vPrevious = v;
                v = 0;
            }
            
            for (float x = 0; x <= xMax; x += xStep)
            {
                //int xx = x * x;
                
                tempVertices.Add(new Vector3(x, 0, z));
                v++;

                if (vPrevious != 0)
                {
                    if (v > 1)
                    {
                        int vTotal = tempVertices.Count - 1;
                        tempTriangles.Add(vTotal - 0);
                        tempTriangles.Add(vTotal - vPrevious - 1);
                        tempTriangles.Add(vTotal - 1);
                        tempTriangles.Add(vTotal - 0);
                        tempTriangles.Add(vTotal - vPrevious);
                        tempTriangles.Add(vTotal - vPrevious - 1);
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
