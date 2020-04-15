using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltFireRotation : MonoBehaviour
{
    public GameObject Lighter, FireBomb;
    Vector3 OriginalRot;
    

    public void Rotation(int Direction, int AltFire)
    {

        OriginalRot = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, Direction * 90f, 0);
        

       // Debug.Log(Direction*90f);
       // Debug.Log(transform.eulerAngles);
        if(AltFire == 0)
         {

             Lighter.SetActive(true);

         }
         if (AltFire == 1)
         {
             FireBomb.SetActive(true);

         }
     
        



    }
    public void RotateBack()
    {
        transform.eulerAngles = OriginalRot;
    }
    
}
