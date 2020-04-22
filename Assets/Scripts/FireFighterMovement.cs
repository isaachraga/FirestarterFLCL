using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FireFighterMovement : MonoBehaviour
{
    
    public float CheckRate = 1f;
    float Timer;
    public bool PathComplete = false;
    public bool Traveling = false;
    int TempDirection;
    GameObject[] BurntBuildings;
    Transform TargetBuilding;
    public bool Building = false;
    public float BuildTime = 5f;
    GameObject Player;
    GameObject Police;
    public bool tempDissable;
    Transform spawnLocation;
    Vector3 TempLeash;
    float LeashDistance = 200f;
    public float LighterDenominator;
    public float FirebombDenominator;
    public GameObject LighterDrop;
    public GameObject FireBombDrop;

    [Space]

    public Transform currentCube;
    public Transform targetCube;


    public List<Transform> finalPath = new List<Transform>();

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        GetSpawnLocation();
    }


    void Update()
    {
        Timer += Time.deltaTime;


        
        if (Timer >= CheckRate)
        {
            //gets current location
            RaycastDown();
            
            if (!tempDissable)
            {
                //if not already going to path or building 
                if (!Traveling && !Building )
                {
                   //and if there are burnt buildings   
                   if (FindClosestBurntBuildingsTest() != null)
                   {
                        //find a new burnt building to fix
                        PathComplete = false;
                        RetrievePathOptions(FindClosestBurntBuilding());
                   } 
                }
               
            }
            
            
            Timer = 0;
        }
        



    }

    //not necessary if fire fighters are spaced out more than their leash
    /*void FFRebuildRoute(Transform TempTargetCube)
    {
        Debug.Log("Hit FF");
        if (targetCube != null)
        {

            Clear();
            targetCube = TempTargetCube;
            
            PathComplete = false;
            RaycastDown();
            FindPath();
            Debug.Log(targetCube.name);


        }
    }*/

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

            //spawn location start of leash
            //store a max distance away from that

            for (int i = 0; i < BurntBuildings.Length; i++)
            {
                TempDistance = currentCube.position - BurntBuildings[i].transform.position;
                TempLeash = spawnLocation.position - BurntBuildings[i].transform.position;
                if (TempDistance.sqrMagnitude < ShortestDistance && TempLeash.sqrMagnitude < LeashDistance)
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

    Transform FindClosestBurntBuilding()
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
                TempLeash = spawnLocation.position - BurntBuildings[i].transform.position;
                

                if (ShortestDistance > TempDistance.sqrMagnitude && TempLeash.sqrMagnitude < LeashDistance)
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
                targetCube = pathOptions[i].transform;
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

        if (current == targetCube)
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
        Transform cube = targetCube;
        while (cube != currentCube)
        {
            finalPath.Add(cube);
            if (cube.GetComponent<PathBuilder>().previousBlock != null)
                cube = cube.GetComponent<PathBuilder>().previousBlock;
            else
                return;
        }

        finalPath.Insert(0, targetCube);

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
        targetCube = null;
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

    void GetSpawnLocation()
    {
        Ray PositionRay = new Ray(transform.position, -transform.up);
        RaycastHit PositionHit;

        if (Physics.Raycast(PositionRay, out PositionHit))
        {
            if (PositionHit.transform.GetComponent<PathBuilder>() != null)
            {
                spawnLocation = PositionHit.transform;

            }

        }
    }

    public void RaycastDownDissable()
    {
        tempDissable = true;
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

    bool PlayerPushCheck(int direction)
    {
        if (direction == 0)
        {
            Ray FrontRay = new Ray(transform.position, transform.forward);
            RaycastHit FrontHit;
            if (Physics.Raycast(FrontRay, out FrontHit, .5f))
            {
                if (FrontHit.transform.CompareTag("Player"))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        if (direction == 1)
        {
            Ray BackRay = new Ray(transform.position, -transform.forward);
            RaycastHit BackHit;
            if (Physics.Raycast(BackRay, out BackHit, .5f))
            {
                if (BackHit.transform.CompareTag("Player"))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        if (direction == 2)
        {
            Ray RightRay = new Ray(transform.position, transform.right);
            RaycastHit RightHit;
            if (Physics.Raycast(RightRay, out RightHit, .5f))
            {
                if (RightHit.transform.CompareTag("Player"))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        if (direction == 3)
        {

            Ray LeftRay = new Ray(transform.position, -transform.right);
            RaycastHit LeftHit;

            if (Physics.Raycast(LeftRay, out LeftHit, .5f))
            {

                if (LeftHit.transform.CompareTag("Player"))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        else
            return false;
    }

    

    void FullPlayerCheck(int i)
    {
        if (PlayerPushCheck(0))
        {
            
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(0) != null)
            {
                Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(0).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 1)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
                

            }
        }
        else if (PlayerPushCheck(1))
        {
            
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(1) != null)
            {
                Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(1).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 0)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
            }
        }
        else if (PlayerPushCheck(2))
        {
            
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(2) != null)
            {
                Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(2).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 3)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
            }

        }
        else if (PlayerPushCheck(3))
        {
            
            
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(3) != null)
            {
                Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(3).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                
               
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 2)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Player.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
            }
        }
    }


    bool PolicePushCheck(int direction)
    {
        if (direction == 0)
        {
            Ray FrontRay = new Ray(transform.position, transform.forward);
            RaycastHit FrontHit;
            if (Physics.Raycast(FrontRay, out FrontHit, .5f))
            {
                if (FrontHit.transform.CompareTag("Police"))
                {
                    Police = FrontHit.transform.gameObject;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        if (direction == 1)
        {
            Ray BackRay = new Ray(transform.position, -transform.forward);
            RaycastHit BackHit;
            if (Physics.Raycast(BackRay, out BackHit, .5f))
            {
                if (BackHit.transform.CompareTag("Police"))
                {
                    Police = BackHit.transform.gameObject;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        if (direction == 2)
        {
            Ray RightRay = new Ray(transform.position, transform.right);
            RaycastHit RightHit;
            if (Physics.Raycast(RightRay, out RightHit, .5f))
            {
                if (RightHit.transform.CompareTag("Police"))
                {
                    Police = RightHit.transform.gameObject;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        if (direction == 3)
        {

            Ray LeftRay = new Ray(transform.position, -transform.right);
            RaycastHit LeftHit;

            if (Physics.Raycast(LeftRay, out LeftHit, .5f))
            {

                if (LeftHit.transform.CompareTag("Police"))
                {
                    Police = LeftHit.transform.gameObject;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        else
            return false;
    }

    void FullPoliceCheck(int i)
    {
        if (PolicePushCheck(0))
        {
            Debug.Log("pHit 0 ");
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(0) != null)
            {
                Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(0).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 1)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
                Debug.Log("pHit else");

            }
        }
        else if (PolicePushCheck(0))
        {
            Debug.Log("pHit 1 ");
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(1) != null)
            {
                Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(1).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                Debug.Log("pHit else");
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 0)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
            }
        }
        else if (PolicePushCheck(0))
        {
            Debug.Log("pHit 2 ");
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(2) != null)
            {
                Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(2).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                Debug.Log("pHit else");
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 3)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
            }

        }
        else if (PolicePushCheck(0))
        {
            Debug.Log("pHit 3 ");
            Debug.Log(finalPath[i]);
            if (finalPath[i].GetComponent<PathBuilder>().LocatePush(3) != null)
            {
                Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(3).GetComponent<PathBuilder>().GetWalkPoint();
            }
            else
            {
                Debug.Log("pHit else");
                //Player.transform.position = finalPath[i].GetComponent<PathBuilder>().possiblePaths.First().target.transform.GetComponent<PathBuilder>().GetWalkPoint();
                for (int j = 0; j < finalPath.Count; j++)
                {
                    if (j != 2)
                    {
                        if (finalPath[i].GetComponent<PathBuilder>().LocatePush(j) != null)
                        {
                            Police.transform.position = finalPath[i].GetComponent<PathBuilder>().LocatePush(j).GetComponent<PathBuilder>().GetWalkPoint();
                        }
                    }
                }
            }
        }
    }

    void PowerupDrop()
    {
        int TempLighterNum = (int)Random.Range(1f, LighterDenominator);
        int TempFireBombNum = (int)Random.Range(1f, FirebombDenominator);
        if(TempLighterNum == 3)
        {
            GameObject g = (GameObject)Instantiate(LighterDrop, transform.position, transform.rotation);
        }
        else if(TempFireBombNum == 3)
        {
            GameObject g = (GameObject)Instantiate(FireBombDrop, transform.position, transform.rotation);
        }
    }


    IEnumerator PursuitPath()
    {
        Traveling = true;
        while (!PathComplete)
        {
            

            for (int i = finalPath.Count - 1; i > 0; i--)
            {
                
                if(finalPath[i].GetComponent<PathBuilder>().CheckForPlayer() == true)
                {

                    FullPlayerCheck(i);
                }

                //not necessary if fire fighters are spaced out more than their leash
                /*if (finalPath[i].GetComponent<PathBuilder>().CheckForFireFighter() == true)
                {
                    finalPath[i].GetComponent<PathBuilder>().SendFireFighter().GetComponent<FireFighterMovement>().RaycastDownDissable();
                    FFRebuildRoute(targetCube);
                    
                    
                    

                }
                else
                {
                    transform.position = finalPath[i].GetComponent<PathBuilder>().GetWalkPoint();
                }*/

                //if using ff avoidance, delete this 
                transform.position = finalPath[i].GetComponent<PathBuilder>().GetWalkPoint();




                yield return new WaitForSeconds(CheckRate);
            }
            Clear();
            StartCoroutine(Rebuild());
            PathComplete = true;
            StartCoroutine(DropPowerUp());
           

        }
    }
    IEnumerator Rebuild()
    {
        Building = true;
        yield return new WaitForSeconds(BuildTime);
        TargetBuilding.GetComponent<BurntBuilding>().Rebuild();
        Building = false;
    }
    IEnumerator DropPowerUp()
    {
        yield return new WaitForSeconds(5f);
        PowerupDrop();
        
    }


}

