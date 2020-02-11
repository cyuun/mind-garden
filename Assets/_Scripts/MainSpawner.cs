using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSpawner : MonoBehaviour
{
    public static Transform SPAWNER_PARENT;

    float bpm;
    float songLength;

    public float spawnerRadius;
    public int forestCount;
    [Range(1,20)]
    public int treesMin,treesMax;
    public float treeSeparation;

    public GameObject[] treePrefabs;


    void Start()
    {
        if (SPAWNER_PARENT == null)
        {
            GameObject go = new GameObject("_Spawns");
            go.transform.SetParent(TerrainScript.S.transform);
            SPAWNER_PARENT = go.transform;
        }

        CreateForests();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateForests()
    {
        for(int f = 0; f < forestCount; f++)
        {
            int treeCount = Random.Range(treesMin, treesMax);

            Vector3 offset = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * Random.Range(1, spawnerRadius);
            Vector3 pos = transform.position + offset;
            pos.y = TerrainScript.S.GetTerrainHeight(pos.x, pos.z);


            

        }
    }
}
