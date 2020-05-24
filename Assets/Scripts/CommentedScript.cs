using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentedScript : MonoBehaviour
{
    /// <summary>
    /// Varibles
    /// </summary>

    //The spawnmaster object handles police, firefighter, and player spawning
    [SerializeField] GameObject SpawnManager;

    //Path prefab object
    [SerializeField] GameObject path;
    //House, placeholder
    [SerializeField] GameObject building;
    //grid size refers to the length or width of the grid, it's always square, the number of cells is this times itself
    [SerializeField] int gridSize = 10;
    //Where the building random chance starts, it gets modified to get better shaped results.
    [SerializeField] float startBuildingChance = 0.5f;
    //This is the building chance that actually changes
    [SerializeField] float buildingChance = 0.5f;
    //The all-important path grid. Think of this as a mental image of the actual grid. It keeps everything in a 2d array. 
    //While it currently can grow to immense sizes, I think it is optimal to reference the game logic through this array whenever possible.
    //If you try our plan of locking grid size and forgetting old areas, this will become even more efficient, I believe.
    Transform[,] pathGrid;

    //These are references to the adjacent blocks to the top, left, right, and bottom of the current space. The path generation algorithm needs to 
    //Reference these objects to update their path properties and instruct the placement of the current building or path
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

    //The coordinates of the player, I is row (y), J is column (x), by nature of how I iterate 2d arrays. 
    [SerializeField] int playerI;
    [SerializeField] int playerJ;

    //This is for culling, 15 spaces in all 4 directions and diagonals as well, cutting off corners
    [SerializeField] int visionLimit = 15;
    //This was for cutting off the bottom sooner because it had more room, you could repurpose this to get the tall rectangle shape.
    [SerializeField] int bottomVisionLimit = 10;

    //These are important now, but if you implement the changes the way I think you should, they won't be necessary.
    //In order to expand the grid, I copy the old one into a new one thats one size bigger, depending on where the player is, the old grid 
    //occupies one of the 4 corners while two new edges are created closer to the player
    int newGridOffsetI = 0;
    int newGridOffsetJ = 0;

    //This is for selecting the building that goes in a building space
    [SerializeField] GameObject[] buildingArray;
    [SerializeField] float[] buildingProbabilityArray;

    float buildingProbabilityRange = 100f;

    
    void Start()
    {
        //Initialize pathgrid
        pathGrid = new Transform[gridSize, gridSize];
        //Initialize building chance
        buildingChance = startBuildingChance;
        //Perform the initial construction process, doing all the j spaces in one i row and moving on to the next
        ConstructGrid();
        //Once the grid is ready, the spawn master can place the player and enemies, this will be called later when more enemies are needed.
        //Though you will need to change that process to suit the optimized grid.
        SpawnManager.GetComponent<SpawnMasterScript>().StartingSetup();
    }

    //No update needed

    void ConstructGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {
                //Putting off this code to another function, but I reuse it later so it's efficient, you'll see
                GenerateLayout(i, j, i, j);
            }
        }
    }


    //I made this a coroutine but it's too slow this way*. You may need to make it a regular function again if you're doing the fixed grid size.
    //New spaces need to check adjacent spaces to determine if it's illegal to put a path or building, this coroutine allows one new grid to start forming 
    //before the previous one is complete. That's no good.
    IEnumerator UpdateGrid(int offsetI, int offsetJ)
    {
        //Debug.Log("OffsetI = " + offsetI + " OffsetJ = " + offsetJ);
        //Self explanatory, but increment grid size
        int newGridSize = gridSize + 1;
        //Create a new pathGrid
        Transform[,] newPathGrid = new Transform[newGridSize, newGridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                //copy the ENTIRE grid into the new one leaving an empty row and column for the new spaces.
                //This is the heaviest load. I did not put yield return null here because you'd be waiting minutes for the new grid to complete.
                newPathGrid[i + offsetI, j + offsetJ] = pathGrid[i, j];
            }
        }
        //copy it over the current path grid and grid size
        pathGrid = newPathGrid;
        gridSize = newGridSize;
        //Lazy variable name on my part, these capital letters are different from the lowercase. These remain constant as the little letters iterate.
        //This is how I keep updates within a column and row
        int I = 0;
        int J = 0;
        //Ok, so this is a little complicated. But it's important you understand this becuase its a big reason why fixing grid size will help optimize.
        //The pathGrid on initial construction corresponds with the coordinates of the real in-game grid 1to1. The spaces are 1 unit apart so 
        //pathGrid[1,3] is at location Vector3(1,-.5,3) and so on. But let's say the player moves up and to the left. A new grid is created with a new row 
        //at position pathGrid[0,] and a new column at position pathGrid[,0], but where does this go in the real grid? Rather than move the whole grid over 
        //So a new row can be made, I use the COMPENSATE variables to move the new spaces over. These are preserved in the newGridOffset variables.
        //Because the next row and coluumn would need to be -2 spaces over if the player continues in that same direction
        int compensateI = 0;
        int compensateJ = 0;
        //So remember that new grids leave a row and column blank so the update can place new paths or buildings? Well, is the corner part of the row or
        //column? That depends on the order you're doing the row or column in. A new path or building cannot be placed next to a null part of the path grid
        //array, at least not effectively, so for our upper left example startOverlap is true so the corner is handled by the first row or column we do.
        //We do the row in this case because (at least in earlier builds so this may not be 100% necessary or could be more efficient) it's expecting to
        //Check above and to the left because the left-to-right, top-to-bottom nature of the grid's construction
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

        //These functions take the values generated above and use them to iterate through the actual construction of the grid.
        //These are the parts currently running on the second stream in the coroutine, hence the yield return null. Unless there's a way
        //To drastically speed it up, it's too noticeable. Get rid of the yield return nulls when you make this no longer a coroutine
        if (columnFirst == true && startOverlap == true)
        {
            //Debug.Log("Bottom Left");
            for (int i = 0; i < newGridSize; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
                yield return null;
            }
            for (int j = 1; j < newGridSize; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
                yield return null;
            }

        }
        else if (columnFirst == true && startOverlap == false)
        {
            //Debug.Log("bottom right");
            for (int i = 0; i < newGridSize - 1; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
                yield return null;
            }
            for (int j = 0; j < newGridSize; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
                yield return null;
            }
        }
        else if (columnFirst == false && startOverlap == true)
        {
            //Debug.Log("top left");
            for (int j = 0; j < newGridSize; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
                yield return null;
            }
            for (int i = 1; i < newGridSize; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
                yield return null;
            }
        }
        else
        {
            //Debug.Log("Top Right");
            for (int j = 0; j < newGridSize - 1; j++)
            {
                //pathGrid[I, j] = Instantiate<Transform>(building.transform, new Vector3(I-compensateI, -1, j-compensateJ), Quaternion.identity);
                GenerateLayout(I, j, I - compensateI, j - compensateJ);
                yield return null;
            }
            for (int i = 0; i < newGridSize; i++)
            {
                //Debug.Log("New column: I = " + i + " & J = " + J);
                //pathGrid[i, J] = Instantiate<Transform>(building.transform, new Vector3(i-compensateI, -1, J-compensateJ), Quaternion.identity);
                GenerateLayout(i, J, i - compensateI, J - compensateJ);
                yield return null;
            }
        }
        //Don't forget about that pesky GridCull!
        GridCull();
    }

    //Not Exactly in ordere here, but this is a function for checking in a + shape for a path. Used for spawning player and enemies. As well as Route 
    //Construction for police, useful.
    public Transform GetNearestPath(int testI, int testJ)
    {
        Transform nearestPath = null;
        if (testI < (gridSize - 1) && testJ < (gridSize - 1) && testI > 0 && testJ > 0)
        {
            if (pathGrid[testI, testJ] != null)
            {
                nearestPath = pathGrid[testI, testJ];
                //If we didn't hit it in one...
                if (pathGrid[testI, testJ].tag != "Path")
                {
                    //Check for a path above
                    if (pathGrid[testI - 1, testJ] != null)
                    {
                        if (pathGrid[testI - 1, testJ].tag == "Path")
                        {
                            nearestPath = pathGrid[testI - 1, testJ];
                        }
                    }
                    //Check for one to the left
                    if (pathGrid[testI, testJ - 1] != null)
                    {
                        if (pathGrid[testI, testJ - 1].tag == "Path")
                        {
                            nearestPath = pathGrid[testI, testJ - 1];
                        }
                    }
                    //Check for one to the right
                    if (pathGrid[testI, testJ + 1] != null)
                    {
                        if (pathGrid[testI, testJ + 1].tag == "Path")
                        {
                            nearestPath = pathGrid[testI, testJ + 1];
                        }
                    }
                    //And check underneath
                    if (pathGrid[testI + 1, testJ] != null)
                    {
                        if (pathGrid[testI + 1, testJ].tag == "Path")
                        {
                            nearestPath = pathGrid[testI + 1, testJ];
                        }
                    }
                }
            }
        }
        if (nearestPath == null)
        {
            //Debug.Log("Didn't find path!");
        }
        return nearestPath;
    }


    //I believe the police call this one themselves. I'll explain this in more detail later, but if we can find a more consistent way of keeping track of
    //The grid offset then we could simply pass the current position of the police transform and use the offset to translate it to the pathGrid.
    //I feel like I answered my own question here, you could use those gridOffset variables I bet.
    //Right now it searches the whole grid, which, as you may have guessed is very slow.
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
        return new int[] { enemyI, enemyJ };
    }

    //Old player square search. Positives: You don't need to worry about the grid offset, no room for error. Negatives: Very Slow.
    public void SetPlayerSquare(GameObject playerSquare)
    {
        if (playerSquare != null)
        {
            //This is the slow part
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //Debug.Log("I: " + i + " J: " + j + " object = " + pathGrid[i, j].gameObject);
                    if (pathGrid[i, j] != null)
                    {
                        if (pathGrid[i, j].gameObject == playerSquare)
                        {
                            playerI = i;
                            playerJ = j;
                        }
                    }
                }
            }
            //This part is critical. Whenever we determine where the player is, we must use that information to update the grid or not.
            //Vision limit is 15, which we approximate to be the extent of the camera's actual view of the existing grid.
            //If the player approaches within the vision limit of any of the 4 edges, update the grid along that edge and one other. 
            //The secondary edge is determined by an arbitrary 50/50 split of the grid, whichever half it's on. This keeps it even if they travel straight 
            //toward the middle of an edge
            //Up
            if (playerI < visionLimit)
            {
                if (playerJ < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(1, 1));
                }
                else
                {
                    StartCoroutine(UpdateGrid(1, 0));
                }
            }
            //Down
            else if (playerI >= (gridSize - visionLimit))
            {
                if (playerJ < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(0, 0));
                }
                else
                {
                    StartCoroutine(UpdateGrid(0, 1));
                }
            }
            //Left
            else if (playerJ < visionLimit)
            {
                if (playerI < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(1, 1));
                }
                else
                {
                    StartCoroutine(UpdateGrid(0, 1));
                }
            }
            //Right
            else if (playerJ >= (gridSize - visionLimit))
            {
                if (playerI < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(1, 0));
                }
                else
                {
                    StartCoroutine(UpdateGrid(0, 0));
                }
            }
            //StartCoroutine(GridCull());
        }
        //GridDump();
    }

    //This is the new player square search. Right now it only works when the player spawns. The player passes an update and the function applies it to
    //the existing coordinates. The initial update is just the actual coordinates. Everything else should be pairs of 1's or 0's.
    //This is more efficient but it causes problems with the current set-up. The ongoing position of the player is not consistent with the pathGrid
    //perception of things. It needs to account for the offset in the negative for left or up expansion.
    public void SetPlayerSquare(int playerSquareI, int playerSquareJ)
    {
        if (playerSquareI >= 0 && playerSquareI < gridSize && playerSquareJ >= 0 && playerSquareJ < gridSize)
        {

            playerI += playerSquareI;
            playerJ += playerSquareJ;

            if (playerI < visionLimit)
            {
                if (playerJ < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(1, 1));
                }
                else
                {
                    StartCoroutine(UpdateGrid(1, 0));
                }
            }
            else if (playerI >= (gridSize - visionLimit))
            {
                if (playerJ < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(0, 0));
                }
                else
                {
                    StartCoroutine(UpdateGrid(0, 1));
                }
            }
            else if (playerJ < visionLimit)
            {
                if (playerI < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(1, 1));
                }
                else
                {
                    StartCoroutine(UpdateGrid(0, 1));
                }
            }
            else if (playerJ >= (gridSize - visionLimit))
            {
                if (playerI < (gridSize / 2))
                {
                    StartCoroutine(UpdateGrid(1, 0));
                }
                else
                {
                    StartCoroutine(UpdateGrid(0, 0));
                }
            }
            //StartCoroutine(GridCull());
        }
        //GridDump();
    }

    //This has problems for the same reason as above, but could technically be a slight improvement on the raycast if the offset is accounted for between
    //The real grid and pathGrid
    public bool CheckPlayerMove(GameObject currentPlayerSpace, int moveI, int moveJ)
    {
        SetPlayerSquare(currentPlayerSpace);
        bool moveLegal = true;
        if (pathGrid[playerI + moveI, playerJ + moveJ].tag != "Path")
        {
            //Debug.Log("pathGrid[" + (playerI + moveI) + " | " + (playerJ + moveJ) + "] is blocked");
            moveLegal = false;
        }
        return moveLegal;
    }

    //Oh boy, here we go. These are the majority of the code. These checks are a doozy. Any improvement to their efficiency or clarity is welcome, but you 
    //may find yourself making more rules to add to these already extensive beasts
    //As the name implies, there are times when a path is REQUIRED to be placed at a position. There may be a path adjacent to 2 walls, etc.
    bool CheckForRequiredPath(int currentI, int currentJ)
    {
        //I set this variable here and if it ever changes, it can't change back. An easy improvement you could make is adding returns so it doesn't run
        //unnecessary checks. If a path is required because of one side, it's required 100%. 
        bool pathRequired = false;

        //I won't do this for each of these but I'll give you an idea of how this works here
        //Check the space above.
        if (currentI - 1 >= 0)
        {
            //If it is a path
            if (pathGrid[currentI - 1, currentJ].tag == "Path")
            {
                //Call it's path properties function GetAdjacentBuildings, this number is updated with the previousUp check
                if (pathGrid[currentI - 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                {
                    //The space above is adjacent to 2 buildings, a path is required to avoid a dead end
                    pathRequired = true;
                }
            }
        }
        //Dead end avoidance: Left
        if (currentJ - 1 >= 0)
        {
            //Debug.Log("CurrentI = " + currentI + " CurrentJ = " + (currentJ - 1) + "  => " + pathGrid[currentI, currentJ - 1]);
            if (pathGrid[currentI, currentJ - 1].tag == "Path")
            {
                if (pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                {
                    pathRequired = true;
                }
            }
        }
        //Dead End avoidance: Right
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
        //Dead end avoidance: bottom
        if (currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ + 1] != null)
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
        //B: Building P: Path ?: Anything r: Path Required here
        //0|B|P|P|B| | | | | | or 0| |B|P|P|?| | | | | or 0| | | | | | |B|P|P| 
        //1| |r|?| | | | | | |    1| | |r|B| | | | | |    1| | | | | | | |r|B|

        if (currentI - 1 == 0 && currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI - 1, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag == "Path")
                {
                    if (pathGrid[currentI - 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
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
        //7| | | | | | | | |B| or 2| | | | | | | | |B| or 4| | | | | | | | |B| 
        //8| | | | | | | |r|P|    3| | | | | | | |r|P|    5| | | | | | | |r|P|
        //9| | | | | | | |B|P|    4| | | | | | | |B|P|    6| | | | | | | |B|P|
        if (currentI + 1 < gridSize && currentJ + 1 == gridSize - 1)
        {
            if (pathGrid[currentI, currentJ + 1] != null)
            {
                if (pathGrid[currentI, currentJ + 1].tag == "Path")
                {
                    if (pathGrid[currentI, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
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

    //This is to simply check if a path is even allowed. I say simply, but this one get's pretty hefty
    //x: illegal Path N: null space, not constructed yet
    bool CheckForLegalPath(int currentI, int currentJ)
    {
        bool pathLegal = true;

        //6| | | | | | | |P|P| 
        //7| | | | | | | |x|P|    
        //8| | | | | | | | | |   
        //These are all checks for 3 adjacent paths
        if (currentI - 1 >= 0)
        {
            if (currentJ + 1 < gridSize)
            {
                if (pathGrid[currentI - 1, currentJ + 1] != null && pathGrid[currentI, currentJ + 1] != null)
                {
                    if (pathGrid[currentI - 1, currentJ].tag == "Path" && pathGrid[currentI - 1, currentJ + 1].tag == "Path" && pathGrid[currentI, currentJ + 1].tag == "Path")
                    {
                        //Is this check necessary or even problematic?? I'm commenting it in case I forgot something
                        //if (pathGrid[currentI - 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                        //{
                        pathLegal = false;
                        //}
                    }
                }
                //6| | | | | | | |B| | 
                //7| | | | | | |P|P|B|    
                //8| | | | | | |x|N|N|
                //It's illegal here because it will be required later
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
        //2| | |B| | | | | | | or 1| | | | | |B| | | | or 3| | | | |B| | | | | 
        //3| |B|x| | | | | | |    2| | | | |B|x| | | |    4| | | |B|x| | | | | 
        //4|*|B|r| | | | | | |    3| | | |*|r|r| | | |    5| | |*|B|r| | | | |
        //5|B|P|r| | | | | | |    4| | | |B|P|r| | | |    6| | |?|P|r| | | | |
        //6| |?| | | | | | | |    5| | | | |B| | | | |    7| | | |B| | | | | |
        // ^ this case is actually fine...                  ^ and so is this one
        //This check needs some fixing, try checking the space to the right of * to make sure it's a path?
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
                                if (pathGrid[currentI + 2, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                                {
                                    pathLegal = false;
                                }
                            }
                        }
                    }
                    //2| | |B| | | | | | | 
                    //3| |B|x|r| | | | | |    
                    //4| | |r| | | | | | |    
                    //5| |P| | | | | | | |    
                    //6|B|P| | | | | | | |
                    //7| |B| | | | | | | |
                    //I'm not sure about this one, I'm sure my heart was in the right place. But it likely needs removal or rewriting...
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
        //2| | | |B| | | | | | 
        //3| | |B|P|P| |B| | |    
        //4| | | |P| | |x|B| |    
        //5| | | | | | | | | |    
        //6| | | | | | | | | |
        //7| | | | | | | | | |
        //This is the pattern above rotated 90 degrees clockwise. I think I'm more stringent here, so it might be ok.
        //But it turns out that some of these -3 might need to just be -2 and so on, I think I just made a mistake? Double check for me if you can
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
                    //2| | | | | | | | | | 
                    //3| | | | | | |B| | |    
                    //4| | | | | | |x|B| |    
                    //5| | | | | | | | | |    
                    //6| | | | | | | |P| |
                    //7| | | | | | |N|P|B|
                    //8| | | | | | | |B| |
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
        //This pattern needs to be rotated 90 degrees and mirror flipped to be 100% sure. I'm sure I missed spots, and judging by the others, I probably made other mistakes
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

        //5| | | | | | | | | |    
        //6| | | | |x|P| | | |
        //7| | | | | |P|B| | |
        //8| | | | | |B| | | |
        if (currentI + 1 < gridSize && currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ + 1] != null)
            {
                if (pathGrid[currentI + 1, currentJ + 1] != null)
                {
                    if (pathGrid[currentI, currentJ + 1].tag == "Path" && pathGrid[currentI + 1, currentJ + 1].tag == "Path")
                    {
                        if (pathGrid[currentI + 1, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                        {
                            pathLegal = false;
                        }
                    }
                }
            }
            //5| | | | | | | | | |    
            //6| | | | |x| | | | |
            //7| | | | |P|P|B| | |
            //8| | | | | |B| | | |
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




        //5| | | | | | | | | |    
        //6| | | | |P|P| | | |
        //7| | | | |x|P| | | |
        //8| | | | | | | | | |
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
                    if (currentJ + 1 < gridSize)
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

    //Ugh, so tired. Is building allowed? Call pathProperties on adjacent buildings and see how many it's adjacent to. It can't be more than 3!
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
            if (pathGrid[currentI, currentJ - 1] != null)
            {
                if (pathGrid[currentI, currentJ - 1].tag != "Path")
                {
                    if (pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 2)
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
                    if (pathGrid[currentI, currentJ + 1].GetComponent<PathProperties>().GetAdjacentBuildings() > 2)
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

        //5| | | | | | | | | |    
        //6| | | | | | | | | |
        //7| | |B|P|P|X| | | |
        //8| | | |B|B| | | | |
        //9| | | | | | | | | |
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
                                //HEY THIS IS IMPORTANT. I wrote a check to see if the adjacent buildings form a corner. I did it late so it might
                                //be useful in other checks I never bothered to implement it in
                                if (pathGrid[currentI, currentJ - 2].GetComponent<PathProperties>().GetCorner() == true)
                                {
                                    //Debug.Log("Corner 2 Left of " + currentI + ", " + currentJ);
                                    //HEY, THIS IS IMPORTANT TOO. I wrote another useful function in path properties.
                                    //It returns an array of 4 1's or 0's indicating the top, left, right, bottom space occcupiers respectively.
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
                                if (pathGrid[currentI, currentJ + 2].GetComponent<PathProperties>().GetCorner() == true)
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
            if (pathGrid[currentI + 2, currentJ] != null)
            {
                if (pathGrid[currentI + 1, currentJ].tag == "Path")
                {
                    //Debug.Log("I: " + currentI + " | J: " + currentJ + " check previous = " + pathGrid[currentI, currentJ - 1].GetComponent<PathProperties>().GetAdjacentBuildings());
                    if (pathGrid[currentI + 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 0)
                    {
                        //Debug.Log("I: " + currentI + " | J: " + currentJ + " can be a block");
                        if (pathGrid[currentI + 2, currentJ].tag == "Path")
                        {
                            //Debug.Log("I: " + currentI + " | J: " + currentJ + " might be a block");
                            if (pathGrid[currentI + 2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildings() > 1)
                            {
                                if (pathGrid[currentI + 2, currentJ].GetComponent<PathProperties>().GetCorner() == true)
                                {
                                    //Debug.Log("Corner 2 DOWN of " + currentI + ", " + currentJ);
                                    for (int k = 0; k < 4; k++)
                                    {
                                        //Debug.Log("Corner Code = " + pathGrid[currentI + 2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);
                                        //Debug.Log("Adjacent Code = " + pathGrid[currentI + 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k]);

                                        if (pathGrid[currentI + 2, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1 && pathGrid[currentI + 1, currentJ].GetComponent<PathProperties>().GetAdjacentBuildingsCode()[k] == 1)
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

    //This is what sets those previous variables for making checks and calling on PathProperties
    void SetPrevious(int currentI, int currentJ)
    {
        if (currentJ - 1 >= 0)
        {
            previousLeft = pathGrid[currentI, currentJ - 1].gameObject;
            previousLeftI = currentI;
            previousLeftJ = currentJ - 1;
        }
        else
        {
            previousLeft = null;
        }
        if (currentI - 1 >= 0)
        {
            previousTop = pathGrid[currentI - 1, currentJ].gameObject;
            previousTopI = currentI - 1;
            previousTopJ = currentJ;
        }
        else
        {
            previousTop = null;
        }
        if (currentJ + 1 < gridSize)
        {
            if (pathGrid[currentI, currentJ + 1] != null)
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
        if (currentI + 1 < gridSize)
        {
            if (pathGrid[currentI + 1, currentJ] != null)
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

    //You can play with this one. It mostly tries to make blocks and avoid long stringy rows.
    void SetBuildingChance(int currentI, int currentJ)
    {
        float chanceIncrement = 0.12f;
        float chanceDecay = 0.2f;
        if (currentI - 1 >= 0)
        {
            if (pathGrid[currentI - 1, currentJ] != null)
            {
                if (pathGrid[currentI - 1, currentJ].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                }
            }
            if (currentJ - 1 >= 0)
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
        if (currentJ - 1 >= 0)
        {
            if (pathGrid[currentI, currentJ - 1] != null)
            {
                if (pathGrid[currentI, currentJ - 1].tag != "Path")
                {
                    buildingChance += chanceIncrement;
                }
            }
            if (currentI - 1 >= 0)
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
        if (currentI - 1 >= 0 && currentJ - 1 >= 0 && set == false)
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

    //3 Checks, make each one, if both are legal leave it to chance. If none are legal, cheat and make a building. Comment out that else and you'll get an 
    //error in this case. Good for catching bad checks.
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
        SetupPreviousProperties(previousBottom, previousBottomI, previousBottomJ);
    }

    //Take the previous space and set how many and what buildings are adjacent to it.
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

    //Much like the player checks, this one is slower but works in the current system.
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
                if ((Mathf.Abs(i - playerI) + Mathf.Abs(j - playerJ)) / 2 > (visionLimit * 0.75f) || Mathf.Abs(i - playerI) > visionLimit || Mathf.Abs(j - playerJ) > visionLimit)
                {
                    pathGrid[i, j].GetComponent<MeshRenderer>().enabled = false;
                    //pathGrid[i, j].gameObject.SetActive(false);
                }
                else
                {
                    pathGrid[i, j].GetComponent<MeshRenderer>().enabled = true;
                }
                /*}*/
            }
        }
    }

    //I thought this would be faster but it's not good. No work.
    void UpdateGridCull()
    {
        for (int i = playerI - visionLimit; i < playerI + visionLimit; i++)
        {
            for (int j = playerJ - visionLimit; j < playerJ + visionLimit; j++)
            {
                if (i >= 0 && i < gridSize && j >= 0 && j < gridSize)
                {
                    if ((Mathf.Abs(i - playerI) + Mathf.Abs(j - playerJ)) / 2 > (visionLimit * 0.75f) || Mathf.Abs(i - playerI) > visionLimit || Mathf.Abs(j - playerJ) > visionLimit)
                    {
                        //pathGrid[i, j].GetComponent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        //pathGrid[i, j].GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
        }
    }

    //Just some getters.

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
        return playerCoordinates;
    }

    public int GetVisionLimit()
    {
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

    //A debug function. Call it to see the mental projection, pathGrid in a text form. Get a look under the hood.

    void GridDump()
    {
        string grid = "";
        string row = "";
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (pathGrid[i, j].gameObject.tag == "Path")
                {
                    row += "P";
                }
                else
                {
                    row += "X";
                }
                if (j < gridSize - 1)
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


