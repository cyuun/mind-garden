using UnityEngine;
using System.Collections;

public class GlobalFlock : MonoBehaviour
{

    public GameObject bugPrefab;
    public GameObject goalPrefab;
    public static int boundsSize = 1;

    static int numBugs = 15;
    public static GameObject[] allBugs = new GameObject[numBugs];
    public static Vector3 goalPos = Vector3.zero;

    bool hitRock = false;

    // Use this for initialization
    void Start()
    {

        transform.position = ResetYPosition(transform.position);

        for (int i = 0; i < numBugs; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(transform.position.x-boundsSize, transform.position.x+ boundsSize),
                transform.position.y,
                Random.Range(transform.position.z - boundsSize, transform.position.z + boundsSize)
            );
            allBugs[i]= (GameObject)Instantiate(
                bugPrefab, pos, Quaternion.identity, this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        HandleGoalPos();

        foreach(GameObject bug in allBugs)
        {
            bug.transform.position = ResetYPosition(bug.transform.position);
        }
    }

    void HandleGoalPos()
    {
        if (Random.Range(1, 5000) < 50)
        {/*
            hitRock = true;
            while (hitRock)
            {
                hitRock = false;*/
                goalPos = new Vector3(
                    Random.Range(transform.position.x - boundsSize, transform.position.x + boundsSize),
                    transform.position.y,
                    Random.Range(transform.position.z - boundsSize, transform.position.z + boundsSize)
                );
                /*foreach (Collider c in Physics.OverlapSphere(goalPos, 1f))
                {
                    if (c.name.Contains("Sphere"))
                    {
                        hitRock = true;
                    }
                }
            }*/
            goalPrefab.transform.position = goalPos;
        }
    }

    Vector3 ResetYPosition(Vector3 pos)
    {
        Vector3 Ypos = pos;
        Ypos.y = TerrainScript.S.GetTerrainHeight(Ypos.x, Ypos.z) + 2;
        pos = Ypos;
        return pos;
    }
}