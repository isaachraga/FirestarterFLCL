using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewPoliceMvmt : MonoBehaviour
{
    /*
     *Temporarily Works
     * 
     * Need To get buildings in place to limit the amount of paths that police has to iterate through
     * 
     * 
     */
    

    public float CheckRate = 1f;
    float Timer;
    public bool PathComplete = false;
    public bool Patrolling;
    Transform RouteStop1, RouteStop2, RouteStop3, RouteStop4;
    public Transform[] Route;
    GameObject[] PathsGO;
    Transform[] Paths;
    int Check = 0;
    public GameObject EnableBlock;

    [SerializeField] private LayerMask mask;


    public bool pathfinding = false;

    [Space]

    public Transform currentCube;
    public Transform clickedCube;
    public Transform targetCube;


    public List<Transform> finalPath = new List<Transform>();


    private void Start()
    {
        StartCoroutine(Spawn());
        //BuildRoute();
        
    }

    void CheckForPlayer()
    {
        StartCoroutine(Spawn());
    }

    void Update()
    {
        Timer += Time.deltaTime;

       /* if (Timer >= CheckRate)
        {

            if (Raycast(0) || Raycast(1) || Raycast(2) || Raycast(3))
            {
                Debug.Log("hit palyer");
                //police has seen player and builds path to last location
                if (FireRaycast(0) || FireRaycast(1) || FireRaycast(2) || FireRaycast(3))
                {
                    //if police see a firefighter, rebuild route to avoid running into them
                    FFRebuildChase();
                }
                else
                {
                    pathfinding = true;
                    PathComplete = false;
                    FindPath();
                }
            }
            else
            {

                if (FireRaycast(0) || FireRaycast(1) || FireRaycast(2) || FireRaycast(3))
                {
                    //if police see a firefighter, rebuild route to avoid running into them
                    FFRebuildRoute();
                }
                if (!pathfinding)
                {
                    //police starts moving to a route location if they arent searching or dont currently see player
                    pathfinding = true;
                    PathComplete = false;
                    PatrolRoute(null);
                }
            }
            Timer = 0;
        }*/
    }

    void FFRebuildChase()
    {
        if (targetCube != null)
        {
            Transform TempTargetCube = targetCube;
            Clear();
            targetCube = TempTargetCube;
            pathfinding = true;
            PathComplete = false;
            RaycastDown();
            FindPath();


        }
    }
    void FFRebuildRoute()
    {
        if (targetCube != null)
        {
            Transform TempTargetCube = targetCube;
            Clear();
            targetCube = TempTargetCube;
            pathfinding = true;
            PathComplete = false;

            PatrolRoute(targetCube);


        }
    }

    bool FireRaycast(int direction)
    {

        if (direction == 0)
        {

            Ray playerRay = new Ray(transform.position, transform.forward);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit))
            {
                if (playerHit.transform.CompareTag("FireFighter"))
                {

                    playerHit.transform.GetComponent<FireFighterMovement>().RaycastDownDissable();
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
        else if (direction == 1)
        {
            Ray playerRay = new Ray(transform.position, -transform.right);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit))
            {
                if (playerHit.transform.CompareTag("FireFighter"))
                {

                    playerHit.transform.GetComponent<FireFighterMovement>().RaycastDownDissable();
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
        else if (direction == 2)
        {
            Ray playerRay = new Ray(transform.position, -transform.forward);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit))
            {
                if (playerHit.transform.CompareTag("FireFighter"))
                {

                    playerHit.transform.GetComponent<FireFighterMovement>().RaycastDownDissable();
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
        else if (direction == 3)
        {
            Ray playerRay = new Ray(transform.position, transform.right);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit))
            {
                if (playerHit.transform.CompareTag("FireFighter"))
                {

                    playerHit.transform.GetComponent<FireFighterMovement>().RaycastDownDissable();
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
        else
        {
            return false;
        }
    }

    void BuildRoute()
    {

        //first route point is where they spawn
        //following route points should be within 20 blocks of
        Route = new Transform[] { RouteStop1, RouteStop2, RouteStop3, RouteStop4 };

        RaycastDown();
        RouteStop1 = currentCube;
        Route[0] = RouteStop1;

        //
        //might be too intensive for full game, be aware of this array
        //
        PathsGO = GameObject.FindGameObjectsWithTag("Path");
        Paths = new Transform[PathsGO.Length];

        for (int i = 0; i < PathsGO.Length; ++i)
        {
            Paths[i] = PathsGO[i].transform;
        }

        for (int i = 1; i < Route.Length; i++)
        {

            bool found = false;
            while (!found)
            {
                int Rand = (int)Random.Range(0, Paths.Length);
                //find distance between starting point and this route point
                //if less then 20 found is true
                float tempMag;

                tempMag = Paths[Rand].position.magnitude - currentCube.position.magnitude;
                //Debug.Log(tempMag + " " + Paths[Rand].position.magnitude + " " + currentCube.position.magnitude);
                if (tempMag < 50f)
                {
                    Route[i] = Paths[Rand];
                    found = true;
                }

            }



        }
    }


    void PatrolRoute(Transform TargetCube)
    {
        //tells police which route location to go to next
        RaycastDown();
        if (TargetCube == null)
        {

            if (currentCube == Route[0])
            {
                targetCube = Route[1];
                FindPath();
            }
            else if (currentCube == Route[1])
            {
                targetCube = Route[2];
                FindPath();
            }
            else if (currentCube == Route[2])
            {
                targetCube = Route[3];
                FindPath();
            }
            else if (currentCube == Route[3])
            {
                targetCube = Route[0];
                FindPath();
            }
            else
            {
                //finds closest route location
                Vector3 TempDistance;
                float ShortestDistance = 1000f;

                for (int i = 0; i < Route.Length; i++)
                {
                    TempDistance = currentCube.position - Route[i].position;

                    if (ShortestDistance > TempDistance.sqrMagnitude)
                    {
                        targetCube = Route[i];
                        ShortestDistance = TempDistance.sqrMagnitude;
                    }
                }
                FindPath();
            }
        }
        else
        {

            //FIX???
            FindPath();
        }

    }

    void DirectionalEnable()
    {
        //spawn block as child
        //place in direction foward towards target
        //collider trigger enable active2 on those in collider
        //send those enabled to temp location
        //perform search
        //dissable active2s when done using

    }

    void DirectionalDissable()
    {

    }



    void FindPath()
    {
        //finds path options
        //puts the police's current cube in the visitedCubes list, 
        //all of the the path options in nextCube list, 
        //and attaches the curent cube to all of the path 
        //options using the variable previousBlock
        //Then send info to exploreCube
        Dictionary<Transform, int> nextCubes = new Dictionary<Transform, int>();
        Dictionary<Transform, int> visitedCubes = new Dictionary<Transform, int>();

        //find direction of target
        //enable cubes in that direction

        foreach (WalkPath path in currentCube.GetComponent<PathBuilder>().possiblePaths)
        {
            if (path.active)
            {
                nextCubes.Add(path.target, 1);
                path.target.GetComponent<PathBuilder>().previousBlock = currentCube;
            }
        }

        visitedCubes.Add(currentCube, 0);
        Debug.Log("prehit");
        StartCoroutine(ExploreCubes(nextCubes, visitedCubes));
        BuildPath();
    }

    IEnumerator ExploreCubes(Dictionary<Transform, int> nextCubes, Dictionary<Transform, int> visitedCubes)
    {
        int distance = 2;
        while (nextCubes.Any())
        {
            //looks through all possible paths for best path
            //ExploreCube turns the first cube in the nextCube 
            //list into the currentCube and check to see if it is the target location
            //Then it repeats the same step from FindPath, 
            //checks if there are any cubes in the nextCube list, 
            //and then reruns explorecube with the new information
            //? The ExploreCube's problem could be that its not 
            //able to ever locate the target location and now that
            //the level is procedurally generated the amount of locations 
            //it has to go through could be a problem?
            
            Debug.Log(nextCubes.OrderBy(Key => Key.Value).Select(kp => kp.Key).First().name);
            
            Transform current = nextCubes.OrderBy(Key => Key.Value).Select(kp => kp.Key).First();
            nextCubes.Remove(current);

            // Debug.Log(Check + "Check");
            

            if (current == targetCube)
            {
                nextCubes.Clear();
                visitedCubes.Clear();
                Check = 0;
                Debug.Log("hit");
                yield break;
            }

            foreach (WalkPath path in current.GetComponent<PathBuilder>().possiblePaths)
            {
                if (!visitedCubes.ContainsKey(path.target) && path.active)
                {
                    nextCubes.Add(path.target, distance);
                    path.target.GetComponent<PathBuilder>().previousBlock = current;
                }
            }

            visitedCubes.Add(current, distance - 1);
            distance++;
            Check++;
        }
        yield break;
    }

    void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        //looks through all possible paths for best path
        //ExploreCube turns the first cube in the nextCube 
        //list into the currentCube and check to see if it is the target location
        //Then it repeats the same step from FindPath, 
        //checks if there are any cubes in the nextCube list, 
        //and then reruns explorecube with the new information
        //? The ExploreCube's problem could be that its not 
        //able to ever locate the target location and now that
        //the level is procedurally generated the amount of locations 
        //it has to go through could be a problem?
        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        Debug.Log(Check + "Check");

        /*if (current == targetCube || Check > 75)
        {
            nextCubes.Clear();
            visitedCubes.Clear();
            Check = 0;
            Debug.Log("hit");
            return;
        }*/

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
            Check++;
            ExploreCube(nextCubes, visitedCubes);
        }


    }

    void BuildPath()
    {
        Debug.Log("posthit");
        //builds the route
        //BuildPath runs when ExploreCube locates the target cube,
        //then adds the completed path to the finalPath list by 
        //itterating through each cube's previous block variable
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
        pathfinding = false;
        StopAllCoroutines();
        PathComplete = true;
        //Dissable
       // CheckForPlayer();


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

            if (Physics.Raycast(playerRay, out playerHit, 50f, mask))
            {
                if (playerHit.transform.CompareTag("Player"))
                {
                    Clear();
                    targetCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
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

            if (Physics.Raycast(playerRay, out playerHit, 50f, mask))
            {
                if (playerHit.transform.CompareTag("Player"))
                {
                    Clear();
                    targetCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
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


            if (Physics.Raycast(playerRay, out playerHit, 50f, mask))
            {

                if (playerHit.transform.CompareTag("Player"))
                {
                    Clear();
                    targetCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
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

            if (Physics.Raycast(playerRay, out playerHit, 50f, mask))
            {
                if (playerHit.transform.CompareTag("Player"))
                {
                    Clear();
                    targetCube = playerHit.transform.GetComponent<CharacterMovement>().RaycastDown();
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

    //not sure if i need this or not
    /*public void RaycastDownDissable()
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

    }*/

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

    IEnumerator Spawn()
    {
        Debug.Log("send");
        yield return new WaitForSeconds(0);
        if (Raycast(0) || Raycast(1) || Raycast(2) || Raycast(3))
        {
            Debug.Log("send 2");
            pathfinding = true;
            PathComplete = false;
            FindPath();
        }
    }



}
