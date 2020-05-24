using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroCheck : MonoBehaviour
{
    public int ColNum;
    GameObject NewPathGen;
    bool isCol = true;

    //Leftmost and ccw

    private void Start()
    {
        NewPathGen = GameObject.FindGameObjectWithTag("Gen");
        Rescale();
    }
    private void Update()
    {
        isCol = false;
    }

    void Rescale()
    {
        this.transform.localScale = new Vector3(0.5f, 2, (float)NewPathGen.GetComponent<NewPathGeneration>().gridSize - 0.5f);

        if(this.transform.name == "MacroCheckE1")
        {
            this.transform.localPosition = new Vector3(-(int)(NewPathGen.GetComponent<NewPathGeneration>().gridSize / 2) -1 , 2, 0);
        }
        if (this.transform.name == "MacroCheckE2")
        {
            this.transform.localPosition = new Vector3(0, 2, -(int)(NewPathGen.GetComponent<NewPathGeneration>().gridSize / 2) - 1);
        }
        if (this.transform.name == "MacroCheckE3")
        {
            this.transform.localPosition = new Vector3((int)(NewPathGen.GetComponent<NewPathGeneration>().gridSize / 2) + 1, 2, 0);
        }
        if (this.transform.name == "MacroCheckE4")
        {
            this.transform.localPosition = new Vector3(0, 2, (int)(NewPathGen.GetComponent<NewPathGeneration>().gridSize / 2) + 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCol)
        {
            return;
        }
            
        isCol = true;
       
        if (other.CompareTag("Player"))
        {
           
            NewPathGen.GetComponent<NewPathGeneration>().CheckMacroLocation(ColNum, 
                transform.parent.transform.position.x, transform.parent.transform.position.z);
        }
    }

    
    
}
