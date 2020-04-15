using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebuildPaths : MonoBehaviour
{
    GameObject[] Paths;
    // Start is called before the first frame update
    void Start()
    {
        Paths = GameObject.FindGameObjectsWithTag("Path");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < Paths.Length; i++)
            {
                Paths[i].GetComponent<PathBuilder>().TriggerTempDissable();
            }
        }
    }
    //clear current paths and build new ones
}
