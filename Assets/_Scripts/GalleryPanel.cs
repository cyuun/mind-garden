using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryPanel : MonoBehaviour
{
    public Image[] images;
    public Image[] backgrounds;
    public GameObject nextButton;

    void Start()
    {
        
    }

    public void nextScreen()
    {
        if (!MenuController.S._cameraMoving) StartCoroutine(MoveLeft());
    }

    public void previousScreen()
    {
        if (!MenuController.S._cameraMoving) StartCoroutine(MoveRight());
    }

    IEnumerator MoveLeft()
    {
        yield return null;
    }

    IEnumerator MoveRight()
    {
        yield return null;
    }
}
