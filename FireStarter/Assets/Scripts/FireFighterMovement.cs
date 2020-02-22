using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FireFighterMovement : MonoBehaviour
{
    //
    //limit firefighter search range to generated area only
    //
    //dont let fire fighters pass through each other

    // float speed = 1;
    public float CheckRate = 1f;
    float Timer;
    public bool PathComplete = false;
    public bool Traveling = false;
    GameObject[] BurntBuildings;
    Transform TargetBuilding;
    public bool Building = false;
    public float BuildTime = 5f;

    [Space]

    public Transform currentCube;
    public Transform clickedCube;


    public List<Transform> finalPath = new List<Transform>();

    

    void Update()
    {
        Timer += Time.deltaTime;


        //somewhere in here
        //check raycast
        //if something is there assign target to a temporary variable
        //rebuild path using temp variable
        if (Timer >= CheckRate)
        {
            RaycastDown();
            //if not already going to path or building and if there are burnt buildings
           
            if (!Traveling && !Building && FindClosestBurntBuildingsTest() != null)
            {

                //Debug.Log("hit");
                PathComplete = false;
                RetrievePathOptions(FindClosestBurntBuildings());
            }
            
            Timer = 0;
        }




    }

    Transform FindClosestBurntBuildingsTest()
    {
        //build an array of burt buildings
        BurntBuildings = GameObject.FindGameObjectsWithTag("Burnt");
        Transform ClosestBuilding = null;
        //locate the size of that array
        //search the array for closest building
        if (BurntBuildings == null)
        {
            return null;
        }
        else
        {
            Vector3 TempDistance;
            float ShortestDistance = 1000f;

            for (int i = 0; i < BurntBuildings.Length; i++)
            {
                TempDistance = currentCube.position - BurntBuildings[i].transform.position;

                if (ShortestDistance > TempDistance.sqrMagnitude)
                {
                    if (!BurntBuildings[i].transform.GetComponent<BurntBuilding>().selected)
                    {
                        ClosestBuilding = BurntBuildings[i].transform;
                        ShortestDistance = TempDistance.sqrMagnitude;
                    }

                }
            }

            //build path to that building
            if (ClosestBuilding != null)
            {
                if (!ClosestBuilding.GetComponent<BurntBuilding>().selected)
                {
                    
                    //Debug.Log(ClosestBuilding);
                    return ClosestBuilding;

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

    }

    Transform FindClosestBurntBuildings()
    {
        //build an array of burt buildings
        BurntBuildings = GameObject.FindGameObjectsWithTag("Burnt");
        Transform ClosestBuilding = null;
        //locate the size of that array
        //search the array for closest building
        if (BurntBuildings == null)
        {
            return null;
        }
        else
        {
            Vector3 TempDistance;
            float ShortestDistance = 1000f;

            for (int i = 0; i < BurntBuildings.Length; i++)
            {
                TempDistance = currentCube.position - BurntBuildings[i].transform.position;

                if (ShortestDistance > TempDistance.sqrMagnitude)
                {
                    if (!BurntBuildings[i].transform.GetComponent<BurntBuilding>().selected)
                    {
                        ClosestBuilding = BurntBuildings[i].transform;
                        ShortestDistance = TempDistance.sqrMagnitude;
                    }
                    
                }
            }

            //build path to that building
            if(ClosestBuilding != null)
            {
                if (!ClosestBuilding.GetComponent<BurntBuilding>().selected)
                {
                    ClosestBuilding.GetComponent<BurntBuilding>().selected = true;
                    //Debug.Log(ClosestBuilding);
                    return ClosestBuilding;
                    
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
        
    }

    void RetrievePathOptions(Transform target)
    {
        TargetBuilding = target;
        
        
        GameObject[] pathOptions = target.GetComponent<BurntBuilding>().FixLocations();

        Vector3 TempDistance;
        float ShortestDistance = 1000f;

        for (int i = 0; i < pathOptions.Length; i++)
        {
            TempDistance = currentCube.position - pathOptions[i].transform.position;

            if (ShortestDistance > TempDistance.sqrMagnitude)
            {
                clickedCube = pathOptions[i].transform;
                ShortestDistance = TempDistance.sqrMagnitude;
            }
        }

        FindPath();
    }


    
    

    
    void FindPath()
    {
        
        //finds path options
        List<Transform> nextCubes = new List<Transform>();
        List<Transform> pastCubes = new List<Transform>();

        foreach (WalkPath path in currentCube.GetComponent<PathBuilder>().possiblePaths)
        {
            if (path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<PathBuilder>().previousBlock = currentCube;
            }
        }

        pastCubes.Add(currentCube);

        ExploreCube(nextCubes, pastCubes);
        BuildPath();
    }

    void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        //looks through all possible paths for best path
        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        if (current == clickedCube)
        {
            nextCubes.Clear();
            visitedCubes.Clear();
            return;
        }

        foreach (WalkPath path in current.GetComponent<PathBuilder>().possiblePaths)
        {
            if (!visitedCubes.Contains(path.target) && path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<PathBuilder>().previousBlock = current;
            }
        }

        visitedCubes.Add(current);

        if (nextCubes.Any())
        {
            ExploreCube(nextCubes, visitedCubes);
        }

    }

    void BuildPath()
    {
        //builds the route
        Transform cube = clickedCube;
        while (cube != currentCube)
        {
            finalPath.Add(cube);
            if (cube.GetComponent<PathBuilder>().previousBlock != null)
                cube = cube.GetComponent<PathBuilder>().previousBlock;
            else
                return;
        }

        finalPath.Insert(0, clickedCube);

        StartCoroutine(PursuitPath());

    }

    void Clear()
    {
        //clears all pathfinding and starts from scratch
        foreach (Transform t in finalPath)
        {
            t.GetComponent<PathBuilder>().previousBlock = null;
        }
        finalPath.Clear();
        currentCube = null;
        clickedCube = null;
        Traveling = false;
        StopAllCoroutines();
        PathComplete = true;
        


    }

    public void RaycastDown()
    {
        //retrieves current location of police
        Ray PositionRay = new Ray(transform.position, -transform.up);
        RaycastHit PositionHit;

        if (Physics.Raycast(PositionRay, out PositionHit))
        {
            if (PositionHit.transform.GetComponent<PathBuilder>() != null)
            {
                currentCube = PositionHit.transform;

            }

        }

    }

    bool RaycastCheck(int direction)
    {
        if (direction == 0)
        {
            Ray playerRay = new Ray(transform.position, transform.forward);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("FireFighter") || playerHit.transform.CompareTag("Police") || playerHit.transform.CompareTag("Player"))
                {
                    //deactivate current path of trigger
                    if (playerHit.transform.CompareTag("FireFighter"))
                    {
                        playerHit.transform.gameObject.GetComponent<FireFighterMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Police"))
                    {
                        playerHit.transform.gameObject.GetComponent<PoliceMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Player"))
                    {
                        playerHit.transform.gameObject.GetComponent<CharacterMovement>().RaycastDownDissable();
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        else if (direction == 1)
        {
            Ray playerRay = new Ray(transform.position, -transform.right);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("FireFighter") || playerHit.transform.CompareTag("Police") || playerHit.transform.CompareTag("Player"))
                {
                    //deactivate current path of trigger
                    if (playerHit.transform.CompareTag("FireFighter"))
                    {
                        playerHit.transform.gameObject.GetComponent<FireFighterMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Police"))
                    {
                        playerHit.transform.gameObject.GetComponent<PoliceMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Player"))
                    {
                        playerHit.transform.gameObject.GetComponent<CharacterMovement>().RaycastDownDissable();
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        else if (direction == 2)
        {
            Ray playerRay = new Ray(transform.position, -transform.forward);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("FireFighter") || playerHit.transform.CompareTag("Police") || playerHit.transform.CompareTag("Player"))
                {
                    //deactivate current path of trigger
                    if (playerHit.transform.CompareTag("FireFighter"))
                    {
                        playerHit.transform.gameObject.GetComponent<FireFighterMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Police"))
                    {
                        playerHit.transform.gameObject.GetComponent<PoliceMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Player"))
                    {
                        playerHit.transform.gameObject.GetComponent<CharacterMovement>().RaycastDownDissable();
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        else if (direction == 3)
        {
            Ray playerRay = new Ray(transform.position, transform.right);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("FireFighter") || playerHit.transform.CompareTag("Police") || playerHit.transform.CompareTag("Player"))
                {
                    //deactivate current path of trigger
                    if (playerHit.transform.CompareTag("FireFighter"))
                    {
                        playerHit.transform.gameObject.GetComponent<FireFighterMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Police"))
                    {
                        playerHit.transform.gameObject.GetComponent<PoliceMovement>().RaycastDownDissable();
                    }
                    else if (playerHit.transform.CompareTag("Player"))
                    {
                        playerHit.transform.gameObject.GetComponent<CharacterMovement>().RaycastDownDissable();
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    public void RaycastDownDissable()
    {
        Transform DissablePath = null;
        //retrieves current location of police
        Ray PositionRay = new Ray(transform.position, -transform.up);
        RaycastHit PositionHit;

        if (Physics.Raycast(PositionRay, out PositionHit))
        {
            if (PositionHit.transform.GetComponent<PathBuilder>() != null)
            {
                DissablePath = PositionHit.transform;

            }

        }
        DissablePath.GetComponent<PathBuilder>().TriggerTempDissable();

    }

   
    IEnumerator PursuitPath()
    {
        Traveling = true;
        while (!PathComplete)
        {
            

            for (int i = finalPath.Count - 1; i > 0; i--)
            {
                transform.position = finalPath[i].GetComponent<PathBuilder>().GetWalkPoint();

                //Debug.Log(i);
                yield return new WaitForSeconds(CheckRate);
            }
            Clear();
            StartCoroutine(Rebuild());
            PathComplete = true;
            
        }
    }
    IEnumerator Rebuild()
    {
        Building = true;
        yield return new WaitForSeconds(BuildTime);
        TargetBuilding.GetComponent<BurntBuilding>().Rebuild();
        Building = false;
    }


}

