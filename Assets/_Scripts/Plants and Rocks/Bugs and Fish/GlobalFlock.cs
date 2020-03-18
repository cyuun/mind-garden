using UnityEngine;
using System.Collections;

public class GlobalFlock : MonoBehaviour
{

    public GameObject bugPrefab;
    public GameObject goalPrefab;
    public int boundsSize = 1;

    static int numBugs = 15;
    public GameObject[] allBugs = new GameObject[numBugs];
    
    public Vector3 goalPos = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < numBugs; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(transform.position.x-boundsSize, transform.position.x+ boundsSize),
                transform.position.y,
                Random.Range(transform.position.z - boundsSize, transform.position.z + boundsSize)
            );
            allBugs[i]= (GameObject)Instantiate(
                bugPrefab, pos, Quaternion.identity, this.transform);
            allBugs[i].GetComponent<bug>()._flock = this;
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
            RaycastHit hit;
            float heightAboveGround = 0f;
            if (Physics.Raycast(goalPrefab.transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                heightAboveGround = hit.distance;
            }
            if (heightAboveGround<1.5f)
            {
                goalPos = new Vector3(
                Random.Range(transform.position.x - boundsSize, transform.position.x + boundsSize),
                Random.Range(transform.position.y, transform.position.y + boundsSize),
                Random.Range(transform.position.z - boundsSize, transform.position.z + boundsSize)
            );
            }
            else
            {
                goalPos = new Vector3(
                Random.Range(transform.position.x - boundsSize, transform.position.x + boundsSize),
                Random.Range(transform.position.y-boundsSize, transform.position.y + boundsSize),
                Random.Range(transform.position.z - boundsSize, transform.position.z + boundsSize)
            );
            }
            
            goalPrefab.transform.position = goalPos;
        }
    }
}