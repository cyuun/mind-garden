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
    }

    void HandleGoalPos()
    {
        if (Random.Range(1, 5000) < 50)
        {
            float xPos = Random.Range(transform.position.x - boundsSize, transform.position.x + boundsSize);
            float zPos = Random.Range(transform.position.z - boundsSize, transform.position.z + boundsSize);
            goalPos = new Vector3(
                xPos,
                transform.position.y,
                zPos
            );
            goalPrefab.transform.position = goalPos;
        }
    }

    public static Vector3 ResetYPosition(Vector3 pos)
    {
        Vector3 Ypos = pos;
        Ypos.y = TerrainScript.S.GetTerrainHeight(Ypos.x, Ypos.z) + 2;
        return pos;
    }
}