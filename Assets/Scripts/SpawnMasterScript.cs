using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMasterScript : MonoBehaviour
{
    [SerializeField] GameObject gridManager;

    [SerializeField] Transform player;
    [SerializeField] Transform police;
    [SerializeField] Transform fireFighter;

    [SerializeField] float fireFighterToPoliceRatio = 2;

    [SerializeField] int enemyDensityFactor = 20;

    [SerializeField] bool startSetupComplete;

    int activeEnemies;
    int activeFireFighters;
    int activePolice;
    int enemyActiveLimit;
    int gridSize;

    int initialPlayerI;
    int initialPlayerJ;

    Transform[,] pathGridCopy;

    // Start is called before the first frame update
    void Start()
    {
        startSetupComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartingSetup()
    {
        PlayerSpawn();
        EnemySpawn();
        startSetupComplete = true;
    }

    void PlayerSpawn()
    {
        pathGridCopy = gridManager.GetComponent<PathGenerationScript>().GetPathGrid();
        gridSize = gridManager.GetComponent<PathGenerationScript>().GetGridSize();

        int playerSpawnI = Mathf.FloorToInt(gridSize / 2);
        int playerSpawnJ = Mathf.FloorToInt(gridSize / 2);

        initialPlayerI = playerSpawnI;
        initialPlayerJ = playerSpawnJ;

        if (pathGridCopy[playerSpawnI, playerSpawnJ].gameObject.tag == "Path")
        {
            Instantiate<Transform>(player.transform, new Vector3(playerSpawnI, 0, playerSpawnJ), Quaternion.identity);
        }
        else if (pathGridCopy[playerSpawnI, playerSpawnJ - 1].gameObject.tag == "Path")
        {
            Instantiate<Transform>(player.transform, new Vector3(playerSpawnI, 0, playerSpawnJ - 1), Quaternion.identity);
        }
        else if (pathGridCopy[playerSpawnI - 1, playerSpawnJ].gameObject.tag == "Path")
        {
            Instantiate<Transform>(player.transform, new Vector3(playerSpawnI - 1, 0, playerSpawnJ), Quaternion.identity);
        }
        else if (pathGridCopy[playerSpawnI, playerSpawnJ + 1].gameObject.tag == "Path")
        {
            Instantiate<Transform>(player.transform, new Vector3(playerSpawnI, 0, playerSpawnJ + 1), Quaternion.identity);
        }
        else if (pathGridCopy[playerSpawnI + 1, playerSpawnJ].gameObject.tag == "Path")
        {
            Instantiate<Transform>(player.transform, new Vector3(playerSpawnI + 1, 0, playerSpawnJ), Quaternion.identity);
        }
    }

    public void EnemySpawn()
    {
        pathGridCopy = gridManager.GetComponent<PathGenerationScript>().GetPathGrid();
        gridSize = gridManager.GetComponent<PathGenerationScript>().GetGridSize();

        int playerSquareI = gridManager.GetComponent<PathGenerationScript>().GetPlayerCoordinates()[0];
        //Debug.Log(playerSquareI + "psi");
        int playerSquareJ = gridManager.GetComponent<PathGenerationScript>().GetPlayerCoordinates()[1];

        if (startSetupComplete == false)
        {
            playerSquareI = initialPlayerI;
            playerSquareJ = initialPlayerJ;
        }

        //Debug.Log("enemy spawn playerLocation: i = " + playerSquareI + ", j = " + playerSquareJ);

        int spawnVisionLimit = gridManager.GetComponent<PathGenerationScript>().GetVisionLimit();

        enemyActiveLimit = Mathf.FloorToInt(gridSize / enemyDensityFactor);

        int[] cornerIList = { playerSquareI-(spawnVisionLimit-1), playerSquareI - (spawnVisionLimit - 1), playerSquareI + (spawnVisionLimit - 2), playerSquareI + (spawnVisionLimit - 2) };
        int[] cornerJList = { playerSquareJ-(spawnVisionLimit-1), playerSquareJ + (spawnVisionLimit - 2), playerSquareJ - (spawnVisionLimit - 1), playerSquareJ + (spawnVisionLimit - 2) };

        int policeLimit = Mathf.FloorToInt(enemyActiveLimit / (fireFighterToPoliceRatio+1));
        int fireFighterLimit = Mathf.FloorToInt(enemyActiveLimit-policeLimit);

        int spawnCornerIndex = Random.Range(0, 4);
        
        while (activeEnemies < enemyActiveLimit)
        {
            for(int i = 0; i < cornerIList.Length; i++)
            {
                //Debug.Log(cornerIList[i] + "cli");
            }
            for (int i = 0; i < cornerIList.Length; i++)
            {
                //Debug.Log(cornerJList[i] + "clj");
            }
            int enemySpawnI = cornerIList[spawnCornerIndex];
            int enemySpawnJ = cornerJList[spawnCornerIndex];


            //Debug.Log("ENEMY spawn: I = " + enemySpawnI + " | J = " + enemySpawnJ + " and CORNER INDEX = " + spawnCornerIndex);

            Transform enemySpawnLocation = gridManager.GetComponent<PathGenerationScript>().GetNearestPath(enemySpawnI, enemySpawnJ);
            /*
            if (pathGridCopy[enemySpawnI, enemySpawnJ].gameObject.tag != "Path")
            {
                if (pathGridCopy[enemySpawnI, enemySpawnJ - 1].gameObject.tag == "Path")
                {
                    enemySpawnLocation = pathGridCopy[enemySpawnI, enemySpawnJ - 1];
                }
                else if (pathGridCopy[enemySpawnI - 1, enemySpawnJ].gameObject.tag == "Path")
                {
                    enemySpawnLocation = pathGridCopy[enemySpawnI-1, enemySpawnJ];
                }
                else if (pathGridCopy[enemySpawnI, enemySpawnJ + 1].gameObject.tag == "Path")
                {
                    enemySpawnLocation = pathGridCopy[enemySpawnI, enemySpawnJ + 1];
                }
                else if (pathGridCopy[enemySpawnI + 1, enemySpawnJ].gameObject.tag == "Path")
                {
                    enemySpawnLocation = pathGridCopy[enemySpawnI+1, enemySpawnJ];
                }
            }*/

            if (activeFireFighters < fireFighterLimit)
            {
                //spawn firefighters
                //Debug.Log(fireFighter.transform.name);
                //Debug.Log(enemySpawnLocation.position);
                Instantiate<Transform>(fireFighter.transform, new Vector3(enemySpawnLocation.position.x, 0, enemySpawnLocation.position.z), Quaternion.identity);
                spawnCornerIndex += 1;
                activeFireFighters += 1;
            }
            else if (activePolice < policeLimit)
            {
                //spawn police
                //Debug.Log(police.transform.name);
                //Debug.Log(enemySpawnLocation.position);
                Instantiate<Transform>(police.transform, new Vector3(enemySpawnLocation.position.x, 0, enemySpawnLocation.position.z), Quaternion.identity);
                spawnCornerIndex += 1;
                activePolice += 1;
            }
            else
            {
                break;
            }

            activeEnemies = activeFireFighters + activePolice;

            if (spawnCornerIndex > 3)
            {
                spawnCornerIndex = 0;
            }
        }
    }

    public void SetDensityFactor(int newDensityFactor)
    {
        enemyDensityFactor = newDensityFactor;
    }
}
