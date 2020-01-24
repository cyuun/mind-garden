using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpenFileExplorer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explore()
    {
        //ShowExplorer(Application.persistentDataPath);
        string path = EditorUtility.OpenFilePanel("Select Song", Application.persistentDataPath, "mp3");
        if (path.Length != 0)
        {
            //read path

        }
    }

        private void ShowExplorer(string itemPath)
    {
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
        Debug.Log(Application.persistentDataPath);
    }

}
