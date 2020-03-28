using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public static Dash S;
    
    public float distance = 5;
    public float speed = 100;
    public float cooldown = 2;

    private float _distanceCovered;
    private GameObject _camera;
    private bool _collided;
    private bool _canDash;

    private TerrainScript _terrainScript;

    public void SetTerrain(TerrainScript terrainScript)
    {
        _terrainScript = terrainScript;
        _canDash = true;
    }
    
    private void Start()
    {
        S = this;
        
        _distanceCovered = 0;
        _camera = transform.GetChild(0).gameObject;

        _canDash = false;
        _collided = false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            StartCoroutine(CooldownDash());
            StartCoroutine(UpdateDash(_camera.transform.forward));
        }
    }

    private IEnumerator UpdateDash(Vector3 direction)
    {
        _distanceCovered = 0;
        while (_distanceCovered < distance)
        {
            if (_collided)
            {
                yield break;
            }
            
            Vector3 origPos = transform.position;
            transform.position = origPos + speed * Time.deltaTime * direction;

            float terrainHeight = _terrainScript.GetTerrainHeight(transform.position.x, transform.position.z) + 0.1f;
            if (transform.position.y < terrainHeight)
            {
                transform.position = new Vector3(transform.position.x, terrainHeight + 0.1f, transform.position.z);
            }

            if (Mathf.Pow(transform.position.x, 2) / Mathf.Pow(_terrainScript.xMax, 2) +
                Mathf.Pow(transform.position.z, 2) / Mathf.Pow(_terrainScript.zMax, 2) > 0.09f)
            {
                transform.position = origPos;
                yield break;
            }

            _distanceCovered += Vector3.Magnitude(transform.position - origPos);

            yield return null;
        }
    }

    private IEnumerator CooldownDash()
    {
        _canDash = false;
        yield return new WaitForSeconds(cooldown);
        _canDash = true;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Terrain"))
        {
            _collided = true;
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Terrain"))
        {
            _collided = false;
        }
    }
}
