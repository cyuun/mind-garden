using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public bool dashing = false;

    public float maxDistance = 5;
    public float speed = 100;

    private float _distanceCovered;
    private GameObject _camera;

    void Start()
    {
        _distanceCovered = 0;
        _camera = transform.GetChild(0).gameObject;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashing)
        {
            dashing = true;
            StartCoroutine(UpdateDash(_camera.transform.forward));
        }
    }

    private IEnumerator UpdateDash(Vector3 direction)
    {
        ResetDash();
        while (_distanceCovered < maxDistance)
        {
            Vector3 origPos = transform.position;
            transform.position = origPos + speed * Time.deltaTime * direction;

            _distanceCovered += Vector3.Magnitude(transform.position - origPos);

            yield return null;
        }

        dashing = false;
    }

    private void ResetDash()
    {
        _distanceCovered = 0;
    }
}
