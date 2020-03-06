using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boneFishMovement : MonoBehaviour
{
    Animator anim;
    //Collider col;
    int move = 1;
    float rotSpeed = 50f;
    float collSpeed = 80f;
    float speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        //col = GetComponent<Collider>();
        InvokeRepeating("movement", 0, 3.0f);

    }

    // Update is called once per frame
    void Update()
    {
        if (move <=3)
        {
            transform.Translate(-transform.right * Time.deltaTime, Space.World);
            anim.SetBool("goRight", false);
            anim.SetBool("goLeft", false);
        }
        else if (move == 4)
        {
            transform.Translate(-transform.right *speed* Time.deltaTime, Space.World);
            transform.Translate(-transform.forward * speed * Time.deltaTime, Space.Self);
            transform.Rotate(transform.up, -rotSpeed * Time.deltaTime, Space.Self);
            anim.SetBool("goRight", false);
            anim.SetBool("goLeft", true);
        }
        else if (move == 5)
        {
            transform.Translate(-transform.right * speed *Time.deltaTime, Space.World);
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.Self);
            transform.Rotate(transform.up, rotSpeed * Time.deltaTime, Space.Self);
            anim.SetBool("goRight", true);
            anim.SetBool("goLeft", false);
        }
        else if (move == 6)
        {
            transform.Translate(-transform.right * speed * Time.deltaTime, Space.World);
            transform.Translate(-transform.forward * speed * Time.deltaTime, Space.Self);
            transform.Rotate(transform.up, -collSpeed * Time.deltaTime, Space.Self);
            anim.SetBool("goRight", false);
            anim.SetBool("goLeft", true);
        }
        else if (move == 7)
        {
            transform.Translate(-transform.right * speed * Time.deltaTime, Space.World);
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.Self);
            transform.Rotate(transform.up, collSpeed * Time.deltaTime, Space.Self);
            anim.SetBool("goRight", true);
            anim.SetBool("goLeft", false);
        }
        else
        {
            Debug.Log("problems");
        }


    }
    void movement()
    {
        move=Random.Range(1, 6);
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collde");
        if(collision.gameObject.tag!= "Player")
        {
            move = Random.Range(6, 8);
        }
    }
}
