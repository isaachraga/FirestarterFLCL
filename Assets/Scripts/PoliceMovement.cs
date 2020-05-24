using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoliceMovement : MonoBehaviour
{
   // float speed = 1;
    public float CheckRate = 1f;
    float Timer;
    public bool PathComplete = false;
    public bool Patrolling;
    Transform RouteStop1, RouteStop2, RouteStop3, RouteStop4;
    Transform[] Route;
    GameObject[] PathsGO;
    Transform[] Paths;


    public bool pathfinding = false;

    [Space]

    public Transform currentCube;
    public Transform clickedCube;
   

    public List<Transform> finalPath = new List<Transform>();

    [SerializeField] GameObject gridManager;

    int exploreSafetyLimit = 60;
    int exploreSafetyLimiter = 0;

    //when police sees a firefighter, rebuild the path
    //keep destination


    private void Start()
    {
        gridManager = GameObject.FindGameObjectWithTag("Grid");
        exploreSafetyLimit = 30*30;
        BuildRoute();
    }

    void Update()
    {
        Timer += Time.deltaTime;
    
            if (Timer >= CheckRate)
            {
                
                if (Raycast(0) || Raycast(1) || Raycast(2) || Raycast(3))
                {
                    Debug.Log("Player Sighted.");
                    //police has seen player and builds path to last location
                    pathfinding = true;
                    PathComplete = false;
                    FindPath();
                    
                }
                else
                {
                    if (!pathfinding)
                    {
                    //police starts moving to a route location if they arent searching or dont currently see player
                        pathfinding = true;
                        PathComplete = false;
                        
                        PatrolRoute();
                    }

                }
                Timer = 0;
            }
        
        


    }

    Transform CheckThisSpace()
    {
        Transform space = null;
        Ray enemyRay = new Ray(transform.position, -transform.up);
        RaycastHit spaceHit;
        if (Physics.Raycast(enemyRay, out spaceHit))
        {
            if (spaceHit.transform.tag == "Path")
            {
                space = spaceHit.transform;
            }
        }
        return space;
    }


    /*I made Chancges to this function*/
    void BuildRoute()
    {
        int currentEnemyI = 0;
        int currentEnemyJ = 0;

        Route = new Transform[] { RouteStop1, RouteStop2, RouteStop3, RouteStop4 };
        int routeILimit = 0;
        int routeJLimit = 0;
        
        int gridSize = gridManager.GetComponent<NewPathGeneration>().gridSize;
        GameObject currentSpace = CheckThisSpace().gameObject;
        currentEnemyI = gridManager.GetComponent<PathGenerationScript>().GetEnemyCoordinates(currentSpace)[0];
        currentEnemyJ = gridManager.GetComponent<PathGenerationScript>().GetEnemyCoordinates(currentSpace)[1];

        int directionToPlayerI = (int)Mathf.Sign(GameObject.FindGameObjectWithTag("Player").transform.position.x - transform.position.x);
        int directionToPlayerJ = (int)Mathf.Sign(GameObject.FindGameObjectWithTag("Player").transform.position.z - transform.position.z);
        if (directionToPlayerI == 0)
        {
            directionToPlayerI = 1;
        }
        if (directionToPlayerJ == 0)
        {
            directionToPlayerJ = 1;
        }

        routeILimit = Random.Range(2, ((2*gridManager.GetComponent<PathGenerationScript>().GetVisionLimit()) - 2));
        routeJLimit = Random.Range(2, ((2*gridManager.GetComponent<PathGenerationScript>().GetVisionLimit()) - 2));

        Route[0] = gridManager.GetComponent<PathGenerationScript>().GetNearestPath(currentEnemyI + (directionToPlayerI * routeILimit), currentEnemyJ);
        Route[1] = gridManager.GetComponent<PathGenerationScript>().GetNearestPath(currentEnemyI + (directionToPlayerI * routeILimit), currentEnemyJ+(directionToPlayerJ*routeJLimit));
        Route[2] = gridManager.GetComponent<PathGenerationScript>().GetNearestPath(currentEnemyI, currentEnemyJ + (directionToPlayerJ * routeJLimit));
        Route[3] = gridManager.GetComponent<PathGenerationScript>().GetNearestPath(currentEnemyI, currentEnemyJ);
        Debug.Log("Route:0 ALTERNATIVE = " + currentEnemyI + " + ( " + directionToPlayerI + " * " + routeILimit + ") , " + currentEnemyJ);
        Debug.Log("Route:0 = " + Route[0].position.x + ", " + Route[0].position.z);
        Debug.Log("Route:1 = " + Route[1].position.x + ", " + Route[1].position.z);
        Debug.Log("Route:2 = " + Route[2].position.x + ", " + Route[2].position.z);
        Debug.Log("Route:3 = " + Route[3].position.x + ", " + Route[3].position.z);
        //
        //Limit this to area generated on police creation
        //
        /*PathsGO = GameObject.FindGameObjectsWithTag("Path");
        Paths = new Transform[PathsGO.Length];

        for (int i = 0; i < PathsGO.Length; ++i)
        {
            Paths[i] = PathsGO[i].transform;
        }

        for(int i = 0; i < Route.Length; i++)
        {
            int Rand = (int)Random.Range(0, Paths.Length);
            Route[i] = Paths[Rand];
            //Debug.Log(Route[i].transform.name);
        }*/
    }
    
    
    void PatrolRoute()
    {
        //tells police which route location to go to next
        RaycastDown();
        if (currentCube == Route[0])
        {
            clickedCube = Route[1];
            FindPath();
        }
        else if (currentCube == Route[1])
        {
            clickedCube = Route[2];
            FindPath();
        }
        else if (currentCube == Route[2])
        {
            clickedCube = Route[3];
            FindPath();
        }
        else if (currentCube == Route[3])
        {
            clickedCube = Route[0];
            FindPath();
        }
        else
        {
            //finds closest route location
            Vector3 TempDistance;
            float ShortestDistance = 1000f;

            for(int i = 0; i < Route.Length; i++)
            {
                TempDistance = currentCube.position - Route[i].position;

                if(ShortestDistance > TempDistance.sqrMagnitude)
                {
                    clickedCube = Route[i];
                    ShortestDistance = TempDistance.sqrMagnitude;
                }
            }
            FindPath();
        }
    }
   
    void FindPath()
    {
        for (int i = 0; i < 3; i++)
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
            exploreSafetyLimiter = 0;



            ExploreCube(nextCubes, pastCubes);
            if (exploreSafetyLimit < exploreSafetyLimiter)
            {
                break;
            }
        }
        BuildPath();
    }

    void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        Transform current;
        //looks through all possible paths for best path
        if (nextCubes.Any())
        {
            current = nextCubes.First();
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

            if (exploreSafetyLimiter >= exploreSafetyLimit)
            {
                nextCubes.Clear();
                visitedCubes.Clear();
                int routeIndex = System.Array.IndexOf(Route, clickedCube);
                routeIndex++;
                if (routeIndex >= Route.Length)
                {
                    routeIndex = 0;
                }
                clickedCube = Route[routeIndex];
                return;
            }

            if (nextCubes.Any() && exploreSafetyLimiter < exploreSafetyLimit)
            {
                exploreSafetyLimiter++;
                ExploreCube(nextCubes, visitedCubes);
            }
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
        pathfinding = false;
        StopAllCoroutines();
        PathComplete = true;
        

    }

    public void RaycastDown()
    {
        //retrieves current location of police
        Ray PoliceRay = new Ray(transform.position, -transform.up);
        RaycastHit PoliceHit;

        if (Physics.Raycast(PoliceRay, out PoliceHit))
        {
            if (PoliceHit.transform.GetComponent<PathBuilder>() != null)
            {
                currentCube = PoliceHit.transform;

            }

        }

    }

    bool Raycast(int direction)
    {
        //searches all possible directions for player
        //if seen, resets current path to build an updated version
         if (direction == 0)
         {
            
             Ray playerRay = new Ray(transform.position, transform.forward);
             RaycastHit playerHit;

             if (Physics.Raycast(playerRay, out playerHit))
             {
                 if (playerHit.transform.CompareTag("Player"))
                 {
                    Clear();
                    clickedCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
                    RaycastDown();
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             }
             if (Physics.Raycast(playerRay, out playerHit, .5f))
             {
                 if (playerHit.transform.CompareTag("Building"))
                 {
                     return false;
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
         else if (direction == 1)
         {
             Ray playerRay = new Ray(transform.position, -transform.right);
             RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit))
            {
                if (playerHit.transform.CompareTag("Player"))
                {
                    Clear();
                    clickedCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
                    RaycastDown();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building"))
                {
                    return false;
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
         else if (direction == 2)
         {
             Ray playerRay = new Ray(transform.position, -transform.forward);
             RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit))
            {
               
                if (playerHit.transform.CompareTag("Player"))
                {
                    Clear();
                    clickedCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
                    RaycastDown();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building"))
                {
                    return false;
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
         else if (direction == 3)
         {
             Ray playerRay = new Ray(transform.position, transform.right);
             RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit))
            {
                if (playerHit.transform.CompareTag("Player"))
                {
                    Clear();
                    clickedCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
                    RaycastDown();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building"))
                {
                    return false;
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
         else
         {
             return false;
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
        
        while (!PathComplete)
        {
            

            for (int i = finalPath.Count - 1; i > 0; i--)
            {
                transform.position = finalPath[i].GetComponent<PathBuilder>().GetWalkPoint();

                //Debug.Log(i);
                yield return new WaitForSeconds(CheckRate);
            }
            PathComplete = true;
            Clear();
            



        }
        

    }
    

}


