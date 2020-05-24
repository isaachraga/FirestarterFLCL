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
        //Debug.Log(6);
        //Dissable for generation
         LocatePaths();
    }
    



    public Transform LocatePush(int direction)
    {
        if(direction == 0)
        {
            Ray TestUp = new Ray(transform.position, transform.forward);
            RaycastHit TestUpHit;

            if (Physics.Raycast(TestUp, out TestUpHit, .5f))
            {
                if (TestUpHit.transform.CompareTag("Path"))
                {
                    return TestUpHit.transform;
                }
                else
                    return null;

            }
            else
                return null;
        }
        if(direction == 1)
        {
            Ray TestDown = new Ray(transform.position, -transform.forward);
            RaycastHit TestDownHit;

            if (Physics.Raycast(TestDown, out TestDownHit, .5f))
            {
                if (TestDownHit.transform.CompareTag("Path"))
                {
                    return TestDownHit.transform;
                }
                else
                    return null;

            }
            else
                return null;
        }
        if(direction == 2)
        {
            
            Ray TestRight = new Ray(transform.position, transform.right);
            RaycastHit TestRightHit;

            if (Physics.Raycast(TestRight, out TestRightHit, .5f))
            {
                if (TestRightHit.transform.CompareTag("Path"))
                {
                    return TestRightHit.transform;
                }
                else
                    return null;

            }
            else
                return null;
        }
        if(direction == 3)
        {
            
            Ray TestLeft = new Ray(transform.position, -transform.right);
            RaycastHit TestLeftHit;

            if (Physics.Raycast(TestLeft, out TestLeftHit, .5f))
            {
                if (TestLeftHit.transform.CompareTag("Path"))
                {
                    return TestLeftHit.transform;
                }
                else
                    return null;

            }
            else
                return null;
        }
        else
        {
            return null;
        }
        
        

    }

    public void PathCheck(Transform T)
    {
        //Debug.Log(1);
        WalkPath WP = new WalkPath();
        WP.target = T;

        WalkPath[] WalkCheck = possiblePaths.ToArray();
        for(int i = 0; i < WalkCheck.Length; i++)
        {
            if(WalkCheck[i].target == WP.target)
            {
                //Debug.Log(10);
                break;
            }
            if(i == WalkCheck.Length - 1)
            {
                possiblePaths.Add(WP);
            }
        }
        
            
    }

    private void OnDestroy()
    {
        /*foreach (WalkPath wp in possiblePaths)
        {
            if (wp.target.GetComponent<PathBuilder>().possiblePaths != null)
            {
                foreach (WalkPath wp2 in wp.target.GetComponent<PathBuilder>().possiblePaths)
                {
                    if (wp2.target == this.transform)
                    {
                        wp.target.GetComponent<PathBuilder>().possiblePaths.Remove(wp2);

                    }
                }
            }


        }*/
    }




    public void LocatePaths()
    {
        
        possiblePaths.Clear();
        
        //Shoots rays in 4 directions to find possible paths
        //adds positive hits to the walkable list

        Ray TestUp = new Ray(transform.position, transform.forward);
        RaycastHit TestUpHit;

        if (Physics.Raycast(TestUp, out TestUpHit, .5f))
        {
            if (TestUpHit.transform.CompareTag("Path"))
            {
                //Debug.Log(1);
                WalkPath WP = new WalkPath();
                WP.target = TestUpHit.transform; 
                possiblePaths.Add(WP);
                TestUpHit.transform.GetComponent<PathBuilder>().PathCheck(this.transform);
                

            }

        }
        Ray TestDown = new Ray(transform.position, -transform.forward);
        RaycastHit TestDownHit;

        if (Physics.Raycast(TestDown, out TestDownHit, .5f))
        {
            if (TestDownHit.transform.CompareTag("Path"))
            {
                
                WalkPath WP = new WalkPath();
                WP.target = TestDownHit.transform; 
                possiblePaths.Add(WP);
                TestDownHit.transform.GetComponent<PathBuilder>().PathCheck(this.transform);

            }

        }
        Ray TestRight = new Ray(transform.position, transform.right);
        RaycastHit TestRightHit;

        if (Physics.Raycast(TestRight, out TestRightHit, .5f))
        {
            if (TestRightHit.transform.CompareTag("Path"))
            {
                
                WalkPath WP = new WalkPath();
                WP.target = TestRightHit.transform;
                possiblePaths.Add(WP);
                TestRightHit.transform.GetComponent<PathBuilder>().PathCheck(this.transform);

            }

        }
        Ray TestLeft = new Ray(transform.position, -transform.right);
        RaycastHit TestLeftHit;

        if (Physics.Raycast(TestLeft, out TestLeftHit, .5f))
        {
            if (TestLeftHit.transform.CompareTag("Path"))
            {
                
                WalkPath WP = new WalkPath();
                WP.target = TestLeftHit.transform;
                possiblePaths.Add(WP);
                TestLeftHit.transform.GetComponent<PathBuilder>().PathCheck(this.transform);

            }

        }

    }

    public bool CheckForPlayer()
    {
        Ray pCheck = new Ray(transform.position, transform.up);
        RaycastHit pCheckHit;

        if (Physics.Raycast(pCheck, out pCheckHit, .5f))
        {
            if (pCheckHit.transform.CompareTag("Player"))
            {
                
                return true;

            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }

    }
    public bool CheckForPolice()
    {
        Ray pCheck = new Ray(transform.position, transform.up);
        RaycastHit pCheckHit;

        if (Physics.Raycast(pCheck, out pCheckHit, .5f))
        {
            if (pCheckHit.transform.CompareTag("Police"))
            {

                return true;

            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }
    public bool CheckForFireFighter()
    {
        
        Ray pCheck = new Ray(transform.position, transform.up);
        RaycastHit pCheckHit;

        if (Physics.Raycast(pCheck, out pCheckHit, .5f))
        {
            if (pCheckHit.transform.CompareTag("FireFighter"))
            {

                return true;

            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }

    public Transform SendFireFighter()
    {
        Ray pCheck = new Ray(transform.position, transform.up);
        RaycastHit pCheckHit;

        if (Physics.Raycast(pCheck, out pCheckHit, .5f))
        {
            if (pCheckHit.transform.CompareTag("FireFighter"))
            {

                return pCheckHit.transform;

            }
            else
            {
                return null;
            }

        }
        else
        {
            return null;
        }
    }


    public Vector3 GetWalkPoint()
    {
        return transform.position + transform.up;
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.gray;

        Gizmos.DrawSphere(GetWalkPoint(), .1f);

        if (possiblePaths == null)
            return;

        foreach (WalkPath p in possiblePaths)
        {
            if (p.target == null)
                return;
            Gizmos.color = p.active ? Color.black : Color.clear;
            Gizmos.DrawLine(GetWalkPoint(), p.target.GetComponent<PathBuilder>().GetWalkPoint());
        }*/
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

    public void PathDissable()
    {
        foreach (WalkPath wp in possiblePaths)
        {
            wp.active = false;
            foreach (WalkPath wp2 in wp.target.GetComponent<PathBuilder>().possiblePaths)
            {
                if (wp2.target == this.transform)
                {
                    wp2.active = false;
                }
            }

        }
    }

    public void PathEnable()
    {
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

    private void DisableTemp()
    {
        Ray fCheck = new Ray(transform.position, transform.up);
        RaycastHit fCheckHit;

        if (Physics.Raycast(fCheck, out fCheckHit, .5f))
        {
            if (fCheckHit.transform.CompareTag("FireFighter"))
            {
                fCheckHit.transform.gameObject.GetComponent<FireFighterMovement>().tempDissable = false;
            }
            
        }
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
        DisableTemp();
    }
}


[System.Serializable]
public class WalkPath 
{
    public Transform target;
    public bool active = true;
}


