using UnityEngine;
using System.Collections;

public class bug2 : MonoBehaviour
{
    public float musicMultipier = 2f;
    public float speed=0.1f;
    public float rotationSpeed = 4.0f;
    float minSpeed = 0.3f;
    float maxSpeed = 1f;
    //public float turnSpeed = 4.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighborDistance = 3.0f;
    public Vector3 newGoalPos;

    bool turning = false;

    // Use this for initialization
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!turning && other.isTrigger)
        {
            newGoalPos = transform.position - other.gameObject.transform.position;
        }
        turning = true;
    }
    private void OnTriggerExit(Collider other )
    {
        if(other.isTrigger)
        turning = false;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyBoundary();

        if (turning)
        {
            Vector3 direction = GlobalFlock2.goalPos - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                rotationSpeed * Time.deltaTime);
            speed = Random.Range(minSpeed, maxSpeed);
        }
        else
        {
            if (Random.Range(-1, 5) < 1)
                ApplyRules();
        }

        transform.Translate(0, 0, Time.deltaTime * speed*musicMultipier);
    }

    void ApplyBoundary()
    {
        if (Vector3.Distance(transform.position, GlobalFlock2.goalPos) >= GlobalFlock2.boundsSize)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = GlobalFlock2.allBugs;

        Vector3 vCenter = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = GlobalFlock2.goalPos;

        float dist;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighborDistance)
                {
                    vCenter += go.transform.position;
                    groupSize++;

                    if (dist < 0.75f)
                    {
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    bug2 anotherBug = go.GetComponent<bug2>();
                    gSpeed += anotherBug.speed;
                }

            }
        }

        if (groupSize > 0)
        {
            vCenter = vCenter / groupSize + (goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vCenter + vAvoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    rotationSpeed * Time.deltaTime);
            }
        }

    }
}