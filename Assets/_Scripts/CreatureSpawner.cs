using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public static Transform CREATURE_PARENT;
    public GameObject[] prefabs;

    const int spawnRadiusMin = 15;
    [Range(16, 100)]
    public float spawnRadiusMax;
    [Range(1, 20)]
    public int minSpawn, maxSpawn;
    public float yOffset;

    void Start()
    {
        if (CREATURE_PARENT == null)
        {
            GameObject go = new GameObject("_CreatureParent");
            go.layer = LayerMask.NameToLayer("Creatures");
            go.tag = "Creatures";
            go.transform.SetParent(TerrainScript.S.transform);
            CREATURE_PARENT = go.transform;
        }

        SpawnCreatures();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCreatures()
    {
        int spawnCount = Random.Range(minSpawn, maxSpawn);
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject creature = prefabs[Random.Range(0, prefabs.Length)];

            Vector3 pos = transform.position + (new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z).normalized * Random.Range(spawnRadiusMin,spawnRadiusMax));
            pos.y = TerrainScript.S.GetTerrainHeight(pos.x, pos.z) + yOffset;
            creature = Instantiate(creature, pos, Quaternion.identity, CREATURE_PARENT);
        }
    }
}
