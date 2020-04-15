using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScoreData")]

public class ScoreData : ScriptableObject
{
    public int Score = 10000000;
    
    public void ResetScore()
    {
        Score = 10000000;
    }
    public void AddScore()
    {
        Score += 100;
    }

    public int GetScore()
    {
        return Score;
    }
}
