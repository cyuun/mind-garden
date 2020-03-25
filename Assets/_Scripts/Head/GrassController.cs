using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrassController : MonoBehaviour
{
    [Range(0, 1)] public float grassPatchSize = 0.5f;
    public float grassNoiseScale = 10;
    public TerrainScript terrainScript;

    public GameObject grassPrefab;
    
    private System.Random _rng;
    private Vector2[] _offsets;

    private int _maxSearch = 10;

    public void GenerateGrass()
    {
        SetRandom();
        SetOffsets();
        
        Vector3[] vertices = CullVertices(terrainScript.GetVertices());
        int[] patchNumbers = AssignPatchNumbers(vertices);
        List<Vector3>[] patchedVertices = SeparateVertices(vertices, patchNumbers);
        List<int>[] patchedTriangles = GenerateTriangles(patchedVertices);

        for (int i = 0; i < patchedVertices.Length; i++)
        {
            GameObject grassPatch = Instantiate(grassPrefab, transform.position, Quaternion.identity, transform);
            Grass grassScript = grassPatch.GetComponent<Grass>();
            grassScript.Initialize();
            grassScript.SetMesh(patchedVertices[i].ToArray(), patchedTriangles[i].ToArray());
        }
    }

    public void AdjustRotation(Vector3 eulerAngles)
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(currentRotation.x, currentRotation.y - (180 - eulerAngles.y), currentRotation.z));
    }

    private Vector3[] CullVertices(Vector3[] vertices)
    {
        List<Vector3> newVertices = new List<Vector3>();
        
        for (int i = 0; i < vertices.Length; i++)
        {
            float xPerlin0 = vertices[i].x / grassNoiseScale + _offsets[0].x;
            float zPerlin0 = vertices[i].z / grassNoiseScale + _offsets[0].y;
            
            float xPerlin1 = 2 * vertices[i].x / grassNoiseScale + _offsets[1].x;
            float zPerlin1 = 2 * vertices[i].z / grassNoiseScale + _offsets[1].y;
            
            float xPerlin2 = 0.5f * vertices[i].x / grassNoiseScale + _offsets[2].x;
            float zPerlin2 = 0.5f * vertices[i].z / grassNoiseScale + _offsets[2].y;
            
            float xPerlin3 = 1.5f * vertices[i].x / grassNoiseScale + _offsets[3].x;
            float zPerlin3 = 1.5f * vertices[i].z / grassNoiseScale + _offsets[3].y;

            float perlinValue = Mathf.PerlinNoise(xPerlin0, zPerlin0) * Mathf.PerlinNoise(zPerlin1, xPerlin1) *
                                Mathf.PerlinNoise(xPerlin2, zPerlin2) * Mathf.PerlinNoise(zPerlin3, xPerlin3);

            if (Mathf.Pow(grassPatchSize, 4) > perlinValue && Mathf.Sqrt(vertices[i].x * vertices[i].x + vertices[i].z * vertices[i].z) > terrainScript.paintRadius)
            {
                newVertices.Add(new Vector3(vertices[i].x, terrainScript.GetTerrainHeight(vertices[i].x, vertices[i].z), vertices[i].z));
            }
        }

        return newVertices.ToArray();
    }

    private int[] AssignPatchNumbers(Vector3[] vertices)
    {
        List<int> patchNumbers = new List<int>();

        float searchRadius = Mathf.Max(terrainScript.GetXStep(), terrainScript.GetZStep()) * 1.25f;
        int nextPatchIndex = 0;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            List<int> searchedIndices = new List<int>();
            int neighborVertexIndex = SearchVerticesXZ(vertices, vertices[i].x, vertices[i].z, searchRadius, searchedIndices);
            searchedIndices.Add(neighborVertexIndex);
            if (neighborVertexIndex >= 0)
            {
                if (neighborVertexIndex >= patchNumbers.Count)
                {
                    bool patchFound = false;
                    for (int j = 0; j < _maxSearch; j++)
                    {
                        neighborVertexIndex = SearchVerticesXZ(vertices, vertices[neighborVertexIndex].x,
                            vertices[neighborVertexIndex].z, searchRadius, searchedIndices);
                        searchedIndices.Add(neighborVertexIndex);
                        if (neighborVertexIndex < 0)
                        {
                            break;
                        }
                        if (neighborVertexIndex < patchNumbers.Count)
                        {
                            patchNumbers.Add(patchNumbers[neighborVertexIndex]);
                            patchFound = true;
                        }
                    }
                    if (!patchFound)
                    {
                        patchNumbers.Add(nextPatchIndex);
                        nextPatchIndex++;
                    }
                }
                else
                {
                    patchNumbers.Add(patchNumbers[neighborVertexIndex]);
                }
            }
            else
            {
                patchNumbers.Add(nextPatchIndex);
                nextPatchIndex++;
            }
        }

        return patchNumbers.ToArray();
    }

    private int SearchVerticesXZ(Vector3[] vertices, float x, float z, float radius, List<int> searchedIndices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Mathf.Sqrt((vertices[i].x - x) * (vertices[i].x - x) + (vertices[i].z - z) * (vertices[i].z - z)) <
                radius)
            {
                bool skip = false;
                for (int j = 0; j < searchedIndices.Count; j++)
                {
                    if (i == searchedIndices[j])
                    {
                        skip = true;
                    }
                }
                if (!skip)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    private List<Vector3>[] SeparateVertices(Vector3[] vertices, int[] patchNums)
    {
        int numPatches = GetNumPatches(patchNums);
        List<Vector3>[] separatedVertices = new List<Vector3>[numPatches];
        for (int i = 0; i < numPatches; i++)
        {
            separatedVertices[i] = new List<Vector3>();
            for (int j = 0; j < vertices.Length; j++)
            {
                if (patchNums[j] == i)
                {
                    separatedVertices[i].Add(vertices[j]);
                }
            }
        }

        return separatedVertices;
    }

    private int GetNumPatches(int[] patchNumbers)
    {
        int numPatches = 0;
        foreach (int patchNumber in patchNumbers)
        {
            if (patchNumber > numPatches)
            {
                numPatches = patchNumber;
            }
        }

        return numPatches + 1;
    }

    private List<int>[] GenerateTriangles(List<Vector3>[] vertices)
    {
        float xStep = terrainScript.GetXStep();
        float zStep = terrainScript.GetZStep();
        List<int>[] triangles = new List<int>[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            triangles[i] = new List<int>();
            for (int j = 0; j < vertices[i].Count; j++)
            {
                int vertexLeftIndex = GetIndexLeft(j, vertices[i], xStep, zStep);
                int vertexDownLeftIndex = GetIndexDownLeft(j, vertices[i], xStep, zStep);
                int vertexDownRightIndex = GetIndexDownRight(j, vertices[i], xStep, zStep);

                if (vertexLeftIndex >= 0 && vertexDownLeftIndex >= 0)
                {
                    triangles[i].Add(j);
                    triangles[i].Add(vertexDownLeftIndex);
                    triangles[i].Add(vertexLeftIndex);
                }
                if (vertexDownLeftIndex >= 0 && vertexDownRightIndex >= 0)
                {
                    triangles[i].Add(j);
                    triangles[i].Add(vertexDownRightIndex);
                    triangles[i].Add(vertexDownLeftIndex);
                }
            }
        }

        return triangles;
    }

    private int GetIndexLeft(int index, List<Vector3> vertices, float xStep, float zStep)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (Global.CompareFloats(vertices[i].x, vertices[index].x - xStep, xStep / 4) && 
                Global.CompareFloats(vertices[i].z, vertices[index].z, zStep / 4))
            {
                return i;
            }
        }

        return -1;
    }

    private int GetIndexDownLeft(int index, List<Vector3> vertices, float xStep, float zStep)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (Global.CompareFloats(vertices[i].x, vertices[index].x - xStep / 2, xStep / 4) && 
                Global.CompareFloats(vertices[i].z, vertices[index].z - zStep, zStep / 4))
            {
                return i;
            }
        }

        return -1;
    }

    private int GetIndexDownRight(int index, List<Vector3> vertices, float xStep, float zStep)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (Global.CompareFloats(vertices[i].x, vertices[index].x + xStep / 2, xStep / 4) && 
                Global.CompareFloats(vertices[i].z, vertices[index].z - zStep, zStep / 4))
            {
                return i;
            }
        }

        return -1;
    }

    private void SetRandom()
    {
        _rng = new System.Random(terrainScript.seed);
    }

    private void SetOffsets()
    {
        _offsets = new Vector2[4];
        _offsets[0] = new Vector2(_rng.Next(Mathf.CeilToInt(terrainScript.xMax) / 2, 10000), _rng.Next(Mathf.CeilToInt(terrainScript.zMax) / 2, 10000));
        _offsets[1] = new Vector2(_rng.Next(Mathf.CeilToInt(terrainScript.xMax) / 2, 10000), _rng.Next(Mathf.CeilToInt(terrainScript.zMax) / 2, 10000));
        _offsets[2] = new Vector2(_rng.Next(Mathf.CeilToInt(terrainScript.xMax) / 2, 10000), _rng.Next(Mathf.CeilToInt(terrainScript.zMax) / 2, 10000));
        _offsets[3] = new Vector2(_rng.Next(Mathf.CeilToInt(terrainScript.xMax) / 2, 10000), _rng.Next(Mathf.CeilToInt(terrainScript.zMax) / 2, 10000));
    }
}
