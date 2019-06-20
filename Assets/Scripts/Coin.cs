using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    LevelManager levelmanager;

    private void OnEnable()
    {
        levelmanager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            levelmanager.AddCoin();
            levelmanager.coinCount--;
            Destroy(gameObject);
        }
    }
}
