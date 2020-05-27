using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public ScoreData a;

    public static int highscore;

    

    void OnDestroy()
    {
        if (a.GetScore() > highscore) ;
        highscore = a.GetScore();

        PlayerPrefs.SetInt("highscore", highscore);
        PlayerPrefs.Save();
    }
}
