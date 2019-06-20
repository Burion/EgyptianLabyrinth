using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mummy : Enemy {

    private void Start()
    {
        speed = 2f;
    }
    public override void Kill()
    {

        levelmanager.Coins = 0;
        levelmanager.UpdateCoins();
        levelmanager.EndGame(1);
    }

}
