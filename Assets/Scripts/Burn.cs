using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    bool burnt = false;
    Material mat;
    public int buildingNum;
    public GameObject Burnt;
    public ScoreData a;
   
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }


    public void BurnBuilding()
    {
        a.AddScore();
        Vector3 Spawn = this.transform.position - (this.transform.up * .5f);
        GameObject G = (GameObject)Instantiate(Burnt, Spawn, this.transform.rotation);
        G.GetComponent<BurntBuilding>().BurntBuildingNum = buildingNum;
        Destroy(this.gameObject);
        //Debug.Log("Replace Building");
    }

    //burnt building builds an array of transforms of closest available walkpaths that firefighters can pick from
    //replace building with burnt building
    //destroy building GO


}
