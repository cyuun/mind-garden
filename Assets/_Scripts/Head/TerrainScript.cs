using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainScript : MonoBehaviour
{
    //lattice variables
    [Header("Lattice")]
    public float xMax = 100;         //the max x size of the canvas for the outline to be "drawn" upon
    public float zMax = 100;         // "   "  z   "   "  "    "     "   "     "     "  "    "     "
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
    [Range(0, 1)]
    public float lowHeightFlattening = 0.5f; //low height flattening should give us flatter terrain but with peakier mountains
    public int seed = 1;

    //pond variables
    [Header("Center Pond")]
    public float pondDepth = 4.20f;
    public float pondRadius = 10;
    public float centerFlatteningRadius = 20;
    [Range(0.00001f, 100)]
    public float pondNoiseScale = 1;
    public float pondNoiseAmplitude = 5;

    //painting variables
    [Header("Painting")]
    public int textureResolution = 333;
    public float paintRadius = 5;
    [Range(0, 1)]
    public float brushHardness = 0.5f;
    public Color paintColor;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;
    [SerializeField]
    private Vector3[] _vertices;
    [SerializeField]
    private int[] _triangles;

    private System.Random _rng;
    private Vector2[] _offsets;

    private bool _randomizeTerrain = false;

    private float _seaLevel;
    private float _minHeight;

    private float[] _pond;

    private MeshRenderer _meshRenderer;
    private Texture2D _texture;
    private GameObject _player;
    private bool _paintable;
    private Color[] _paint;

    public void RefreshTerrain()
    {
        ClampVariables();

        SetRandom();
        SetSeaLevel(20,360);
        SetMinHeight(40, 360);
        SetPond(360);

        GenerateTerrain();
        UpdateTerrainMesh();
    }

    public float GetTerrainHeight(float x, float z)
    {
        float height = GetPerlinHeight(x, z) - _seaLevel;

        if (x * x + z * z <= centerFlatteningRadius * centerFlatteningRadius)
        {
            if (x * x + z * z >= 4 * centerFlatteningRadius * centerFlatteningRadius / 9)
            {
                height *= Mathf.Sqrt(x * x + z * z) * 3 / centerFlatteningRadius - 2;
            }
            else
            {
                height = 0;
            }
        }

        if (height < 0 && height > _minHeight)
        {
            height *= 1 + (height * (lowHeightFlattening - 1) / _minHeight);
        }
        if (height <= _minHeight)
        {
            height *= lowHeightFlattening;
        }

        if (x * x + z * z <= pondRadius * pondRadius)
        {
            float r = Mathf.Sqrt(x * x + z * z);
            float theta;

            if (z > 0)
            {
                theta = Mathf.Atan2(z, x);
            }
            else
            {
                theta = Mathf.Atan2(-z, x) + Mathf.PI;
            }

            int thetaIndex = FindPondIndex(theta, _pond.Length);

            if (thetaIndex != -1 && thetaIndex < 360)
            {
                if (r <= _pond[thetaIndex])
                {
                    height -= pondDepth * (1 - r * r / (_pond[thetaIndex] * _pond[thetaIndex]));
                }
            }
        }

        height += 7;

        return height;
    }

    public float GetSteepestSlope(float x, float z, int samples)
    {
        float maxSlope = 0;
        for (int thetaStep = 0; thetaStep < samples; thetaStep++)
        {
            float theta = thetaStep * Mathf.PI / samples;
            float r = 0.1f;
            x += r * Mathf.Cos(theta);
            z += r * Mathf.Sin(theta);
            float finiteDifference = Mathf.Abs((GetTerrainHeight(x, z) - GetTerrainHeight(-x, -z)) / (2 * r));

            if (finiteDifference > maxSlope)
            {
                maxSlope = finiteDifference;
            }
        }

        return maxSlope;
    }

    public void SetPaintable(bool paintable)
    {
        if (_player != null && paintable == true)
        {
            _paintable = paintable;
        }
        else if (paintable == false)
        {
            _paintable = paintable;
        }
        else
        {
            Debug.Log("Error: unable to set terrain to paintable because the Player game object cannot be found.");
        }
    }

    public Vector3[] GetVertices()
    {
        return (Vector3[])_vertices.Clone();
    }

    public void SetBasePaint(Color color)
    {
        for (int i = 0; i < textureResolution * textureResolution; i++)
        {
            _paint[i] = color;
        }
        
        _texture.SetPixels(_paint);
        _texture.Apply();
        _meshRenderer.material.mainTexture = _texture;
    }

    public float GetXStep()
    {
        return skew / resolution;
    }

    public float GetZStep()
    {
        return Mathf.Sqrt(3) / (2 * resolution * skew);
    }

    private void Awake()
    {
        if (_randomizeTerrain)
        {
            noiseScale = Random.Range(20f, 75f); //Density of peaks
            noiseAmplitude = Random.Range(35f, 50f); //Height of peaks
            noisePersistance = Random.Range(.2f, .4f); //Sharpness of peaks
            noiseLacunarity = Random.Range(1f, 5f); //Bumpiness of hills
        }
        else if (Global.bpm > 0)
        {
            noiseLacunarity *= Global.bpm / 120;
            noisePersistance *= (Mathf.Clamp(Global.bpm, 60, 200) / 120 + 1) / 2;
        }

        seed = Global.seed;

        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _mesh = new Mesh();

        RefreshTerrain();

        _player = GameObject.Find("Player");
        SetPaintable(true);
        _meshRenderer.material.mainTexture = new Texture2D(textureResolution, textureResolution);
        _texture = (Texture2D)_meshRenderer.material.mainTexture;
        _paint = new Color[textureResolution * textureResolution];
    }

    private void Update()
    {
        if (_paintable)
        {
            PaintTerrain();
        }
    }

    private void OnValidate()
    {
        if (_mesh != null)
        {
            RefreshTerrain();
        }
    }

    private void GenerateTerrain()
    {
        List<Vector3> tempVertices = new List<Vector3>();             //list to temporarily store vertices in because we don't know actual total count
        List<int> tempTriangles = new List<int>();                    // "    "     "         "   triangles...

        float xStep = GetXStep();                                     //the horizontal distance from one grid point to another within the same row
        float zStep = GetZStep();                                     //vertical      "       "   "    "    "    "   "       "     "    "  column

        int v = 0, r = 0;                                             //v = number of vertices in the current row being filled
        for (float z = -zMax/2; z <= zMax/2; z += zStep)              //r = number of rows that have been filled yet
        {                                               
            if (v > 0)
            {
                r++;
            }

            for (float x = -xMax/2; x <= xMax/2; x += xStep)
            {
                if (x < -xMax/2 + xStep/4 && r % 2 == 1)              //if we are at the beginning of our potential to create
                {                                                     //an odd indexed row (not necessarily the beginning of the row itself)
                    x -= xStep / 2;                                   //then shift the row back by a half step
                }

                if ((x * x) / (xMax * xMax) + (z * z) / (zMax * zMax) <= 0.1) //checking if the point is within the ellipse defined by the x and z maxes
                {
                    tempVertices.Add(new Vector3(x, GetTerrainHeight(x, z), z));         //add new vertex
                    
                    int vertexLeftIndex = GetIndexLeft(v, tempVertices, xStep, zStep);
                    int vertexDownLeftIndex = GetIndexDownLeft(v, tempVertices, xStep, zStep);
                    int vertexDownRightIndex = GetIndexDownRight(v, tempVertices, xStep, zStep);

                    if (vertexLeftIndex >= 0 && vertexDownLeftIndex >= 0)
                    {
                        tempTriangles.Add(v);
                        tempTriangles.Add(vertexDownLeftIndex);
                        tempTriangles.Add(vertexLeftIndex);
                    }
                    if (vertexDownLeftIndex >= 0 && vertexDownRightIndex >= 0)
                    {
                        tempTriangles.Add(v);
                        tempTriangles.Add(vertexDownRightIndex);
                        tempTriangles.Add(vertexDownLeftIndex);
                    }

                    v++;
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

        UpdateMeshUV();

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();

        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = GetColliderMesh();
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

    private void SetRandom()
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

    private void SetSeaLevel(int rSteps, int thetaSteps)
    {
        float sum = 0;

        for (int rIndex = 0; rIndex < rSteps; rIndex++)
        {
            float r = rIndex * pondRadius / rSteps;
            for (int thetaIndex = 0; thetaIndex < thetaSteps; thetaIndex++)
            {
                float theta = thetaIndex * 2 * Mathf.PI / thetaSteps;
                float x = r * Mathf.Cos(theta);
                float z = r * Mathf.Sin(theta);

                sum += GetPerlinHeight(x, z);
            }
        }

        _seaLevel = sum / (rSteps * thetaSteps);
    }

    private void SetMinHeight(int rSteps, int thetaSteps)
    {
        float minHeight = 0;

        for (int rIndex = 0; rIndex < rSteps; rIndex++)
        {
            float r = rIndex * Mathf.Max(xMax, zMax) / rSteps;
            for (int thetaIndex = 0; thetaIndex < thetaSteps; thetaIndex++)
            {
                float theta = thetaIndex * 2 * Mathf.PI / thetaSteps;
                float x = r * Mathf.Cos(theta);
                float z = r * Mathf.Sin(theta);
                float height = GetPerlinHeight(x, z);
                if (height < minHeight)
                {
                    minHeight = height;
                }
            }
        }

        _minHeight = minHeight;
    }

    private void SetPond(int thetaSteps)
    {
        _pond = new float[thetaSteps];

        for (int thetaIndex = 0; thetaIndex < thetaSteps; thetaIndex++)
        {
            float theta = thetaIndex * 2 * Mathf.PI / thetaSteps;
            float x = pondRadius * Mathf.Cos(theta) / pondNoiseScale + _offsets[0].x;
            float z = pondRadius * Mathf.Sin(theta) / pondNoiseScale + _offsets[0].y;

            _pond[thetaIndex] = pondRadius - (Mathf.PerlinNoise(x, z) * pondNoiseAmplitude);
        }
    }

    private int FindPondIndex(float theta, float arrayLength)
    {
        float thetaStep = 2 * Mathf.PI / arrayLength;
        float diff = Single.MaxValue;
        int thetaIndex = Int32.MaxValue;
        int searchIndex = Mathf.RoundToInt(theta / thetaStep);

        for (int i = -2; i <= 2; i++)
        {
            if (Mathf.Abs((searchIndex + i) * thetaStep - theta) < diff)
            {
                diff = Mathf.Abs(searchIndex * thetaStep + i * thetaStep - theta);
                thetaIndex = searchIndex + i;
            }
        }

        if (thetaIndex < 0 || thetaIndex > arrayLength)
        {
            thetaIndex = -1;
        }

        return thetaIndex;
    }

    private void UpdateMeshUV()
    {
        Vector2[] uv = new Vector2[_vertices.Length];
        for (int i = 0; i < _vertices.Length; i++)
        {
            uv[i] = new Vector2((_vertices[i].x + xMax / 2) / xMax, (_vertices[i].z + zMax / 2) / zMax);
        }

        _mesh.uv = uv;
    }

    private void PaintTerrain()
    {
        Vector3 playerPos = _player.transform.position;

        for (int i = 0; i < textureResolution; i++)
        {
            for (int j = 0; j < textureResolution; j++)
            {
                Vector2 pixelPos = TexPxToWorldPos(new Vector2(i, j));
                if ((pixelPos.x - playerPos.x) * (pixelPos.x - playerPos.x) +
                    (pixelPos.y - playerPos.z) * (pixelPos.y - playerPos.z) <
                    paintRadius * paintRadius)
                {
                    float tLerp;
                    if (brushHardness == 1)
                    {
                        tLerp = 0.5f;
                    }
                    else
                    {
                        tLerp = 0.5f * ((paintRadius - Mathf.Sqrt((pixelPos.x - playerPos.x) * (pixelPos.x - playerPos.x) +
                                                        (pixelPos.y - playerPos.z) * (pixelPos.y - playerPos.z))) / (paintRadius - brushHardness * paintRadius));
                    }
                    _paint[i + textureResolution * j] = Color.Lerp(_paint[i + textureResolution * j], paintColor, tLerp);
                }
            }
        }

        _texture.SetPixels(_paint);
        _texture.Apply();
        _meshRenderer.material.mainTexture = _texture;
    }

    private Vector2 TexPxToWorldPos(Vector2 pos)
    {
        return new Vector2(pos.x * xMax / textureResolution - xMax / 2, pos.y * zMax / textureResolution - zMax / 2);
    }

    private Mesh GetColliderMesh()
    {
        Mesh colliderMesh = new Mesh();
        
        int origVertexCount = _vertices.Length;
        Vector3[] vertices = new Vector3[origVertexCount * 2];
        for (int i = 0; i < origVertexCount; i++)
        {
            vertices[i] = _vertices[i];
            vertices[i + origVertexCount] = new Vector3(_vertices[i].x, _vertices[i].y - 20, _vertices[i].z);
        }
        
        List<int> triangles = new List<int>();
        int origTriangleCount = _triangles.Length;
        for (int i = 0; i < origTriangleCount; i += 3)
        {
            triangles.Add(_triangles[i]);
            triangles.Add(_triangles[i + 1]);
            triangles.Add(_triangles[i + 2]);
            
            triangles.Add(_triangles[i + 2] + origVertexCount);
            triangles.Add(_triangles[i + 1] + origVertexCount);
            triangles.Add(_triangles[i] + origVertexCount);
        }

        float xStep = GetXStep();
        float zStep = GetZStep();
        for (int i = 0; i < origVertexCount; i++)
        {
            int[] surroundingVertices = GetSurroundingVertices(i, xStep, zStep);
            int l0Norm = 0;
            foreach (int vertexIndex in surroundingVertices)
            {
                if (vertexIndex >= 0)
                {
                    l0Norm++;
                }
            }
            if (l0Norm < 6)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (surroundingVertices[j] < 0)
                    {
                        int vertexIndexCW = SearchEdgeVerticesCW(surroundingVertices, j);
                        int vertexIndexCCW = SearchEdgeVerticesCCW(surroundingVertices, j);

                        if (vertexIndexCW >= 0 && vertexIndexCCW >= 0)
                        {
                            triangles.Add(i);
                            triangles.Add(vertexIndexCCW);
                            triangles.Add(vertexIndexCCW + origVertexCount);
                            
                            triangles.Add(i);
                            triangles.Add(vertexIndexCCW + origVertexCount);
                            triangles.Add(i + origVertexCount);
                            
                            triangles.Add(i);
                            triangles.Add(i + origVertexCount);
                            triangles.Add(vertexIndexCW + origVertexCount);
                            
                            triangles.Add(i);
                            triangles.Add(vertexIndexCW + origVertexCount);
                            triangles.Add(vertexIndexCW);
                        }
                        
                        break;
                    }
                }
            }
        }

        colliderMesh.vertices = vertices;
        colliderMesh.triangles = triangles.ToArray();
        
        colliderMesh.RecalculateNormals();
        colliderMesh.RecalculateBounds();
        colliderMesh.RecalculateTangents();

        return colliderMesh;
    }

    private int[] GetSurroundingVertices(int index, float xStep, float zStep)
    {
        float searchTolerance = Mathf.Max(xStep, zStep) * 0.25f;
        int[] vertexIndices = new int[6];
        for (int i = 0; i < 6; i++)
        {
            vertexIndices[i] = -1;
        }

        float targetX = _vertices[index].x;
        float targetZ = _vertices[index].z;
        for (int i = 0; i < _vertices.Length; i++)
        {
            if (Global.CompareFloats(_vertices[i].x, targetX - xStep / 2, searchTolerance) &&
                Global.CompareFloats(_vertices[i].z, targetZ + zStep, searchTolerance))
            {
                vertexIndices[0] = i;
            }
            if (Global.CompareFloats(_vertices[i].x, targetX + xStep / 2, searchTolerance) &&
                Global.CompareFloats(_vertices[i].z, targetZ + zStep, searchTolerance))
            {
                vertexIndices[1] = i;
            }
            if (Global.CompareFloats(_vertices[i].x, targetX + xStep, searchTolerance) &&
                Global.CompareFloats(_vertices[i].z, targetZ, searchTolerance))
            {
                vertexIndices[2] = i;
            }
            if (Global.CompareFloats(_vertices[i].x, targetX + xStep / 2, searchTolerance) &&
                Global.CompareFloats(_vertices[i].z, targetZ - zStep, searchTolerance))
            {
                vertexIndices[3] = i;
            }
            if (Global.CompareFloats(_vertices[i].x, targetX - xStep / 2, searchTolerance) &&
                Global.CompareFloats(_vertices[i].z, targetZ - zStep, searchTolerance))
            {
                vertexIndices[4] = i;
            }
            if (Global.CompareFloats(_vertices[i].x, targetX - xStep, searchTolerance) &&
                Global.CompareFloats(_vertices[i].z, targetZ, searchTolerance))
            {
                vertexIndices[5] = i;
            }
        }

        return vertexIndices;
    }

    private int SearchEdgeVerticesCW(int[] edgeVertexIndices, int index)
    {
        for (int i = NextIndexCW(index, 6); i != index; i = NextIndexCW(i, 6))
        {
            if (edgeVertexIndices[i] >= 0)
            {
                return edgeVertexIndices[i];
            }
        }

        return -1;
    }
    
    private int SearchEdgeVerticesCCW(int[] edgeVertexIndices, int index)
    {
        for (int i = NextIndexCCW(index, 6); i != index; i = NextIndexCCW(i, 6))
        {
            if (edgeVertexIndices[i] >= 0)
            {
                return edgeVertexIndices[i];
            }
        }

        return -1;
    }

    private int NextIndexCW(int index, int arraySize)
    {
        return (index + 1) % arraySize;
    }
    
    private int NextIndexCCW(int index, int arraySize)
    {
        if (index > 0)
        {
            return index - 1;
        }
        
        return arraySize - 1;
    }

    private void ClampVariables()
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
            noiseScale = 0.00001f;
        }
        if (noiseOctaves < 0)
        {
            noiseOctaves = 0;
        }
        if (noiseLacunarity < 1)
        {
            noiseLacunarity = 1;
        }
        if (pondDepth < 0)
        {
            pondDepth = 0.00001f;
        }
        if (pondRadius < 1)
        {
            pondRadius = 1;
        }
        if (pondRadius > Mathf.Min(xMax, zMax))
        {
            pondRadius = Mathf.Min(xMax, zMax);
        }
        if (centerFlatteningRadius < pondRadius)
        {
            centerFlatteningRadius = pondRadius;
        }
        if (pondNoiseAmplitude <= 0)
        {
            pondNoiseAmplitude = 0.00001f;
        }
    }
}
