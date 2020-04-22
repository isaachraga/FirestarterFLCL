using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{

    //checks for burnt buildings
    int BurnType = 0;
    public int LighterAmmo;
    public int FireBombAmmo;
    
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (BurnType == 0)
            {
                Raycast(0);
            }
            if(BurnType == 1 && LighterAmmo > 0)
            {
                Lighter(0);
            }
            if (BurnType == 2 && FireBombAmmo > 0)
            {
                FireBomb(0);
            }

        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (BurnType == 0)
            {
                Raycast(1);
            }
            if (BurnType == 1 && LighterAmmo > 0)
            {
                Lighter(1);
            }
            if (BurnType == 2 && FireBombAmmo > 0)
            {
                FireBomb(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (BurnType == 0)
            {
                Raycast(2);
            }
            if (BurnType == 1 && LighterAmmo > 0)
            {
                Lighter(2);
            }
            if (BurnType == 2 && FireBombAmmo > 0)
            {
                FireBomb(2);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (BurnType == 0)
            {
                Raycast(3);
            }
            if (BurnType == 1 && LighterAmmo > 0)
            {
                Lighter(3);
            }
            if (BurnType == 2 && FireBombAmmo > 0)
            {
                FireBomb(3);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(BurnType < 2)
            {
                BurnType++;
                Debug.Log("BurnType " + BurnType);
            }
            else
            {
                BurnType = 0;
                Debug.Log("BurnType " + BurnType);
            }
        }
    }
    

    void Lighter(int direction)
    {
        GetComponentInChildren<AltFireRotation>().Rotation(direction, 0);
        //Debug.Log("Lighter");
        LighterAmmo--;
    }

    void FireBomb(int direction)
    {
        GetComponentInChildren<AltFireRotation>().Rotation(direction, 1);
        //Debug.Log("FireBomb");
        FireBombAmmo--;
    }

    void Raycast(int direction)
    {
        if (direction == 0)
        {
            Ray playerRay = new Ray(transform.position, transform.forward);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building"))
                {
                    playerHit.collider.gameObject.GetComponent<Burn>().BurnBuilding();
                }
                
            }
           
        }
        else if (direction == 3)
        {
            Ray playerRay = new Ray(transform.position, -transform.right);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building"))
                {
                    playerHit.collider.gameObject.GetComponent<Burn>().BurnBuilding();
                }
            }
            
        }
        else if (direction == 2)
        {
            Ray playerRay = new Ray(transform.position, -transform.forward);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building"))
                {
                    playerHit.collider.gameObject.GetComponent<Burn>().BurnBuilding();
                }
            }
           
        }
        else if (direction == 1)
        {
            Ray playerRay = new Ray(transform.position, transform.right);
            RaycastHit playerHit;

            if (Physics.Raycast(playerRay, out playerHit, .5f))
            {
                if (playerHit.transform.CompareTag("Building"))
                {
                    playerHit.collider.gameObject.GetComponent<Burn>().BurnBuilding();
                }
            }
            
        }
    }

   
}
