using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public Labyrinth lab;
    public Vector2 target;
    public LevelManager levelmanager;


    void Awake () {
		lab = GameObject.Find("LevelManager").GetComponent<Labyrinth>(); 
    }
}
