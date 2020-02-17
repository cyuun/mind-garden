using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSpawner : MonoBehaviour
{
    public static MainSpawner S;

    public List<GameObject> spawnerPrefabs;
    public GameObject selectedSpawner;
    int spawnIndex;

    void Start()
    {
        if (S == null) S = this;

        spawnIndex = Random.Range(0, spawnerPrefabs.Count);
        selectedSpawner = spawnerPrefabs[spawnIndex];
        selectedSpawner = Instantiate(selectedSpawner, this.transform);

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
        selectedSpawner.SetActive(false);

        spawnIndex++;
        if (spawnIndex >= spawnerPrefabs.Count) spawnIndex = 0;
        selectedSpawner = spawnerPrefabs[spawnIndex];
        selectedSpawner = Instantiate(selectedSpawner, this.transform);

    }
}
