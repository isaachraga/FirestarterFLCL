﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : MonoBehaviour
{
    int max = 0;

   

    private void OnTriggerStay(Collider other)
    {


        if (other.gameObject.CompareTag("Building"))
        {

            other.gameObject.GetComponent<Burn>().BurnBuilding();
            max++;

        }
        if (max >= 6)
        {
            
            max = 0;
            TurnOff();
        }

    }

    public void TurnOff()
    {
        max = 0;
        GetComponentInParent<AltFireRotation>().RotateBack();
        this.gameObject.SetActive(false);
    }
}
