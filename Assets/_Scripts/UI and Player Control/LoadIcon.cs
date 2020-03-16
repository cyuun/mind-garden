using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadIcon : MonoBehaviour
{
    public RectTransform _icon;
    public float _timeStep;
    public float _rotateAngle;

    float _startTime;

    void Start()
    {
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - _startTime >= _timeStep)
        {
            Vector3 iconRot = _icon.localEulerAngles;
            iconRot.z += _rotateAngle;
            _icon.localEulerAngles = iconRot;

            _startTime = Time.time;
        }
    }
}
