using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetweenSceneData : MonoBehaviour {

    public string name;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("data");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
