using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public static Transform ROCK_PARENT;

    public GameObject[] rockPrefabs;
    [Range(1,10)]
    public float spawnRadiusMin;
    [Range(11, 100)]
    public float spawnRadiusMax;
    public int numOfRocks;
    public float scaleMin = 1;
    public float scaleMax = 2;

    void Start()
    {
        //Choose rocks at random
        //Cluster some number of unique rocks around different orbs given certain radius

        if (ROCK_PARENT == null)
        {
            GameObject go = new GameObject("_RockParent");
            //go.layer = LayerMask.NameToLayer("Rocks");
            go.layer = LayerMask.NameToLayer("Rocks");
            go.tag = "Rocks";
            go.transform.SetParent(TerrainScript.S.transform);
            ROCK_PARENT = go.transform;
        }

        GenerateRocks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateRocks()
    {
        for(int i = 0; i < numOfRocks; i++)
        {
            //Select random rock prefab
            GameObject selectedRock = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            //Get/Set position
            Vector3 offsetFromOrb;
            do
            {
                offsetFromOrb = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * Random.Range(1, spawnRadiusMax);
            } while (offsetFromOrb.magnitude < spawnRadiusMin);
            Vector3 pos = transform.position + offsetFromOrb;
            //Level with terrain
            pos.y = TerrainScript.S.GetTerrainHeight(pos.x, pos.z);
            GameObject myRock = Instantiate(selectedRock, pos, Random.rotation, ROCK_PARENT);
            //Resize
            float scale = Random.Range(scaleMin, scaleMax);
            myRock.transform.localScale *= scale;
        }
    }
}
