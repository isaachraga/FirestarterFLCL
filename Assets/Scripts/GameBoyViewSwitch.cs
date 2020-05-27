using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoyViewSwitch : MonoBehaviour
{
    public bool GameView;
    // Start is called before the first frame update
    void Start()
    {
        GameView = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwitchView()
    {
        if (GameView)
        {
            GameView = false;
        } 
        else
        {
            GameView = true;
        }
            
    }

    void GameViewOn()
    {
        //turn everything on for the gameboy screen
    }
    void GameViewOff()
    {
        //turn everything off for the gameboy screen
    }
}
