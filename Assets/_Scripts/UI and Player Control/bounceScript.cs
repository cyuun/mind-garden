using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class bounceScript : MonoBehaviour
{
    Rigidbody rb;
    bool grounded = true;
    public GameObject target;
    public FirstPersonController controller;
    bool collided = false;


    public Transform startPos;
    public Transform endPos;
    public float journeyTime = 300f;
    public float speed=20f;

    float startTime;
    Vector3 centerPoint;
    Vector3 startRelCenter;
    Vector3 endRelCenter;
    public float speedTime = 3f;
    bool currSpeeding = false;

    PlayerScript player;

    void Start()
    {
        controller = GameObject.FindObjectOfType<FirstPersonController>();
        rb = GetComponent<Rigidbody>();
        player = PlayerScript.S;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.spawned)
        {
            float force = 15f;

            if (other.tag == "enemy" && grounded)
            {
                controller.canMove = false;
                ParticleSystem ps = other.gameObject.GetComponentInChildren<ParticleSystem>();
                ps.Play();
                //Vector3 direction = transform.position - other.transform.position+new Vector3(0,2f,0);
                Vector3 direction = (-transform.forward + transform.up);
                direction.Normalize();

                startPos = transform;
                endPos = target.transform;
                StartCoroutine(Push(direction, force));

            }
            else if (other.tag == "enemyFish")
            {
                controller.canMove = false;
                ParticleSystem ps = other.gameObject.GetComponentInChildren<ParticleSystem>();
                ps.Play();
                Vector3 direction = transform.position - other.transform.position + new Vector3(0, 5, 0);
                direction.Normalize();

                startPos = transform;
                endPos = target.transform;
                StartCoroutine(Push(direction, force));
            }
            else if (other.tag == "Speed")
            {
                //screen flash?
                if (!currSpeeding)
                {
                    StartCoroutine(speeding());
                }

            }
        }
    }
    public void GetCenter(Vector3 direct)
    {
        centerPoint = (startPos.position + endPos.position) * .5f;
        centerPoint -= direct;
        startRelCenter = startPos.position - centerPoint;
        endRelCenter = endPos.position - centerPoint;
    }

    IEnumerator Push(Vector3 direction, float force)
    {
        GameHUD.S.FlashColor("Red");

        float dz = -3;
        float dy = direction.y * force;
        Vector3 move = new Vector3(0, dy, dz);
        float timePassed = 0;

        while (!collided)
        {
            if (timePassed > 0.1f)
            {
                DetectCollisions();
            }

            if (collided)
            {
                //rb.isKinematic=true;
                collided = false;
                break;
            }

            move = new Vector3(0, dy, dz);
            transform.Translate(move * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.canMove = true;

                yield break;
            }

            timePassed += Time.deltaTime;
            dy -= .98f;
            yield return null;
        }
        controller.canMove = true;
        controller.stunned = true;

    }

    IEnumerator push(Vector3 direction, float force)
    {
        GameHUD.S.FlashColor("Red");

        GetCenter(Vector3.up);
        float timePassed = 0;
        //rb.isKinematic = false;
        startTime = Time.time;
        while (timePassed < 0.7)
        {
            if(timePassed > 0.4)
            {
                DetectCollisions();
            }

            if (collided)
            {
                //rb.isKinematic=true;
                collided = false;
                break;
            }

            float fracComplete = (Time.time - startTime) / journeyTime * speed;
            transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
            transform.position += centerPoint;
            if (fracComplete >= 1)
            {
                break;
            }

            //transform.Translate(direction * force * Time.deltaTime, Space.World);
            //rb.AddForce(direction * force * Time.deltaTime);
            timePassed += Time.deltaTime;

            yield return null;
        }
        //rb.isKinematic = true;
        controller.canMove = true;
        controller.stunned = true;

    }
    IEnumerator speeding()
    {
        GameHUD.S.FlashColor("Green");
        currSpeeding = true;
        controller.m_WalkSpeed *= 2f;
        controller.m_RunSpeed *= 2f;
        yield return new WaitForSeconds(speedTime);
        controller.m_WalkSpeed /= 2f;
        controller.m_RunSpeed /= 2f;
        currSpeeding = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag!="enemy" && collision.gameObject.tag!= "enemyFish")
        {
            Debug.Log("coll1");
            collided = true;
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "enemy" && collision.gameObject.tag != "enemyFish")
        {
            collided = false;
        }
    }

    void DetectCollisions()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 1);
        foreach(Collider col in cols)
        {
            if (col.gameObject.tag == "Terrain" || (col.gameObject.tag == "Trees" && !col.name.Contains("JunglePlant3")) || col.name.Contains("Sphere"))
            {
                collided = true;
                break;
            }
        }
    }
}
