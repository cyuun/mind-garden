using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float _speed;
    public Vector3 _axis;
    public bool _rotating;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_rotating) transform.Rotate(_axis, (_speed * Time.deltaTime % 360));
    }
}
