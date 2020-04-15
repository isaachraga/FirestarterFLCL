using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreReset : MonoBehaviour
{
    public ScoreData a;
    // Start is called before the first frame update
    void Start()
    {
        a.ResetScore();
    }

   
}
