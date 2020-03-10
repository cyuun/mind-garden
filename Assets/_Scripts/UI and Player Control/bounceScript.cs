using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class bounceScript : MonoBehaviour
{
    Rigidbody rb;
    public FirstPersonController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindObjectOfType<FirstPersonController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision c)
    {
        float force = 30;
        Debug.Log("coll");

        if (c.gameObject.tag == "enemy")
        {
            // Calculate Angle Between the collision point and the player
            Vector3 dir = c.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            //GetComponent<Rigidbody>().AddForce(dir * force);
            transform.position += dir * Time.deltaTime * force;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        float force = 7f;

        if (other.tag == "enemy")
        {
            controller.canMove = false;
            ParticleSystem ps=other.gameObject.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            //transform.position -= transform.forward * Time.deltaTime * force;
            Vector3 direction = transform.position - other.transform.position;
            direction.Normalize();
            StartCoroutine(push(direction, force));
            //rb.AddForce(direction*force);
        }
        else if (other.tag == "enemyFish")
        {
            controller.canMove = false;
            ParticleSystem ps = other.gameObject.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Vector3 direction = transform.up - transform.forward;
            direction.Normalize();
            print(direction);
            StartCoroutine(pushFish(direction, force));
        }
    }
    IEnumerator push(Vector3 direction, float force)
    {


        float timePassed = 0;
        while (timePassed < 0.7)
        {
            transform.Translate(direction * force * Time.deltaTime, Space.World);
            timePassed += Time.deltaTime;

            yield return null;
        }
        controller.canMove = true;
    }

    IEnumerator pushFish(Vector3 direction, float force)
    {


        float timePassed = 0;
        while (timePassed < 0.7)
        {
            transform.Translate(direction * force * Time.deltaTime, Space.Self);
            timePassed += Time.deltaTime;

            yield return null;
        }
        controller.canMove = true;
    }
}
