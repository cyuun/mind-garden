using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public static Transform TREE_PARENT;
    public AudioPeer audioPeer;
    public bool useAsParent = false;
    public GameObject[] treePrefabs;

    public int forestCount = 5;
    const int spawnRadiusMin = 15;
    [Range(16, 200)]
    public float spawnRadiusMax;
    [Range(1,20)]
    public int treesMin,treesMax;
    public float treeSeparation;
    [Range(30, 60)]
    public float maxSlope;
    
    public TerrainScript terrainScript;

    public void SetParent()
    {
        if (useAsParent) TREE_PARENT = this.transform;
        if (TREE_PARENT == null)
        {
            GameObject go = new GameObject("_TreeParent");
            go.layer = LayerMask.NameToLayer("Trees");
            go.tag = "Trees";
            go.transform.SetParent(terrainScript.transform);
            TREE_PARENT = go.transform;
        }
    }

    public void GenerateTrees()
    {
        for (int f = 0; f < forestCount; f++)
        {
            int numOfTrees = Random.Range(treesMin, treesMax);

            Vector3 offset = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * Random.Range(spawnRadiusMin, spawnRadiusMax);
            Vector3 pos = transform.position + offset;
            Vector3 treePos = pos;

            for (int i = 0; i < numOfTrees; i++)
            {
                float yOffset = 0;
                bool hitRock = true;
                bool hitPond = true;
                bool hitTree = true;
                bool onTerrain = false;
                bool inHead = false;
                bool tooSteep = false;

                //Select random treefab
                GameObject tree = treePrefabs[Random.Range(0, treePrefabs.Length)];
                
                //Get/Set position
                treePos = GetTreePos(treePos);
                treePos.y = terrainScript.GetTerrainHeight(treePos.x, treePos.z) + yOffset;
                while (hitRock || hitPond || hitTree || inHead || !onTerrain|| tooSteep)
                {
                    hitRock = false;
                    hitPond = false;
                    hitTree = false;
                    onTerrain = false;
                    inHead = false;
                    tooSteep = false;

                    treePos = GetTreePos(treePos);
                    treePos.y = terrainScript.GetTerrainHeight(treePos.x, treePos.z) + yOffset;
                    if (terrainScript.GetSteepestSlope(treePos.x, treePos.z, 50) > maxSlope)
                    {
                        tooSteep = true;
                        break;
                    }

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

                        if (c.name.Contains("Head"))
                        {
                            inHead = true;
                        }
                    }

                    foreach (Collider c in Physics.OverlapSphere(treePos, 1f))
                    {
                        if (c.name.Contains("Terrain"))
                        {
                            onTerrain = true;
                        }
                    }
                }

                //Instantiate and assign script info
                if (onTerrain && !inHead)
                {
                    float yRot = Random.Range(0, 360);
                    GameObject tre = Instantiate(tree, treePos, Quaternion.identity, TREE_PARENT);
                    tre.transform.rotation = Quaternion.Euler(new Vector3(0, yRot, 0));

                    AudioSource closest = AudioPeerRoot.S.audioPeers[0];
                    foreach (AudioSource o in AudioPeerRoot.S.audioPeers)
                    {
                        float min = Vector3.Distance(treePos, closest.transform.position);
                        float dist = Vector3.Distance(treePos, o.transform.position);
                        if (dist < min)
                        {
                            min = dist;
                            closest = o;
                        }
                    }
                    audioPeer = closest.GetComponent<AudioPeer>();
                    tre = tre.transform.GetChild(0).gameObject;
                    if (tre.GetComponent<smallTree>())
                    {
                        smallTree t = tre.GetComponent<smallTree>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<medTree>())
                    {
                        medTree t = tre.GetComponent<medTree>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<bigTree>())
                    {
                        bigTree t = tre.GetComponent<bigTree>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<underwaterPlantBig>())
                    {
                        underwaterPlantBig t = tre.GetComponent<underwaterPlantBig>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<underwaterPlantSmall>())
                    {
                        underwaterPlantSmall t = tre.GetComponent<underwaterPlantSmall>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<underwaterPlantSpeed>())
                    {
                        underwaterPlantSpeed t = tre.GetComponent<underwaterPlantSpeed>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<junglePlantBig>())
                    {
                        junglePlantBig t = tre.GetComponent<junglePlantBig>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<junglePlantSmall>())
                    {
                        junglePlantSmall t = tre.GetComponent<junglePlantSmall>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<desertPlantBig>())
                    {
                        desertPlantBig t = tre.GetComponent<desertPlantBig>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<desertPlantMedium>())
                    {
                        desertPlantMedium t = tre.GetComponent<desertPlantMedium>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                    else if (tre.GetComponent<desertPlantSmall>())
                    {
                        desertPlantSmall t = tre.GetComponent<desertPlantSmall>();
                        t._audioPeer = audioPeer;
                        t.spawner = this;
                        yOffset = t.y_offset;
                        t.terrainScript = terrainScript;
                    }
                }
            }
        }

    }

    public Vector3 GetTreePos(Vector3 startPos)
    {
        Vector3 treeOffset = new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z).normalized * treeSeparation;
        Vector3 pos = startPos + treeOffset;
        return pos;
    }
    
    public void DestroySpawner()
    {
        foreach (Transform t in TREE_PARENT)
        {
            Destroy(t.gameObject);
        }
        Destroy(this.gameObject);
    }
}
