/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{

    public List<WalkPath> possiblePaths = new List<WalkPath>();

    [Space]

    public Transform previousBlock;

    [Space]

    

    [Header("Offsets")]
    public float walkPointOffset = .5f;

    public Vector3 GetWalkPoint()
    {
        return transform.position + -transform.forward * walkPointOffset + (transform.up * 1.5f) + transform.right * walkPointOffset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        Gizmos.DrawSphere(GetWalkPoint(), .1f);

        if (possiblePaths == null)
            return;

        foreach (WalkPath p in possiblePaths)
        {
            if (p.target == null)
                return;
            Gizmos.color = p.active ? Color.black : Color.clear;
            Gizmos.DrawLine(GetWalkPoint(), p.target.GetComponent<Walkable>().GetWalkPoint());
        }
    }
}

[System.Serializable]
public class WalkPath
{
    public Transform target;
    public bool active = true;
}*/



    

