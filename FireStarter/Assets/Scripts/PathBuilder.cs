using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{
    public List<WalkPath> possiblePaths = new List<WalkPath>();
    

    [Space]

    public Transform previousBlock;

    [Space]



    [Space]

    [Header("Offsets")]
    public float walkPointOffset = .5f;
    public float WaitTime = 1f;


    private void Start()
    {
        LocatePaths();
    }
    

    public void LocatePaths()
    {
        //Shoots rays in 4 directions to find possible paths
        //adds positive hits to the walkable list

        Ray TestUp = new Ray(transform.position, transform.forward);
        RaycastHit TestUpHit;

        if (Physics.Raycast(TestUp, out TestUpHit))
        {
            if (TestUpHit.transform.CompareTag("Path"))
            {
                WalkPath WP = new WalkPath();
                WP.target = TestUpHit.transform; 
                possiblePaths.Add(WP);

            }

        }
        Ray TestDown = new Ray(transform.position, -transform.forward);
        RaycastHit TestDownHit;

        if (Physics.Raycast(TestDown, out TestDownHit))
        {
            if (TestDownHit.transform.CompareTag("Path"))
            {
                WalkPath WP = new WalkPath();
                WP.target = TestDownHit.transform; 
                possiblePaths.Add(WP);

            }

        }
        Ray TestRight = new Ray(transform.position, transform.right);
        RaycastHit TestRightHit;

        if (Physics.Raycast(TestRight, out TestRightHit))
        {
            if (TestRightHit.transform.CompareTag("Path"))
            {
                WalkPath WP = new WalkPath();
                WP.target = TestRightHit.transform;
                possiblePaths.Add(WP);

            }

        }
        Ray TestLeft = new Ray(transform.position, -transform.right);
        RaycastHit TestLeftHit;

        if (Physics.Raycast(TestLeft, out TestLeftHit))
        {
            if (TestLeftHit.transform.CompareTag("Path"))
            {
                WalkPath WP = new WalkPath();
                WP.target = TestLeftHit.transform;
                possiblePaths.Add(WP);

            }

        }

    }


    public Vector3 GetWalkPoint()
    {
        return transform.position + transform.up;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        Gizmos.DrawSphere(GetWalkPoint(), .1f);

        if (possiblePaths == null)
            return;

        foreach (WalkPath p in possiblePaths)
        {
            if (p.target == null)
                return;
            Gizmos.color = p.active ? Color.black : Color.clear;
            Gizmos.DrawLine(GetWalkPoint(), p.target.GetComponent<PathBuilder>().GetWalkPoint());
        }
    }

    public void AttachNewPaths()
    {
        //delay if someone is on the tile
        possiblePaths.Clear();
        LocatePaths();
    }

    public void TriggerTempDissable()
    {
        StartCoroutine(TempDissable());
    }

    IEnumerator TempDissable()
    {
        foreach(WalkPath wp in possiblePaths)
        {
            wp.active = false;
            foreach(WalkPath wp2 in wp.target.GetComponent<PathBuilder>().possiblePaths)
            {
                if(wp2.target == this.transform)
                {
                    wp2.active = false;
                }
            }
            
        }
        
        yield return new WaitForSeconds(WaitTime);

        foreach (WalkPath wp in possiblePaths)
        {
            wp.active = true;
            foreach (WalkPath wp2 in wp.target.GetComponent<PathBuilder>().possiblePaths)
            {
                if (wp2.target == this.transform)
                {
                    wp2.active = true;
                }
            }

        }
    }
}


[System.Serializable]
public class WalkPath 
{
    public Transform target;
    public bool active = true;
}


