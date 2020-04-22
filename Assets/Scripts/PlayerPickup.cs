using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
   
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Lighter"))
        {
            this.transform.GetComponent<Check>().LighterAmmo++;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("FireBomb"))
        {
            this.transform.GetComponent<Check>().FireBombAmmo++;
            Destroy(other.gameObject);
        }
    }

}
