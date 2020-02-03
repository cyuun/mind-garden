using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public static Transform TREE_PARENT;
    public AudioPeer audioPeer;
    public GameObject[] treePrefabs;
    [Range(1, 10)]
    public float spawnRadiusMin;
    [Range(11, 100)]
    public float spawnRadiusMax;
    public int numOfTrees;

    void Start()
    {
        //Choose rocks at random
        //Cluster some number of unique rocks around different orbs given certain radius

        if (TREE_PARENT == null)
        {
            GameObject go = new GameObject("_TreeParent");
            go.layer = LayerMask.NameToLayer("Trees");
            TREE_PARENT = go.transform;
        }

        GenerateTrees();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateTrees()
    {
        for (int i = 0; i < numOfTrees; i++)
        {
            float yOffset = 0;
            //Select random rock prefab
            GameObject tree = treePrefabs[Random.Range(0, treePrefabs.Length)];
            if (tree.GetComponent<smallTree>())
            {
                tree.GetComponent<smallTree>()._audioPeer = audioPeer;
                yOffset = tree.GetComponent<smallTree>().y_offset;
            }
            else if (tree.GetComponent<medTree>())
            {
                tree.GetComponent<medTree>()._audioPeer = audioPeer;
                yOffset = tree.GetComponent<medTree>().y_offset;

            }
            else if (tree.GetComponent<bigTree>())
            {
                tree.GetComponent<bigTree>()._audioPeer = audioPeer;
                yOffset = tree.GetComponent<bigTree>().y_offset;

            }

            //Get/Set position
            Vector3 offsetFromOrb = Vector3.zero;
            while (offsetFromOrb.magnitude < spawnRadiusMin)
            {
                offsetFromOrb = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * Random.Range(1, spawnRadiusMax);
            } 
            Vector3 pos = transform.position + offsetFromOrb;
            pos.y = TerrainScript.S.GetTerrainHeight(pos.x, pos.z);
            pos.y += yOffset;
            GameObject myTree = Instantiate(tree, pos, Quaternion.identity, TREE_PARENT);
        }
    }
}
