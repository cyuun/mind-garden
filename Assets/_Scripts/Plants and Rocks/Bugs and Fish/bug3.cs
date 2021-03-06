﻿using UnityEngine;
using System.Collections;

public class bug3 : MonoBehaviour
{
    public float musicMultipier = 2f;
    public float speed=0.1f;
    public float rotationSpeed = 4.0f;
    //public float turnSpeed = 4.0f;
    float minSpeed = 1.8f;
    float maxSpeed = 2.6f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighborDistance = 3.0f;

    public GlobalFlock3 _flock;

    bool turning = false;

    // Use this for initialization
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyBoundary();

        if (turning)
        {
            Vector3 direction = _flock.goalPos - transform.position;
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
        if (Vector3.Distance(transform.position, _flock.goalPos) >= _flock.boundsSize)
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
        gos = _flock.allBugs;

        Vector3 vCenter = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = _flock.goalPos;

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

                    bug3 anotherBug = go.GetComponent<bug3>();
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