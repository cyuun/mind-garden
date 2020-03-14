using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public static Transform CREATURE_PARENT;
    public bool useAsParent = false;
    public GameObject[] prefabs;

    const int spawnRadiusMin = 15;
    [Range(16, 100)]
    public float spawnRadiusMax;
    [Range(1, 30)]
    public int minSpawn, maxSpawn;

    public bool flying;
    public float yOffset;
    
    public TerrainScript terrainScript;

    public void SetParent()
    {
        if (useAsParent) CREATURE_PARENT = this.transform;
        if (CREATURE_PARENT == null)
        {
            GameObject go = new GameObject("_CreatureParent");
            go.layer = LayerMask.NameToLayer("Creatures");
            go.tag = "Creatures";
            go.transform.SetParent(terrainScript.transform);
            CREATURE_PARENT = go.transform;
        }
    }

    public void SpawnCreatures()
    {
        int spawnCount = Random.Range(minSpawn, maxSpawn);
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject creature = prefabs[Random.Range(0, prefabs.Length)];
            Vector3 pos;
            if (flying)
            {
                float yValue = Random.insideUnitSphere.y;
                if (yValue < 0) yValue *= -1;
                pos = transform.position + (new Vector3(Random.insideUnitSphere.x, yValue, Random.insideUnitSphere.z).normalized * Random.Range(spawnRadiusMin, spawnRadiusMax));
                pos.y += yOffset;
            }
            else
            {
                pos = transform.position + (new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z).normalized * Random.Range(spawnRadiusMin, spawnRadiusMax));
                pos.y = terrainScript.GetTerrainHeight(pos.x, pos.z) + yOffset;
            }
            creature = Instantiate(creature, pos, Quaternion.Euler(new Vector3(0,Random.Range(0f,360f),0)), CREATURE_PARENT);
            if (creature.GetComponent<GlobalFlock>())
            {
                creature.GetComponent<GlobalFlock>().SpawnBugs();
            }
            else if (creature.GetComponent<GlobalFlock2>())
            {
                creature.GetComponent<GlobalFlock2>().SpawnBugs();
            }
            else if (creature.GetComponent<GlobalFlock3>())
            {
                creature.GetComponent<GlobalFlock3>().SpawnBugs();
            }
        }
    }

    public void DestroySpawner()
    {
        foreach(Transform t in CREATURE_PARENT)
        {
            Destroy(t.gameObject);
        }
        Destroy(this.gameObject);
    }
}
