using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAddPath : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Path"))
        {
            other.gameObject.GetComponent<PathBuilder>().AttachNewPaths();
            this.GetComponentInParent<PathBuilder>().AttachNewPaths();
            Destroy(this.gameObject);
        }
    }
}
