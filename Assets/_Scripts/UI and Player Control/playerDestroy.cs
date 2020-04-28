using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameObject player = GameObject.Find("Player");
        if(player!= null)
        Destroy(player);
    }


}
