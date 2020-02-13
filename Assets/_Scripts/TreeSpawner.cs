using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public static Transform TREE_PARENT;
    public AudioPeer audioPeer;
    public GameObject[] treePrefabs;

    public int forestCount = 5;
    const int spawnRadiusMin = 15;
    [Range(16, 100)]
    public float spawnRadiusMax;
    [Range(1,20)]
    public int treesMin,treesMax;
    public float treeSeparation;

    void Start()
    {
        //Choose rocks at random
        //Cluster some number of unique rocks around different orbs given certain radius

        if (TREE_PARENT == null)
        {
            GameObject go = new GameObject("_TreeParent");
            go.layer = LayerMask.NameToLayer("Trees");
            go.tag = "Trees";
            go.transform.SetParent(TerrainScript.S.transform);
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
        for (int f = 0; f < forestCount; f++)
        {
            int numOfTrees = Random.Range(treesMin, treesMax);

            Vector3 offset = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * Random.Range(spawnRadiusMin, spawnRadiusMax);
            Vector3 pos = transform.position + offset;
            pos.y = TerrainScript.S.GetTerrainHeight(pos.x, pos.z);
            Vector3 treePos = pos;

            for (int i = 0; i < numOfTrees; i++)
            {
                float yOffset = 0;
                bool hitRock = true;
                bool hitPond = true;
                bool hitTree = true;
                //Select random rock prefab
                GameObject tree = treePrefabs[Random.Range(0, treePrefabs.Length)];
                if (tree.GetComponent<smallTree>())
                {
                    smallTree t = tree.GetComponent<smallTree>();
                    t._audioPeer = audioPeer;
                    t.spawner = this;
                    yOffset = t.y_offset;
                }
                else if (tree.GetComponent<medTree>())
                {
                    medTree t = tree.GetComponent<medTree>();
                    t._audioPeer = audioPeer;
                    t.spawner = this;
                    yOffset = t.y_offset;

                }
                else if (tree.GetComponent<bigTree>())
                {
                    bigTree t = tree.GetComponent<bigTree>();
                    t._audioPeer = audioPeer;
                    t.spawner = this;
                    yOffset = t.y_offset;
                }

                //Get/Set position
                treePos = GetTreePos(treePos);
                treePos.y = TerrainScript.S.GetTerrainHeight(treePos.x, treePos.z);
                while (hitRock || hitPond || hitTree)
                {
                    hitRock = false;
                    hitPond = false;
                    hitTree = false;

                    treePos = GetTreePos(treePos);
                    treePos.y = TerrainScript.S.GetTerrainHeight(treePos.x, treePos.z);

                    foreach (Collider c in Physics.OverlapSphere(treePos, treeSeparation))
                    {
                        if (c.name.Contains("Sphere"))
                        {
                            hitRock = true;
                            break;
                        }
                        else if (c.name.Contains("Pond"))
                        {
                            hitPond = true;
                            break;
                        }
                        else if (c.gameObject.tag == "Trees")
                        {
                            hitTree = true;
                            break;
                        }
                    }
                }
                GameObject myTree = Instantiate(tree, treePos, Quaternion.identity, TREE_PARENT);
            }
        }

    }

    public Vector3 GetTreePos(Vector3 startPos)
    {
        Vector3 treeOffset = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * treeSeparation;
        Vector3 pos = startPos + treeOffset;
        return pos;
    }

}
