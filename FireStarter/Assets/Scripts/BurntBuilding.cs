using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurntBuilding : MonoBehaviour
{
    public List<GameObject> FixLoc = new List<GameObject>();
    GameObject[] FixLocArray;
    GameObject[] Buildings;
    public GameObject Colosseum, Tower, Hotel, House, CircleTower, PointyHouse;
    public int BurntBuildingNum;
    public bool selected;

    private void Start()
    {
        Buildings = new GameObject[] { Colosseum, Tower, Hotel, House, CircleTower, PointyHouse };
    }

    public GameObject[] FixLocations()
    {
        Ray TestUp = new Ray(transform.position, transform.forward);
        RaycastHit TestUpHit;

        if (Physics.Raycast(TestUp, out TestUpHit))
        {
            if (TestUpHit.transform.CompareTag("Path"))
            {
                
                FixLoc.Add(TestUpHit.transform.gameObject);

            }

        }
        Ray TestDown = new Ray(transform.position, -transform.forward);
        RaycastHit TestDownHit;

        if (Physics.Raycast(TestDown, out TestDownHit))
        {
            if (TestDownHit.transform.CompareTag("Path"))
            {
                FixLoc.Add(TestDownHit.transform.gameObject);

            }

        }
        Ray TestRight = new Ray(transform.position, transform.right);
        RaycastHit TestRightHit;

        if (Physics.Raycast(TestRight, out TestRightHit))
        {
            if (TestRightHit.transform.CompareTag("Path"))
            {
                FixLoc.Add(TestRightHit.transform.gameObject);

            }

        }
        Ray TestLeft = new Ray(transform.position, -transform.right);
        RaycastHit TestLeftHit;

        if (Physics.Raycast(TestLeft, out TestLeftHit))
        {
            if (TestLeftHit.transform.CompareTag("Path"))
            {
                FixLoc.Add(TestLeftHit.transform.gameObject);

            }

        }
        FixLocArray = FixLoc.ToArray();
        return FixLocArray;
    }

    public void Rebuild()
    {
        //store which type of building was destroyed
        //reenable that building here
        Vector3 Spawn = this.transform.position + (this.transform.up * .5f);
        GameObject G = (GameObject)Instantiate(Buildings[BurntBuildingNum], Spawn, this.transform.rotation);
        Destroy(this.gameObject);
        //Debug.Log("Replace Building");
    }
}
