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
    public static Transform viewport;
    GameObject gallery;

    void Start()
    {
        gallery = transform.parent.gameObject;
        if(viewport == null) viewport = gallery.transform.parent.Find("Viewport");
        viewport.gameObject.SetActive(false);
    }

    public void nextScreen()
    {
        if (!MenuController.S._cameraMoving) StartCoroutine(MoveLeft());
    }

    public void previousScreen()
    {
        if (!MenuController.S._cameraMoving) StartCoroutine(MoveRight());
    }

    public void returnHome()
    {
        if (!MenuController.S._cameraMoving) StartCoroutine(MoveHome());
    }

    IEnumerator MoveHome()
    {
        RectTransform tr = gallery.GetComponent<RectTransform>();
        float startPos = tr.localPosition.x;
        float endPos = 0;
        float temp;
        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            temp = Mathf.SmoothStep(startPos, endPos, t);
            gallery.GetComponent<RectTransform>().transform.localPosition = new Vector3(temp, tr.localPosition.y, tr.localPosition.z);
            yield return null;
        }
        gallery.GetComponent<RectTransform>().transform.localPosition = new Vector3(endPos, tr.localPosition.y, tr.localPosition.z);
    }

    IEnumerator MoveLeft()
    {
        RectTransform tr = gallery.GetComponent<RectTransform>();
        float startPos = tr.localPosition.x;
        float endPos = startPos - tr.rect.width;
        float temp;
        for(float t = 0; t <= 1; t += Time.deltaTime)
        {
            temp = Mathf.SmoothStep(startPos, endPos, t);
            gallery.GetComponent<RectTransform>().transform.localPosition = new Vector3(temp, tr.localPosition.y, tr.localPosition.z);
            yield return null;
        }
        gallery.GetComponent<RectTransform>().transform.localPosition = new Vector3(endPos, tr.localPosition.y, tr.localPosition.z);
    }

    IEnumerator MoveRight()
    {
        RectTransform tr = gallery.GetComponent<RectTransform>();
        float startPos = tr.localPosition.x;
        float endPos = startPos + tr.rect.width;
        float temp;
        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            temp = Mathf.SmoothStep(startPos, endPos, t);
            gallery.GetComponent<RectTransform>().transform.localPosition = new Vector3(temp, tr.localPosition.y, tr.localPosition.z);
            yield return null;
        }
        gallery.GetComponent<RectTransform>().transform.localPosition = new Vector3(endPos, tr.localPosition.y, tr.localPosition.z);
    }

    public void DisplayImage(Image myImage)
    {
        viewport.gameObject.SetActive(true);
        Transform image = viewport.Find("Image");
        image.GetComponent<Image>().sprite = myImage.sprite;
    }

}
