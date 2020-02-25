using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSpawner : MonoBehaviour
{
    public static MainSpawner S;

    public int spawnIndex;
    public GameObject activeHead;

    public Biome[] allBiomes;
    Biome currentBiome;

    void Start()
    {
        if (S == null) S = this;
        if (activeHead == null) activeHead = transform.parent.gameObject; //Keep Main Spawner as child of head

        spawnIndex = Random.Range(0, allBiomes.Length);
        currentBiome = allBiomes[spawnIndex];

        currentBiome.creatureSpawn = currentBiome.creatureSpawners[Random.Range(0, currentBiome.creatureSpawners.Count)];
        currentBiome.creatureSpawn = Instantiate(currentBiome.creatureSpawn, this.transform);
        currentBiome.treeSpawn = currentBiome.treeSpawners[Random.Range(0, currentBiome.treeSpawners.Count)];
        currentBiome.treeSpawn = Instantiate(currentBiome.treeSpawn, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ChangeSpawner();
        }
    }

    public void ChangeSpawner()
    {
        currentBiome.creatureSpawn.GetComponent<CreatureSpawner>().DestroySpawner();

        spawnIndex++;
        if (spawnIndex >= allBiomes.Length) spawnIndex = 0;
        currentBiome = allBiomes[spawnIndex];

        currentBiome.creatureSpawn = currentBiome.creatureSpawners[Random.Range(0, currentBiome.creatureSpawners.Count)];
        currentBiome.creatureSpawn = Instantiate(currentBiome.creatureSpawn, this.transform);
        //ColorController.S.SetActiveHead(activeHead);

    }
}
