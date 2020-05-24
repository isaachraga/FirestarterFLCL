using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathProperties : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject topAdjacent = null;
    [SerializeField] GameObject leftAdjacent = null;
    [SerializeField] GameObject rightAdjacent = null;
    [SerializeField] GameObject bottomAdjacent = null;
    [SerializeField] int adjacentBuildings = 0;


    public void Setup(GameObject top, GameObject left, GameObject right, GameObject bottom)
    {
        adjacentBuildings = 0;
        topAdjacent = top;
        leftAdjacent = left;
        rightAdjacent = right;
        bottomAdjacent = bottom;
        if (topAdjacent != null)
        {
            if (topAdjacent.tag != "Path")
            {
                adjacentBuildings += 1;
            }
        }
        if (leftAdjacent != null)
        {
            if (leftAdjacent.tag != "Path")
            {
                adjacentBuildings += 1;
            }
        }
        if (rightAdjacent != null)
        {
            if (rightAdjacent.tag != "Path")
            {
                adjacentBuildings += 1;
            }
        }
        if (bottomAdjacent != null)
        {
            if (bottomAdjacent.tag != "Path")
            {
                adjacentBuildings += 1;
            }
        }
    }

    public int GetAdjacentBuildings()
    {
        return adjacentBuildings;
    }

    public GameObject GetTop()
    {
        return topAdjacent;
    }

    public GameObject GetLeft()
    {
        return leftAdjacent;
    }

    public GameObject GetRight()
    {
        return rightAdjacent;
    }

    public GameObject GetBottom()
    {
        return bottomAdjacent;
    }

    public int[] GetAdjacentBuildingsCode()
    {
        int[] adjacentCode = new int[4];
        adjacentCode[0] = 0;
        adjacentCode[1] = 0;
        adjacentCode[2] = 0;
        adjacentCode[3] = 0;
        if (topAdjacent != null)
        {
            if (topAdjacent.tag != "Path")
            {
                adjacentCode[0] = 1;
            }
        }
        if (leftAdjacent != null)
        {
            if (leftAdjacent.tag != "Path")
            {
                adjacentCode[1] = 1;
            }
        }
        if (rightAdjacent != null)
        {
            if (rightAdjacent.tag != "Path")
            {
                adjacentCode[2] = 1;
            }
        }
        if (bottomAdjacent != null)
        {
            if (bottomAdjacent.tag != "Path")
            {
                adjacentCode[3] = 1;
            }
        }
        return adjacentCode;
    }


    public bool GetCorner()
    {
        if (topAdjacent != null && leftAdjacent != null)
        {
            return true;
        }
        else if (topAdjacent != null && rightAdjacent != null)
        {
            return true;
        }
        else if (bottomAdjacent != null && leftAdjacent != null)
        {
            return true;
        }
        else if (bottomAdjacent != null && rightAdjacent != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
