using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    bool soundOn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            ToggleVolume();
        }
    }

    void ToggleVolume()
    {

    }
}
