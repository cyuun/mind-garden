using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public GameObject waterSplashPrefab;
    public static GameObject ring;

    [SerializeField]
    public static List<Transform> orbPositions = new List<Transform>();
    public static int orbsFound = 0;

    void Start()
    {
        Transform orbPositions = transform.Find("OrbPositions");
        if (orbPositions)
        {
            foreach(Transform child in orbPositions)
            {
                WaterScript.orbPositions.Add(child.transform);
            }
        }
        StartCoroutine(DelayMagic(1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Splash(float x, float z)
    {
        GameObject splash = Instantiate(waterSplashPrefab, new Vector3(x, 6, z), Quaternion.identity, this.transform);

    }
    IEnumerator DelayMagic(float duration)
    {
        yield return new WaitForSeconds(duration);
        ring = Instantiate(OrbScript.lightRing, transform.GetChild(0).position, Quaternion.identity, this.transform.GetChild(0));
        ring.transform.Find("rings").localScale *= 3;
        ring.transform.Find("Cube").GetComponent<BoxCollider>().size = new Vector3(.5f, 2, .5f);
        ring.SetActive(false);

    }


}
