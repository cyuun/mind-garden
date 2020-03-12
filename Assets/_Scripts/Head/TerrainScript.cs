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
    public Color paintColor;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;
    private Vector3[] _vertices;
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
        else
        {
            //TODO: Determine parameters based on audio analyzer class
        }

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

        UpdateMeshUV();

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
                    _paint[i + textureResolution * j] = Color.Lerp(_paint[i + textureResolution * j], paintColor, 0.5f);
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
