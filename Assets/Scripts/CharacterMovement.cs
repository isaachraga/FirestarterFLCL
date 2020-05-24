using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    float speed = 1;
    GameObject[] MacroGrid;
    public GameObject NewPathGeneration;
    void Start()
    {
        NewPathGeneration = GameObject.FindGameObjectWithTag("Gen");
        CheckSpace();    
    }

    void Update()
    {
        //replace with buttons
        //if there isnt a building in the way, move in desired direction
        if (Input.GetKeyDown(KeyCode.W)){
            //CheckGrid();
            if (Raycast(0))
            {
                TurnOff();
                transform.position += transform.forward * speed;
            }
            CheckSpace();
            CheckToSpawn();
        }
        if (Input.GetKeyDown(KeyCode.A)){
            //CheckGrid();
            if (Raycast(1))
            {
                TurnOff();
                transform.position += transform.right * -speed;
            }
            CheckSpace();
            CheckToSpawn();
        }
        if (Input.GetKeyDown(KeyCode.S)){
            //CheckGrid();
            if (Raycast(2))
            {
                TurnOff();
                transform.position += transform.forward * -speed;
            }
            CheckSpace();
            CheckToSpawn();
        }
        if (Input.GetKeyDown(KeyCode.D)){
            //CheckGrid();
            if (Raycast(3))
            {
                TurnOff();
                transform.position += transform.right * speed;
                
            }
            CheckSpace();
            CheckToSpawn();
        }
    }

    void CheckToSpawn()
    {
        //FROMSPACEJAM
        //GameObject.FindGameObjectWithTag("Spawner").GetComponent<SpawnMasterScript>().EnemySpawn();
    }

    /*void CheckGrid()
    {
        MacroGrid = GameObject.FindGameObjectsWithTag("Macro");
        for(int i = 0; i < MacroGrid.Length; i++)
        {
            Vector3 dis =  MacroGrid[i].transform.position - transform.position;
            //Debug.Log(dis.sqrMagnitude);
            if(dis.sqrMagnitude > 160f)
            {
                for (int k = 0; k < MacroGrid.Length; k++)
                {

                    if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x - 9, 0, MacroGrid[i].transform.position.z + 9))
                    {
                        Debug.Log(1);
                        float TempX = MacroGrid[i].transform.position.x - 27f;
                        float TempY = MacroGrid[i].transform.position.z + 27f;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }
                    if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x - 9, 0, MacroGrid[i].transform.position.z))
                    {
                        Debug.Log(2);
                        float TempX = MacroGrid[i].transform.position.x - 27f;
                        float TempY = MacroGrid[i].transform.position.z;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }
                   if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x - 9, 0, MacroGrid[i].transform.position.z-9))
                    {
                        Debug.Log(3);
                        float TempX = MacroGrid[i].transform.position.x - 27f;
                        float TempY = MacroGrid[i].transform.position.z - 27f;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }


                    if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x + 9, 0, MacroGrid[i].transform.position.z - 9))
                    {
                        Debug.Log(4);
                        float TempX = MacroGrid[i].transform.position.x + 27f;
                        float TempY = MacroGrid[i].transform.position.z - 27f;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }
                    if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x + 9, 0, MacroGrid[i].transform.position.z))
                    {
                        Debug.Log(5);
                        float TempX = MacroGrid[i].transform.position.x + 27f;
                        float TempY = MacroGrid[i].transform.position.z;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }
                    if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x + 9, 0, MacroGrid[i].transform.position.z + 9))
                    {
                        Debug.Log(6);
                        float TempX = MacroGrid[i].transform.position.x + 27f;
                        float TempY = MacroGrid[i].transform.position.z + 27f;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }


                    if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x, 0, MacroGrid[i].transform.position.z-9))
                    {
                        Debug.Log(7);
                        float TempX = MacroGrid[i].transform.position.x;
                        float TempY = MacroGrid[i].transform.position.z - 27f;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }
                    if (MacroGrid[k].transform.position == new Vector3(MacroGrid[i].transform.position.x, 0, MacroGrid[i].transform.position.z + 9))
                    {
                        Debug.Log(8);
                        float TempX = MacroGrid[i].transform.position.x;
                        float TempY = MacroGrid[i].transform.position.z + 27f;
                        NewPathGeneration.GetComponent<NewPathGeneration>().GenerateMicroGrid((int)TempX, (int)TempY);
                        Destroy(MacroGrid[i]);
                        break;
                    }
                }
                       
                
                    
            }
        }
    }*/


    void CheckSpace()
    {
        Ray playerRay = new Ray(transform.position, -transform.up);
        RaycastHit playerHit;

        if (Physics.Raycast(playerRay, out playerHit))
        {
            if (playerHit.transform.tag == "Path")
            {
                //FROM SPACEJAM
                //GameObject.FindGameObjectWithTag("Grid").GetComponent<PathGenerationScript>().SetPlayerSquare(playerHit.transform.gameObject);
            }
        }
    }

    void TurnOff()
    {
        if(GetComponentInChildren<Lighter>() != null)
        {
            GetComponentInChildren<Lighter>().TurnOff();
        }
        if (GetComponentInChildren<FireBomb>() != null)
        {
            GetComponentInChildren<FireBomb>().TurnOff();
        }
       
        
    }

    bool Raycast(float direction)
    {
        //checks for buildings in specified direction
        if(direction == 0)
        {
            Ray playerRay = new Ray(transform.position, transform.forward);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building") || playerHit.transform.CompareTag("Burnt") || playerHit.transform.CompareTag("FireFighter")){
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
                if (playerHit.transform.CompareTag("Building") || playerHit.transform.CompareTag("Burnt") || playerHit.transform.CompareTag("FireFighter")) {
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
                if (playerHit.transform.CompareTag("Building") || playerHit.transform.CompareTag("Burnt") || playerHit.transform.CompareTag("FireFighter")) {
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
                if (playerHit.transform.CompareTag("Building") || playerHit.transform.CompareTag("Burnt") || playerHit.transform.CompareTag("FireFighter"))
                {
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

    public Transform RaycastDown()
    {
        //checks players current loction
        Ray playerRay = new Ray(transform.position, -transform.up);
        RaycastHit playerHit;

        if(Physics.Raycast(playerRay, out playerHit))
        {
            if(playerHit.transform.GetComponent<PathBuilder>() != null)
            {
                Transform Location = playerHit.transform;
                return Location;
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
