using UnityEngine;
using System.Collections;

public class GlobalFlock3 : MonoBehaviour
{

    public GameObject bugPrefab;
    public GameObject goalPrefab;
    public static int boundsSize = 3;

    static int numBugs = 2;
    public static GameObject[] allBugs = new GameObject[numBugs];
    public static Vector3 goalPos = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        transform.position = ResetYPosition(transform.position);

        for (int i = 0; i < numBugs; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(transform.position.x-boundsSize, transform.position.x+ boundsSize),
                Random.Range(transform.position.y - boundsSize, transform.position.y + boundsSize),
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
                TerrainScript.S.GetTerrainHeight(xPos, zPos) + 2,
                zPos
            );
            goalPrefab.transform.position = goalPos;
        }
    }

    Vector3 ResetYPosition(Vector3 pos)
    {
        Vector3 Ypos = pos;
        Ypos.y = TerrainScript.S.GetTerrainHeight(Ypos.x, Ypos.z) + 2;
        return pos;
    }
}