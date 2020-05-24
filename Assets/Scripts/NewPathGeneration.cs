using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewPathGeneration : MonoBehaviour
{
    public GameObject Player;
    public GameObject Police;
    public GameObject Path;
    public GameObject TempPlaceholder;
    bool finished = false;
    GameObject[] MacroGrid;

    [SerializeField] public int gridSize = 9;
    int macroGridSize = 3;
    Transform[,] pathGrid;
    List<GameObject> paths = new List<GameObject>();
    

    void Start()
    {
        pathGrid = new Transform[gridSize, gridSize];
        
        GenerateMacroGrid();
        StartCoroutine(Spawn());

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ColliderCheck();
            
        }
    }


    void GenerateMacroGrid()
    {
        //Debug.Log(1);
        for (int i = 0; i < macroGridSize; i++)
        {
            for (var j = 0; j < macroGridSize; j++)
            {
                
                GenerateMicroGrid((i-1)*gridSize, (j-1)*gridSize);
            }
        }
        //Debug.Log(2.9);
        finished = true;
        StartCoroutine(Trigger());




    }

    IEnumerator Trigger()
    {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0);
        //yield return new WaitUntil(finished);
        while(!finished)
        {
           
            
        }
        ColliderCheck();
        finished = false;
        
    }

    void LocatePaths()
    {
       // Debug.Log(6);
        GameObject[] Paths = GameObject.FindGameObjectsWithTag("Path");
        for(int i = 0; i < Paths.Length; i++)
        {
            Paths[i].GetComponent<PathBuilder>().LocatePaths();
        }
    }


    public void GenerateMicroGrid(int x, int y)
    {
        
        GameObject T = Instantiate(TempPlaceholder, new Vector3(x, -1, y), new Quaternion(0,0,0,0));
        
        for (int i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {

                //Delay til generate grid
                //GameObject g = Instantiate(Path, T.transform.position, T.transform.rotation, T.transform);
                // g.transform.localPosition = new Vector3(i - gridSize/2, 1, j - gridSize/2);
                //paths.Add(g);
                GenerateLayout(i, j, T);
            }
        }

        //Debug.Log(2);
        

    }

    void GenerateLayout(int i, int j, GameObject T)
    {
        GameObject g = Instantiate(Path, T.transform.position, T.transform.rotation, T.transform);
        g.transform.localPosition = new Vector3(i - gridSize / 2, 1, j - gridSize / 2);

        /*if (CheckForRequiredPath(coordinateI, coordinateJ) == true)
        {
            //Debug.Log("Location: " + coordinateI + ", " + coordinateJ + " Path is REQUIRED.");
            pathGrid[coordinateI, coordinateJ] = Instantiate<Transform>(path.transform, new Vector3(worldI, -1, worldJ), Quaternion.identity);
        }
        else
        {

            if (CheckForLegalPath(coordinateI, coordinateJ) == true && CheckForLegalBuilding(coordinateI, coordinateJ) == true)
            {
                SetBuildingChance(coordinateI, coordinateJ);
                //pathGrid[i, j] = Instantiate<Transform>(path.transform, new Vector3(i - (gridSize / 2), -1, j - (gridSize / 2)), Quaternion.identity);
                var max = 100;
                var threshold = buildingChance * max;
                var buildingRoll = Random.Range(0, max);
                if (buildingRoll < threshold)
                {
                    pathGrid[coordinateI, coordinateJ] = Instantiate<Transform>(SelectBuilding(building).transform, new Vector3(worldI, -0.5f, worldJ), Quaternion.identity);
                }
                else
                {
                    pathGrid[coordinateI, coordinateJ] = Instantiate<Transform>(path.transform, new Vector3(worldI, -1, worldJ), Quaternion.identity);
                }
                //Reset buidling chance
                buildingChance = startBuildingChance;
            }
            else if (CheckForLegalPath(coordinateI, coordinateJ) == false && CheckForLegalBuilding(coordinateI, coordinateJ) == true)
            {
                //Debug.Log("coordinates " + coordinateI + ", " + coordinateJ + " PATH is illegal.");
                pathGrid[coordinateI, coordinateJ] = Instantiate<Transform>(SelectBuilding(building).transform, new Vector3(worldI, -0.5f, worldJ), Quaternion.identity);
            }
            else if (CheckForLegalPath(coordinateI, coordinateJ) == true && CheckForLegalBuilding(coordinateI, coordinateJ) == false)
            {
                //Debug.Log("At " + coordinateI + ", " + coordinateJ + " building is illegal.");
                pathGrid[coordinateI, coordinateJ] = Instantiate<Transform>(path.transform, new Vector3(worldI, -1, worldJ), Quaternion.identity);
            }
            else
            {
                //Debug.Log("Position: " + coordinateI + ", " + coordinateJ + " both are illegal.");
                pathGrid[coordinateI, coordinateJ] = Instantiate<Transform>(SelectBuilding(building).transform, new Vector3(worldI, -0.5f, worldJ), Quaternion.identity);
            }
        }
        //if (pathGrid[coordinateI, coordinateJ].tag == "Path")
        //{
        GameObject top = null;
        GameObject left = null;
        GameObject right = null;
        GameObject bottom = null;
        if (coordinateI - 1 >= 0)
        {
            if (pathGrid[coordinateI - 1, coordinateJ] != null)
            {
                top = pathGrid[coordinateI - 1, coordinateJ].gameObject;
            }
        }
        if (coordinateJ - 1 >= 0)
        {
            if (pathGrid[coordinateI, coordinateJ - 1] != null)
            {
                left = pathGrid[coordinateI, coordinateJ - 1].gameObject;
            }
        }
        if (coordinateJ + 1 < gridSize)
        {
            if (pathGrid[coordinateI, coordinateJ + 1] != null)
            {
                right = pathGrid[coordinateI, coordinateJ + 1].gameObject;
            }
        }
        if (coordinateI + 1 < gridSize)
        {
            if (pathGrid[coordinateI + 1, coordinateJ] != null)
            {
                bottom = pathGrid[coordinateI + 1, coordinateJ].gameObject;
            }
        }
        pathGrid[coordinateI, coordinateJ].gameObject.GetComponent<PathProperties>().Setup(top, left, right, bottom);
        //}
        SetPrevious(coordinateI, coordinateJ);
        SetupPreviousProperties(previousTop, previousTopI, previousTopJ);
        SetupPreviousProperties(previousLeft, previousLeftI, previousLeftJ);
        SetupPreviousProperties(previousRight, previousRightI, previousRightJ);
        SetupPreviousProperties(previousBottom, previousBottomI, previousBottomJ);*/
    }

    

    public void ColliderCheck()
    {
        //Debug.Log(5);
        MacroGrid = GameObject.FindGameObjectsWithTag("Macro");
        for (int k = 0; k < MacroGrid.Length; k++)
        {
            bool left = false;
            bool down = false;
            bool right = false;
            bool up = false;

            for (int h = 0; h < MacroGrid.Length; h++)
            {
                
                
                if (MacroGrid[h].transform.position == new Vector3(MacroGrid[k].transform.position.x - gridSize, -1, MacroGrid[k].transform.position.z))
                {
                    left = true;
                }
                if (MacroGrid[h].transform.position == new Vector3(MacroGrid[k].transform.position.x, -1, MacroGrid[k].transform.position.z - gridSize))
                {
                    down = true;
                }
                if (MacroGrid[h].transform.position == new Vector3(MacroGrid[k].transform.position.x + gridSize, -1, MacroGrid[k].transform.position.z))
                {
                    right = true;
                }
                if (MacroGrid[h].transform.position == new Vector3(MacroGrid[k].transform.position.x, -1, MacroGrid[k].transform.position.z + gridSize))
                {
                    up = true;
                }
                if (left && down && right && up)
                {
                    break;
                }


            }

            if (left && down && right && up)
            {
                for (int g = 0; g < 4; g++)
                {
                    MacroGrid[k].transform.GetChild(g).gameObject.SetActive(true);
                    //Debug.Log("true");
                }
            }
            else
            {
                for (int g = 0; g < 4; g++)
                {
                    MacroGrid[k].transform.GetChild(g).gameObject.SetActive(false);
                    //Debug.Log("false");
                }
            }

            

        }
        LocatePaths();
        //Debug.Log(5555555555555555);
        /*for (int y = 0; y < MacroGrid.Length; y++)
        {
            MacroGrid[y] = null;
        }*/

    }

    

    public void CheckMacroLocation(int ColNum, float x, float z)
    {
        
        
        MacroGrid = GameObject.FindGameObjectsWithTag("Macro");
        if (ColNum == 1)
        {
            //Debug.Log(1);
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x - (gridSize*2), -1, z + gridSize))
                {
                    break;
                }
                if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x - (gridSize * 2), (int)z + gridSize);
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x + gridSize, -1, z - gridSize))
                        {
                            Destroy(MacroGrid[j]);   
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {   
                if (MacroGrid[k].transform.position == new Vector3(x - (gridSize * 2), -1, z))
                {
                    break;
                }
                if (k == MacroGrid.Length - 1)
                {
                    
                    GenerateMicroGrid((int)x - (gridSize * 2), (int)z);
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x + gridSize, -1, z))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x - (gridSize * 2), -1, z - gridSize))
                {
                    break;
                }
                if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x - (gridSize * 2), (int)z - gridSize);
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x + gridSize, -1, z + gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            
        }

        if (ColNum == 2)
        {
            //Debug.Log(2);
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x - gridSize, -1, z - (gridSize * 2)))
                {
                    break;
                }
                if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x - gridSize, (int)z - (gridSize * 2));
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x + gridSize, -1, z + gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x, -1, z - (gridSize * 2)))
                {
                    break;
                }
                if (k == MacroGrid.Length - 1)
                {

                    GenerateMicroGrid((int)x, (int)z - (gridSize * 2));
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x, -1, z + gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x + gridSize, -1, z - (gridSize * 2)))
                {
                    break;
                }
                if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x + gridSize, (int)z - (gridSize * 2));
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x - gridSize, -1, z + gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
           
        }

        if (ColNum == 3)
        {
            //Debug.Log(3);
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x + (gridSize * 2), -1, z - gridSize))
                {
                    break;
                }
                else if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x + (gridSize * 2), (int)z - gridSize);
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x - gridSize, -1, z + gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x + (gridSize * 2), -1, z))
                {
                    break;
                }
                else if (k == MacroGrid.Length - 1)
                {

                    GenerateMicroGrid((int)x + (gridSize * 2), (int)z);
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x - gridSize, -1, z))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x + (gridSize * 2), -1, z + gridSize))
                {
                    break;
                }
                else if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x + (gridSize * 2), (int)z + gridSize);
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x - gridSize, -1, z - gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            
        }

        if (ColNum == 4)
        {
            //Debug.Log(4);
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x - gridSize, -1, z + (gridSize * 2)))
                {
                    break;
                }
                else if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x - gridSize, (int)z + (gridSize * 2));
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x + gridSize, -1, z - gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x, -1, z + (gridSize * 2)))
                {
                    break;
                }
                else if (k == MacroGrid.Length - 1)
                {

                    GenerateMicroGrid((int)x, (int)z + (gridSize * 2));
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x, -1, z - gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            for (int k = 0; k < MacroGrid.Length; k++)
            {
                if (MacroGrid[k].transform.position == new Vector3(x + gridSize, -1, z + (gridSize * 2)))
                {
                    break;
                }
                else if (k == MacroGrid.Length - 1)
                {
                    GenerateMicroGrid((int)x + gridSize, (int)z + (gridSize * 2));
                    for (int j = 0; j < MacroGrid.Length; j++)
                    {
                        if (MacroGrid[j].transform.position == new Vector3(x - gridSize, -1, z - gridSize))
                        {
                            Destroy(MacroGrid[j]);
                        }
                    }
                }
            }
            
        }
       
        finished = true;
        StartCoroutine(Trigger());



    }
    IEnumerator Spawn()
    {
        yield return new WaitForEndOfFrame();
        Instantiate(Player, new Vector3(0, 1, 0), new Quaternion(0, 0, 0, 0));
        //Instantiate(Police, new Vector3(4, 1, 0), new Quaternion(0, 0, 0, 0));

    }



}
