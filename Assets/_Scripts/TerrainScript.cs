using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public int xMax = 10;
    public int zMax = 10;
    public int scale = 1;
    public int xScale = 1;
    public int zScale = 1;

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

        for (int i = 0, z = 0; z <= zMax; z++)
        {
            for (int x = 0; x <= xMax; x++, i++)
            {
                tempVertices.Add(new Vector3(x, 0, z));
            }
        }

        List<int> tempTriangles = new List<int>();

        for (int v = 0, t = 0, z = 0; z < zMax; z++, v++)
        {
            for (int x = 0; x < xMax; x++, v++, t+= 6)
            {
                tempTriangles.Add(v + 0);
                tempTriangles.Add(v + xMax + 1);
                tempTriangles.Add(v + 1);
                tempTriangles.Add(v + 1);
                tempTriangles.Add(v + xMax + 1);
                tempTriangles.Add(v + xMax + 2);
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
