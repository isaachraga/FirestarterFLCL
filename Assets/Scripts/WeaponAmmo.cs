using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAmmo : MonoBehaviour
{
    GameObject Player;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.GetComponent<Check>().BurnType == 0)
        {
            text.text = "Unlimited";
        }
        if (Player.GetComponent<Check>().BurnType == 1)
        {
            text.text = Player.GetComponent<Check>().LighterAmmo.ToString();
        }
        if (Player.GetComponent<Check>().BurnType == 2)
        {
            text.text = Player.GetComponent<Check>().FireBombAmmo.ToString();
        }
    }
}
