using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    GameObject player;
    Camera camera;

	void Start () {
        camera = gameObject.GetComponent<Camera>();
        player = GameObject.Find("Player");
	}
	
	void Update () {

        CheckZoom();
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
	}

    void CheckZoom()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus) && camera.orthographicSize > 3f)
        {
            camera.orthographicSize -= 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && camera.orthographicSize < 15f)
        {
            camera.orthographicSize += 0.5f;
        }
    }
}
