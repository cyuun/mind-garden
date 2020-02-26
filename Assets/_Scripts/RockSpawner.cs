using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public static Transform ROCK_PARENT;

    public GameObject[] rockPrefabs;
    [Range(1,10)]
    public float spawnRadiusMin;
    [Range(11, 200)]
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
            bool hitRock = true;
            bool hitPond = true;
            bool onTerrain = false;
            bool inHead = false;

            //Select random rock prefab
            GameObject selectedRock = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

            //Get/Set position
            Vector3 rockPos = transform.position;
            while (hitRock || hitPond)
            {
                hitRock = false;
                hitPond = false;
                onTerrain = false;
                inHead = false;

                rockPos += new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z).normalized * Random.Range(spawnRadiusMin, spawnRadiusMax);
                rockPos.y = TerrainScript.S.GetTerrainHeight(rockPos.x, rockPos.z);
                foreach (Collider c in Physics.OverlapSphere(rockPos, 3f))
                {
                    if (c.name.Contains("Sphere") || c.name.Contains("Orb"))
                    {
                        hitRock = true;
                        break;
                    }
                    else if (c.name.Contains("Pond"))
                    {
                        hitPond = true;
                        break;
                    }

                    if (c.name.Contains("Terrain"))
                    {
                        onTerrain = true;
                    }
                    if (c.name.Contains("Head"))
                    {
                        inHead = true;
                    }
                }
            }

            if(onTerrain && !inHead)
            {
                GameObject myRock = Instantiate(selectedRock, rockPos, Random.rotation, ROCK_PARENT);
                //Resize
                float scale = Random.Range(scaleMin, scaleMax);
                myRock.transform.localScale *= scale;
            }
        }
    }
}
