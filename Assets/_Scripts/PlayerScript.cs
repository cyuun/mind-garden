using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript S;

    void Awake()
    {
        S = this;
    }

    // Update is called once per frame
    void Update()
    {
        //print(TerrainScript.S.GetTerrainHeight(transform.position.x, transform.position.z));
    }
}
