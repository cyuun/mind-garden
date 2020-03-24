using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongLibrary : MonoBehaviour
{
    public static SongLibrary S;
    public static bool libraryCreated = false;

    public GameObject listObject;

    private void Awake()
    {
        if (S == null) S = this;
    }

    void Start()
    {
        if (!libraryCreated)
        {
            gameObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            S.gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
