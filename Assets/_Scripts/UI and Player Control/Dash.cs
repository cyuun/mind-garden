using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public static Dash S;
    
    public bool dashing = true;

    public float maxDistance = 5;
    public float speed = 100;

    private float _distanceCovered;
    private GameObject _camera;

    private TerrainScript _terrainScript;

    public void SetTerrain(TerrainScript terrainScript)
    {
        _terrainScript = terrainScript;
        dashing = false;
    }
    
    private void Start()
    {
        S = this;
        
        _distanceCovered = 0;
        _camera = transform.GetChild(0).gameObject;
    }
    
    private void Update()
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

            float terrainHeight = _terrainScript.GetTerrainHeight(transform.position.x, transform.position.z) + 0.1f;
            if (transform.position.y < terrainHeight)
            {
                transform.position = new Vector3(transform.position.x, terrainHeight + 0.1f, transform.position.z);
            } 

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
