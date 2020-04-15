using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    float speed = 1;
    [SerializeField] private LayerMask mask;


    void Update()
    {
        //replace with buttons
        //if there isnt a building in the way, move in desired direction
        if (Input.GetKeyDown(KeyCode.W)){
            if (Raycast(0))
            {
                TurnOff();
                transform.position += transform.forward * speed;
            }
            CheckSpace();
        }
        if (Input.GetKeyDown(KeyCode.A)){
            if (Raycast(1))
            {
                TurnOff();
                transform.position += transform.right * -speed;
            }
            CheckSpace();
        }
        if (Input.GetKeyDown(KeyCode.S)){
            if (Raycast(2))
            {
                TurnOff();
                transform.position += transform.forward * -speed;
            }
            CheckSpace();
        }
        if (Input.GetKeyDown(KeyCode.D)){
            if (Raycast(3))
            {
                TurnOff();
                transform.position += transform.right * speed;

            }
            
            CheckSpace();
        }
    }

    void CheckSpace()
    {
        Ray playerRay = new Ray(transform.position, -transform.up);
        RaycastHit playerHit;

        if (Physics.Raycast(playerRay, out playerHit))
        {
            if (playerHit.transform.tag == "Path")
            {
                GameObject.FindGameObjectWithTag("Grid").GetComponent<PathGenerationScript>().SetPlayerSquare(playerHit.transform.gameObject);
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

            if (Physics.Raycast(playerRay, out playerHit, .5f, mask))
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
