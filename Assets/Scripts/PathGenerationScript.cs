using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerationScript : MonoBehaviour
{
    [SerializeField] GameObject SpawnManager;

    [SerializeField] GameObject path;
    [SerializeField] GameObject building;
    [SerializeField] int gridSize = 10;
    [SerializeField] float startBuildingChance = 0.5f;
    [SerializeField] float buildingChance = 0.5f;
    Transform[,] pathGrid;

    GameObject previousLeft = null;
    int previousLeftI = 0;
    int previousLeftJ = 0;
    GameObject previousTop = null;
    int previousTopI = 0;
    int previousTopJ = 0;
    GameObject previousRight = null;
    int previousRightI = 0;
    int previousRightJ = 0;
    GameObject previousBottom = null;
    int previousBottomI = 0;
    int previousBottomJ = 0;


    [SerializeField] int playerI;
    [SerializeField] int playerJ;

    [SerializeField] int visionLimit = 15;
    [SerializeField] int bottomVisionLimit = 10;

    int newGridOffsetI = 0;
    int newGridOffsetJ = 0;


    [SerializeField] GameObject[] buildingArray;
    [SerializeField] float[] buildingProbabilityArray;

    float buildingProbabilityRange = 100f;

    // Start is called before the first frame update
    void Start()
    {
        pathGrid = new Transform[gridSize, gridSize];
        buildingChance = startBuildingChance;
        ConstructGrid();
        SpawnManager.GetComponent<SpawnMasterScript>().StartingSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConstructGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {
                GenerateLayout(i, j, i, j);
            }
        }
    }

    public void UpdateGrid(int offsetI, int offsetJ)
    {
        //Debug.Log("OffsetI = " + offsetI + " OffsetJ = " + offsetJ);
        int newGridSize = gridSize + 1;
        Transform[,] newPathGrid = new Transform[newGridSize,newGridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                newPathGrid[i + offsetI, j + offsetJ] = pathGrid[i, j];
            }
        }
        pathGrid = newPathGrid;
        gridSize = newGridSize;
        int I = 0;
        int J = 0;
        int compensateI = 0;
        int compensateJ = 0;
        bool startOverlap = true;
        bool columnFirst = true;
        if (offsetI == 0 && offsetJ == 0)
        {
            //bottomRight
            compensateI = newGridOffsetI;
            compensateJ = newGridOffsetJ;

            I = newGridSize - 1;
            J = newGridSize - 1;
            startOverlap = false;
            columnFirst = true;
        }
        else if (offsetI > 0 && offsetJ == 0)
        {
            //topRight
            newGridOffsetI += 1;
            compensateI = newGridOffsetI;
            compensateJ = newGridOffsetJ;
            I = 0;
            J = newGridSize - 1;
            startOverlap = false;
            columnFirst = false;
        }
        else if (offsetI == 0 && offsetJ > 0)
        {
            //bottomLeft
            newGridOffsetJ += 1;
            compensateJ = newGridOffsetJ;
            compensateI = newGridOffsetI;
            I = newGridSize - 1;
            J = 0;
            startOverlap = true;
            columnFirst = true;
        }
        else if (offsetI > 0 && offsetJ > 0)
        {
            //topLeft
            newGridOffsetI += 1;
            compensateI = newGridOffsetI;
            newGridOffsetJ += 1;
            compensateJ = newGridOffsetJ;
            I = 0;
            J = 0;
            startOverlap = true;
            columnFirst = false;
        }
        //Debug.Log("CompensateI: " + compensateI + " / CompensateJ: " + compensateJ);
        if (columnFirst == true && startOverlap == true)
        {
            //Debug.Log("Bottom Left");
            for (int i = 0; i < newGridSize; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
            }
            for (int j = 1; j < newGridSize; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
            }
            
        }
        else if (columnFirst == true && startOverlap == false)
        {
            //Debug.Log("bottom right");
            for (int i = 0; i < newGridSize-1; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
            }
            for (int j = 0; j < newGridSize; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
            }
        }
        else if (columnFirst == false && startOverlap == true)
        {
            //Debug.Log("top left");
            for (int j = 0; j < newGridSize; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
            }
            for (int i = 1; i < newGridSize; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
            }
        }
        else
        {
            //Debug.Log("Top Right");
            for (int j = 0; j < newGridSize-1; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
            }
            for (int i = 0; i < newGridSize; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
            }
        }
    }

    public Transform GetNearestPath(int testI, int testJ)
    {
        //Debug.Log("hit1");
        Transform nearestPath = null;
        //Debug.Log(testI + "i");
        //Debug.Log(testJ + "j");
        //Debug.Log(gridSize-1 + "gs");
        if (testI < (gridSize-1) && testJ < (gridSize-1) && testI > 0 && testJ > 0)
        {
            //Debug.Log("hit2");
            if (pathGrid[testI, testJ] != null)
            {
                nearestPath = pathGrid[testI, testJ];
                if (pathGrid[testI, testJ].tag != "Path")
                {
                    if (pathGrid[testI - 1, testJ] != null)
                    {
                        if (pathGrid[testI - 1, testJ].tag == "Path")
                        {
                            nearestPath = pathGrid[testI - 1, testJ];
                        }
                    }
                    if (pathGrid[testI, testJ - 1] != null)
                    {
                        if (pathGrid[testI, testJ - 1].tag == "Path")
                        {
                            nearestPath = pathGrid[testI, testJ - 1];
                        }
                    }
                    if (pathGrid[testI, testJ + 1] != null)
                    {
                        if (pathGrid[testI, testJ + 1].tag == "Path")
                        {
                            nearestPath = pathGrid[testI, testJ + 1];
                        }
                    }
                    if (pathGrid[testI + 1, testJ] != null)
                    {
                        if (pathGrid[testI+1, testJ].tag == "Path")
                        {
                            nearestPath = pathGrid[testI + 1, testJ];
                        }
                    }
                }
            }
        }
        if (nearestPath == null)
        {
            Debug.Log("Didn't find path!");
        }
        return nearestPath;
    }

    public int[] GetEnemyCoordinates(GameObject enemySquare)
    {
        int enemyI = 0;
        int enemyJ = 0;
        if (enemySquare != null)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //Debug.Log("I: " + i + " J: " + j + " object = " + pathGrid[i, j].gameObject);
                    if (pathGrid[i, j].gameObject == enemySquare)
                    {
                        enemyI = i;
                        enemyJ = j;
                    }
                }
            }
        }
        return new int[] {enemyI, enemyJ};
    }


    public void SetPlayerSquare(GameObject playerSquare)
    {
        if (playerSquare != null)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //Debug.Log("I: " + i + " J: " + j + " object = " + pathGrid[i, j].gameObject);
                    if (pathGrid[i, j].gameObject == playerSquare)
                    {
                        playerI = i;
                        playerJ = j;
                    }
                }
            }
            if (playerI < visionLimit)
            {
                if (playerJ < (gridSize / 2))
                {
                    UpdateGrid(1, 1);
                }
                else
                {
                    UpdateGrid(1, 0);
                }
            }
            else if (playerI >= (gridSize - visionLimit))
            {
                if (playerJ < (gridSize / 2))
                {
                    UpdateGrid(0, 0);
                }
                else
                {
                    UpdateGrid(0, 1);
                }
            }
            else if (playerJ < visionLimit)
            {
                if (playerI < (gridSize / 2))
                {
                    UpdateGrid(1, 1);
                }
                else
                {
                    UpdateGrid(0, 1);
                }
            }
            else if (playerJ >= (gridSize - visionLimit))
            {
                if (playerI < (gridSize / 2))
                {
                    UpdateGrid(1, 0);
                }
                else
                {
                    UpdateGrid(0, 0);
                }
            }
            GridCull();
        }
        //GridDump();
    }

    bool CheckForRequiredPath(int currentI, int currentJ)
    {
        bool pathRequired = false;
        if (currentI-1 >= 0)
        {
            if (pathGrid[currentI-1,currentJ].tag == "Path")
            {
                if (pathGrid[currentI-1,currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                {
                    pathRequired = true;
                }
            }
        }
        if (currentJ-1 >= 0)
        {
            //Debug.Log("CurrentI = " + currentI + " CurrentJ = " + (currentJ - 1) + "  => " + pathGrid[currentI, currentJ - 1]);
            if (pathGrid[currentI, currentJ-1].tag == "Path")
            {
                if (pathGrid[currentI,currentJ-1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                {
                    pathRequired = true;
                }
            }
        }
        if (currentI + 1 < gridSize)
        {
            if (pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ].tag == "Path")
                {
                    if (pathGrid[currentI + 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                    {
                        pathRequired = true;
                    }
                }
            }
        }
        if (currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ+1] != null)
            {
                //Debug.Log("CurrentI = " + currentI + " CurrentJ = " + (currentJ - 1) + "  => " + pathGrid[currentI, currentJ - 1]);
                if (pathGrid[currentI, currentJ + 1].tag == "Path")
                {
                    if (pathGrid[currentI, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                    {
                        pathRequired = true;
                    }
                }
            }
        }
        if (currentI - 1 == 0 && currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI-1,currentJ] != null)
            {
                if (pathGrid[currentI-1, currentJ].tag == "Path")
                {
                    if (pathGrid[currentI-1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                    {
                        if (pathGrid[currentI - 1, currentJ + 1] != null)
                        {
                            if (pathGrid[currentI - 1, currentJ + 1].tag == "Path")
                            {
                                if (pathGrid[currentI - 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                                {
                                    Debug.Log("Special Requirement 1");
                                    pathRequired = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI + 1 < gridSize && currentJ + 1 == gridSize-1)
        {
            if (pathGrid[currentI, currentJ+1] != null)
            {
                if (pathGrid[currentI, currentJ+1].tag == "Path")
                {
                    if (pathGrid[currentI, currentJ+1].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                    {
                        if (pathGrid[currentI + 1, currentJ + 1] != null)
                        {
                            if (pathGrid[currentI + 1, currentJ + 1].tag == "Path")
                            {
                                if (pathGrid[currentI + 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                                {
                                    Debug.Log("Special Requirement 2");
                                    pathRequired = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return pathRequired;
    }

    bool CheckForLegalPath(int currentI, int currentJ)
    {
        bool pathLegal = true;

        if (currentI-1 >= 0)
        {
            if (currentJ+1 < gridSize)
            {
                if (pathGrid[currentI - 1, currentJ + 1] != null && pathGrid[currentI, currentJ + 1] != null)
                {
                    if (pathGrid[currentI - 1, currentJ].tag == "Path" && pathGrid[currentI - 1, currentJ + 1].tag == "Path" && pathGrid[currentI,currentJ+1].tag == "Path")
                    {
                        if (pathGrid[currentI - 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                        {
                            pathLegal = false;
                        }
                    }
                }
                else if (pathGrid[currentI - 1, currentJ + 1] != null && pathGrid[currentI, currentJ + 1] == null)
                {
                    if (pathGrid[currentI - 1, currentJ].tag == "Path" && pathGrid[currentI - 1, currentJ + 1].tag == "Path")
                    {
                        if (pathGrid[currentI - 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                        {
                            pathLegal = false;
                        }
                    }
                }

            }     
        }
        if (currentJ - 1 >= 0 && currentI - 1 >= 0)
        {
            if (pathGrid[currentI - 1, currentJ] != null && pathGrid[currentI, currentJ - 1] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag != "Path" && pathGrid[currentI, currentJ - 1].tag != "Path")
                {
                    if (currentI + 2 < gridSize)
                    {
                        if (pathGrid[currentI + 2, currentJ - 1] != null)
                        {
                            if (pathGrid[currentI + 2, currentJ - 1].tag == "Path")
                            {
                                if (pathGrid[currentI+2, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                {
                                    pathLegal = false;
                                }
                            }
                        }
                    }
                    if (currentI + 3 < gridSize)
                    {
                        if (pathGrid[currentI + 3, currentJ - 1] != null)
                        {
                            if (pathGrid[currentI + 2, currentJ - 1].tag == "Path")
                            {
                                if (pathGrid[currentI + 3, currentJ - 1].tag == "Path")
                                {
                                    if (pathGrid[currentI + 3, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                    {
                                        pathLegal = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI - 1 >= 0 && currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI - 1, currentJ] != null && pathGrid[currentI, currentJ + 1] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag != "Path" && pathGrid[currentI, currentJ + 1].tag != "Path")
                {
                    if (currentJ - 3 >= 0)
                    {
                        if (pathGrid[currentI - 1, currentJ - 3] != null)
                        {
                            if (pathGrid[currentI - 1, currentJ - 2].tag == "Path")
                            {
                                if (pathGrid[currentI - 1, currentJ - 3].tag == "Path")
                                {
                                    if (pathGrid[currentI, currentJ - 3].tag == "Path")
                                    {
                                        if (pathGrid[currentI - 1, currentJ - 3].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                        {
                                            pathLegal = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (currentI + 3 < gridSize)
                    {
                        if (pathGrid[currentI + 3, currentJ + 1] != null)
                        {
                            if (pathGrid[currentI + 2, currentJ + 1].tag == "Path")
                            {
                                if (pathGrid[currentI + 3, currentJ + 1].tag == "Path")
                                {
                                    if (pathGrid[currentI + 3, currentJ] == null)
                                    {
                                        if (pathGrid[currentI + 3, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                        {
                                            pathLegal = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI + 1 < gridSize && currentJ - 1 >= 0)
        {
            if (pathGrid[currentI, currentJ - 1] != null && pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ].tag != "Path" && pathGrid[currentI, currentJ - 1].tag != "Path")
                {
                    if (currentJ + 2 < gridSize)
                    {
                        if (pathGrid[currentI + 1, currentJ + 2] != null)
                        {
                            if (pathGrid[currentI + 1, currentJ + 2].tag == "Path")
                            {
                                if (pathGrid[currentI + 1, currentJ + 2].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                {
                                    pathLegal = false;
                                }
                            }
                        }
                    }
                    if (currentJ + 3 < gridSize)
                    {
                        if (pathGrid[currentI + 1, currentJ + 2] != null)
                        {
                            if (pathGrid[currentI + 1, currentJ + 2].tag == "Path")
                            {
                                if (pathGrid[currentI + 1, currentJ + 3] != null)
                                {
                                    if (pathGrid[currentI + 1, currentJ + 3].tag == "Path")
                                    {
                                        if (pathGrid[currentI + 1, currentJ + 3].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                        {
                                            pathLegal = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI + 1 < gridSize && currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ + 1] != null && pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ].tag != "Path" && pathGrid[currentI, currentJ + 1].tag != "Path")
                {
                    if (currentI - 3 >= 0)
                    {
                        if (pathGrid[currentI - 3, currentJ + 1] != null)
                        {
                            if (pathGrid[currentI - 2, currentJ + 1].tag == "Path")
                            {
                                if (pathGrid[currentI - 3, currentJ + 1].tag == "Path")
                                {
                                    if (pathGrid[currentI - 3, currentJ].tag == "Path")
                                    {
                                        if (pathGrid[currentI - 3, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                        {
                                            pathLegal = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (currentI + 1 < gridSize && currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ+1] != null)
            {
                if (pathGrid[currentI + 1, currentJ + 1] != null)
                {
                    if (pathGrid[currentI, currentJ+1].tag == "Path" && pathGrid[currentI + 1, currentJ + 1].tag == "Path")
                    {
                        if (pathGrid[currentI + 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                        {
                            pathLegal = false;
                        }
                    }
                }
            }
            
            if (pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ + 1] != null)
                {
                    if (pathGrid[currentI + 1, currentJ].tag == "Path" && pathGrid[currentI + 1, currentJ + 1].tag == "Path")
                    {
                        if (pathGrid[currentI + 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                        {
                            //Debug.Log("Potential False Positive??");
                            pathLegal = false;
                        }
                    }
                }
            }
        }
        if (currentI + 1 < gridSize && currentJ - 1 >= 0)
        {
            if (pathGrid[currentI + 1, currentJ - 1] != null && pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI, currentJ - 1].tag == "Path" && pathGrid[currentI + 1, currentJ - 1].tag == "Path" && pathGrid[currentI + 1, currentJ].tag == "Path")
                {
                    if (pathGrid[currentI + 1, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                    {
                        pathLegal = false;
                    }
                }
            }
            else if (pathGrid[currentI + 1, currentJ - 1] != null && pathGrid[currentI + 1, currentJ] == null)
            {
                if (pathGrid[currentI, currentJ - 1].tag == "Path" && pathGrid[currentI + 1, currentJ - 1].tag == "Path")
                {
                    if (pathGrid[currentI + 1, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                    {
                        pathLegal = false;
                    }
                }
            }
        }





        if (currentI - 1 >= 0)
        {
            if (pathGrid[currentI - 1, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag == "Path")
                {
                    if (currentJ - 1 >= 0)
                    {
                        if (pathGrid[currentI, currentJ - 1] != null)
                        {
                            if (pathGrid[currentI, currentJ - 1].tag == "Path")
                            {
                                if (pathGrid[currentI - 1, currentJ - 1] != null)
                                {
                                    if (pathGrid[currentI - 1, currentJ - 1].tag == "Path")
                                    {
                                        pathLegal = false;
                                    }
                                }
                            }
                        }
                    }
                    if (currentJ+1 < gridSize)
                    {
                        if (pathGrid[currentI, currentJ + 1] != null)
                        {
                            if (pathGrid[currentI, currentJ + 1].tag == "Path")
                            {
                                if (pathGrid[currentI - 1, currentJ + 1] != null)
                                {
                                    if (pathGrid[currentI - 1, currentJ + 1].tag == "Path")
                                    {
                                        pathLegal = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI + 1 < gridSize)
        {
            if (pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ].tag == "Path")
                {
                    if (currentJ - 1 >= 0)
                    {
                        if (pathGrid[currentI, currentJ - 1] != null)
                        {
                            if (pathGrid[currentI, currentJ - 1].tag == "Path")
                            {
                                if (pathGrid[currentI + 1, currentJ - 1] != null)
                                {
                                    if (pathGrid[currentI + 1, currentJ - 1].tag == "Path")
                                    {
                                        pathLegal = false;
                                    }
                                }
                            }
                        }
                    }
                    if (currentJ + 1 < gridSize)
                    {
                        if (pathGrid[currentI, currentJ + 1] != null)
                        {
                            if (pathGrid[currentI, currentJ + 1].tag == "Path")
                            {
                                if (pathGrid[currentI + 1, currentJ + 1] != null)
                                {
                                    if (pathGrid[currentI + 1, currentJ + 1].tag == "Path")
                                    {
                                        pathLegal = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        return pathLegal;
    }

    bool CheckForLegalBuilding(int currentI, int currentJ)
    {
        bool buildingLegal = true;
        if (currentI - 1 >= 0)
        {
            if (pathGrid[currentI - 1, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag != "Path")
                {
                    if (pathGrid[currentI - 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 2)
                    {
                        buildingLegal = false;
                    }
                }
            }
        }
        if (currentJ - 1 >= 0)
        {
            if (pathGrid[currentI, currentJ-1] != null)
            {
                if (pathGrid[currentI, currentJ-1].tag != "Path")
                {
                    if (pathGrid[currentI, currentJ-1].GetComponent<PathProperties>().GetAdjacentBuildings() > 2)
                    {
                        buildingLegal = false;
                    }
                }
            }
        }
        if (currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ + 1] != null)
            {
                if (pathGrid[currentI, currentJ + 1].tag != "Path")
                {
                    if (pathGrid[currentI, currentJ+1].GetComponent<PathProperties>().GetAdjacentBuildings() > 2)
                    {
                        buildingLegal = false;
                    }
                }
            }
        }
        if (currentI + 1 < gridSize)
        {
            if (pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ].tag != "Path")
                {
                    if (pathGrid[currentI + 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 2)
                    {
                        buildingLegal = false;
                    }
                }
            }
        }
        if (currentJ - 2 >= 0)
        {
            if (pathGrid[currentI, currentJ - 2] != null)
            {
                if (pathGrid[currentI, currentJ - 1].tag == "Path")
                {
                    //Debug.Log("I: " + currentI + " | J: " + currentJ + " check previous = " + pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings());
                    if (pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                    {
                        //Debug.Log("I: " + currentI + " | J: " + currentJ + " can be a block");
                        if (pathGrid[currentI, currentJ - 2].tag == "Path")
                        {
                            //Debug.Log("I: " + currentI + " | J: " + currentJ + " might be a block");
                            if (pathGrid[currentI, currentJ - 2].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                            {
                                if (pathGrid[currentI, currentJ - 2].GetComponent<PathProperties>().GetCorner() == true)
                                {
                                    //Debug.Log("Corner 2 Left of " + currentI + ", " + currentJ);
                                    for (int k = 0; k < 4; k++)
                                    {
                                        //Debug.Log("Corner Code = " + pathGrid[currentI, currentJ - 2].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                        //Debug.Log("Adjacent Code = " + pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                        if (pathGrid[currentI, currentJ - 2].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1 && pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1)
                                        {
                                            buildingLegal = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentJ + 2 < gridSize)
        {
            if (pathGrid[currentI, currentJ + 2] != null)
            {
                if (pathGrid[currentI, currentJ + 1].tag == "Path")
                {
                    //Debug.Log("I: " + currentI + " | J: " + currentJ + " check previous = " + pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings());
                    if (pathGrid[currentI, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                    {
                        //Debug.Log("I: " + currentI + " | J: " + currentJ + " can be a block");
                        if (pathGrid[currentI, currentJ + 2].tag == "Path")
                        {
                            //Debug.Log("I: " + currentI + " | J: " + currentJ + " might be a block");
                            if (pathGrid[currentI, currentJ + 2].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                            {
                                if (pathGrid[currentI,currentJ+2].GetComponent<PathProperties>().GetCorner() == true)
                                {
                                    //Debug.Log("CORNER 2 right of " + currentI + ", " + currentJ);
                                    
                                    for (int k = 0; k < 4; k++)
                                    {
                                        //Debug.Log("Corner Code = " + pathGrid[currentI, currentJ + 2].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                        //Debug.Log("Adjacent Code = " + pathGrid[currentI, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                        if (pathGrid[currentI, currentJ + 2].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1 && pathGrid[currentI, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1)
                                        {
                                            buildingLegal = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI + 2 < gridSize)
        {
            if (pathGrid[currentI+2, currentJ] != null)
            {
                if (pathGrid[currentI+1, currentJ].tag == "Path")
                {
                    //Debug.Log("I: " + currentI + " | J: " + currentJ + " check previous = " + pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings());
                    if (pathGrid[currentI+1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                    {
                        //Debug.Log("I: " + currentI + " | J: " + currentJ + " can be a block");
                        if (pathGrid[currentI+2, currentJ].tag == "Path")
                        {
                            //Debug.Log("I: " + currentI + " | J: " + currentJ + " might be a block");
                            if (pathGrid[currentI+2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                            {
                                if (pathGrid[currentI+2, currentJ].GetComponent<PathProperties>().GetCorner() == true)
                                {
                                    //Debug.Log("Corner 2 DOWN of " + currentI + ", " + currentJ);
                                    for (int k = 0; k < 4; k++)
                                    {
                                        //Debug.Log("Corner Code = " + pathGrid[currentI + 2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                        //Debug.Log("Adjacent Code = " + pathGrid[currentI + 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);

                                        if (pathGrid[currentI+2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1 && pathGrid[currentI+1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1)
                                        {
                                            buildingLegal = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI - 2 >= 0)
        {
            if (pathGrid[currentI - 2, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag == "Path")
                {
                    //Debug.Log("I: " + currentI + " | J: " + currentJ + " check previous = " + pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings());
                    if (pathGrid[currentI - 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                    {
                        //Debug.Log("I: " + currentI + " | J: " + currentJ + " can be a block");
                        if (pathGrid[currentI - 2, currentJ].tag == "Path")
                        {
                            if (pathGrid[currentI - 2, currentJ].GetComponent<PathProperties>().GetCorner() == true)
                            {
                                //Debug.Log("Corner 2 up of " + currentI + ", " + currentJ);
                                for (int k = 0; k < 4; k++)
                                {
                                    //Debug.Log("Corner Code = " + pathGrid[currentI - 2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                    //Debug.Log("Adjacent Code = " + pathGrid[currentI - 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                    if (pathGrid[currentI - 2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1 && pathGrid[currentI - 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1)
                                    {
                                        buildingLegal = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return buildingLegal;
    }

    void SetPrevious(int currentI, int currentJ)
    {
        if (currentJ-1 >= 0)
        {
            previousLeft = pathGrid[currentI, currentJ - 1].gameObject;
            previousLeftI = currentI;
            previousLeftJ = currentJ - 1;
        }
        else
        {
            previousLeft = null;
        }
        if (currentI-1 >= 0)
        {
            previousTop = pathGrid[currentI - 1, currentJ].gameObject;
            previousTopI = currentI - 1;
            previousTopJ = currentJ;
        }
        else
        {
            previousTop = null;
        }
        if (currentJ+1 < gridSize)
        {
            if (pathGrid[currentI,currentJ+1] != null)
            {
                previousRight = pathGrid[currentI, currentJ + 1].gameObject;
                previousRightI = currentI;
                previousRightJ = currentJ + 1;
            }
        }
        else
        {
            previousRight = null;
        }
        if (currentI+1 < gridSize)
        {
            if (pathGrid[currentI+1,currentJ] != null)
            {
                previousBottom = pathGrid[currentI + 1, currentJ].gameObject;
                previousBottomI = currentI + 1;
                previousBottomJ = currentJ;
            }
        }
        else
        {
            previousBottom = null;
        }
    }

    void SetBuildingChance(int currentI, int currentJ)
    {
        float chanceIncrement = 0.12f;
        float chanceDecay = 0.2f;
        if (currentI-1 >= 0)
        {
            if (pathGrid[currentI - 1, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                }
            }
            if (currentJ-1 >= 0)
            {
                if (pathGrid[currentI, currentJ - 1] != null)
                {
                    if (pathGrid[currentI, currentJ - 1].tag == "Path")
                    {
                        buildingChance -= chanceDecay;
                        if (currentI - 3 >= 0)
                        {
                            if (pathGrid[currentI - 3, currentJ] != null)
                            {
                                if (pathGrid[currentI - 3, currentJ].tag == "Path")
                                {
                                    buildingChance -= chanceDecay;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentI + 1 < gridSize)
        {
            if (pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                }
            }
            if (currentJ + 1 < gridSize)
            {
                if (pathGrid[currentI, currentJ + 1] != null)
                {
                    if (pathGrid[currentI, currentJ + 1].tag == "Path")
                    {
                        buildingChance -= chanceDecay;
                        if (currentI + 3 < gridSize)
                        {
                            if (pathGrid[currentI + 3, currentJ] != null)
                            {
                                if (pathGrid[currentI + 3, currentJ].tag == "Path")
                                {
                                    buildingChance -= chanceDecay;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentJ-1 >= 0)
        {
            if (pathGrid[currentI, currentJ - 1] != null)
            {
                if (pathGrid[currentI, currentJ - 1].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                }
            }
            if (currentI-1 >= 0)
            {
                if (pathGrid[currentI - 1, currentJ] != null)
                {
                    if (pathGrid[currentI - 1, currentJ].tag == "Path")
                    {
                        buildingChance -= chanceDecay;
                        if (currentJ - 3 >= 0)
                        {
                            if (pathGrid[currentI, currentJ - 3] != null)
                            {
                                if (pathGrid[currentI, currentJ - 3].tag == "Path")
                                {
                                    buildingChance -= chanceDecay;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ + 1] != null)
            {
                if (pathGrid[currentI, currentJ + 1].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                }
            }
            if (currentI + 1 < gridSize)
            {
                if (pathGrid[currentI + 1, currentJ] != null)
                {
                    if (pathGrid[currentI + 1, currentJ].tag == "Path")
                    {
                        buildingChance -= chanceDecay;
                        if (currentJ + 3 < gridSize)
                        {
                            if (pathGrid[currentI, currentJ + 3] != null)
                            {
                                if (pathGrid[currentI, currentJ + 3].tag == "Path")
                                {
                                    buildingChance -= chanceDecay;
                                }
                            }
                        }
                    }
                }
            }
        }

        bool set = false;
        if (currentI-1 >= 0 && currentJ-1 >= 0 && set == false)
        {
            if (pathGrid[currentI - 1, currentJ - 1] != null && pathGrid[currentI, currentJ - 1] != null && pathGrid[currentI - 1, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ - 1].tag != "Path" && pathGrid[currentI, currentJ - 1].tag != "Path" && pathGrid[currentI - 1, currentJ].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                    set = true;
                }
            }
        }
        if (currentI + 1 < gridSize && currentJ - 1 >= 0 && set == false)
        {
            if (pathGrid[currentI + 1, currentJ - 1] != null && pathGrid[currentI, currentJ - 1] != null && pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ - 1].tag != "Path" && pathGrid[currentI, currentJ - 1].tag != "Path" && pathGrid[currentI + 1, currentJ].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                    set = true;
                }
            }
        }
        if (currentI - 1 >= 0 && currentJ + 1 < gridSize && set == false)
        {
            if (pathGrid[currentI - 1, currentJ + 1] != null && pathGrid[currentI, currentJ + 1] != null && pathGrid[currentI - 1, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ + 1].tag != "Path" && pathGrid[currentI, currentJ + 1].tag != "Path" && pathGrid[currentI - 1, currentJ].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                    set = true;
                }
            }
        }
        if (currentI + 1 < gridSize && currentJ + 1 < gridSize && set == false)
        {
            if (pathGrid[currentI + 1, currentJ + 1] != null && pathGrid[currentI, currentJ + 1] != null && pathGrid[currentI + 1, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ + 1].tag != "Path" && pathGrid[currentI, currentJ + 1].tag != "Path" && pathGrid[currentI + 1, currentJ].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                    set = true;
                }
            }
        }
    }

    void GenerateLayout(int coordinateI, int coordinateJ, int worldI, int worldJ)
    {
        if (CheckForRequiredPath(coordinateI, coordinateJ) == true)
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
            if (coordinateJ+1 < gridSize)
            {
                if (pathGrid[coordinateI,coordinateJ+1] != null)
                {
                    right = pathGrid[coordinateI, coordinateJ + 1].gameObject;
                }
            }
            if (coordinateI+1 < gridSize)
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
        SetupPreviousProperties(previousBottom, previousBottomI, previousBottomJ);
    }

    void SetupPreviousProperties(GameObject previousBlock, int pI, int pJ)
    {
        if (previousBlock != null)
        {
            //if (previousBlock.tag == "Path")
            //{
                GameObject top = null;
                GameObject left = null;
                GameObject right = null;
                GameObject bottom = null;
                if (pI - 1 >= 0)
                {
                    if (pathGrid[pI - 1, pJ] != null)
                    {
                        top = pathGrid[pI - 1, pJ].gameObject;
                    }
                }
                if (pJ - 1 >= 0)
                {
                    if (pathGrid[pI, pJ - 1] != null)
                    {
                        left = pathGrid[pI, pJ - 1].gameObject;
                    }
                }
                if (pJ + 1 < gridSize)
                {
                    if (pathGrid[pI, pJ + 1] != null)
                    {
                        right = pathGrid[pI, pJ + 1].gameObject;
                    }
                }
                if (pI + 1 < gridSize)
                {
                    if (pathGrid[pI + 1, pJ] != null)
                    {
                        bottom = pathGrid[pI + 1, pJ].gameObject;
                    }
                }
                previousBlock.GetComponent<PathProperties>().Setup(top, left, right, bottom);
            //}
        }
    }

    void GridCull()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                /*if (i-playerI >= 0 && j-playerJ < 0)
                {
                    //Use smaller limit
                    if (Mathf.Abs(i - playerI) > bottomVisionLimit && Mathf.Abs(j - playerJ) > bottomVisionLimit)
                    {
                        pathGrid[i, j].GetComponent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        pathGrid[i, j].GetComponent<MeshRenderer>().enabled = true;
                    }
                }
                else
                {*/
                if ((Mathf.Abs(i - playerI) + Mathf.Abs(j - playerJ))/2 > (visionLimit*0.75f) || Mathf.Abs(i - playerI) > visionLimit || Mathf.Abs(j - playerJ) > visionLimit)
                {
                    pathGrid[i, j].GetComponent<MeshRenderer>().enabled = false;
                    if(pathGrid[i, j].CompareTag("Path"))
                    {
                        pathGrid[i, j].GetComponent<PathBuilder>().PathDissable();
                    }
                    
                }
                else
                {
                    pathGrid[i, j].GetComponent<MeshRenderer>().enabled = true;
                    if (pathGrid[i, j].CompareTag("Path"))
                    {
                        pathGrid[i, j].GetComponent<PathBuilder>().PathEnable();
                    }
                }
                /*}*/
            }
        }
    }

    public int GetGridSize()
    {
        return gridSize;
    }

    public Transform[,] GetPathGrid()
    {
        return pathGrid;
    }

    public int[] GetPlayerCoordinates()
    {
        int[] playerCoordinates = { playerI, playerJ };
        for (int i = 0; i < playerCoordinates.Length; i++)
        {
            //Debug.Log(playerCoordinates[i] + "pc");
        }
        return playerCoordinates;
    }

    public int GetVisionLimit()
    {
        //Debug.Log(visionLimit + "vl");
        return visionLimit;
    }

    public void TraversePath(int previousSpaceI, int previousSpaceJ, Transform destinationSpace)
    {
        
    }


    GameObject SelectBuilding(GameObject buildingToSpawn)
    {
        

        float randomResult = Random.Range(0, buildingProbabilityRange);

        for (int i = 0; i < buildingProbabilityArray.Length; i++)
        {
            if (randomResult < buildingProbabilityArray[i])
            {
                buildingToSpawn = buildingArray[i];
                break;
            }
        }

        return buildingToSpawn;

    }


    void GridDump()
    {
        string grid = "";
        string row = "";
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (pathGrid[i,j].gameObject.tag == "Path")
                {
                    row += "P";
                }
                else
                {
                    row += "X";
                }
                if (j < gridSize-1)
                {
                    row += "|";
                }
                else
                {
                    row += "\n";
                }
            }
            grid += row;
        }
        Debug.Log(grid);
    }
}
