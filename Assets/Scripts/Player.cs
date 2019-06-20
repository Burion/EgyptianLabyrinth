using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : Unit {

    public Vertex currentCell;
    bool isMoving;
    Animator anim;

    enum Direction
    {
        up,
        down,
        left,
        right
    }

	void Start () {
        PlayerInit();
    }
	void PlayerInit()
    {
        currentCell = lab.sa.vertices.First();
        isMoving = false;
        anim = gameObject.GetComponent<Animator>();
    }

	void Update () {
        CheckMove();
        transform.position = Vector2.MoveTowards(transform.position, new Vector2 (currentCell.x, currentCell.y), Time.deltaTime * 5);
    }


    /// <summary>
    /// checking if any key is pressed
    /// </summary>
    void CheckMove()
    {
        if (transform.position != new Vector3(currentCell.x, currentCell.y))
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (!isMoving)
        {
            
            if (Input.GetKey(KeyCode.W))
            {
                Move(Direction.up);
            }

            else if (Input.GetKey(KeyCode.D))
            {
                Move(Direction.right);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }

            else if (Input.GetKey(KeyCode.A))
            {
                Move(Direction.left);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }

            else if (Input.GetKey(KeyCode.S))
            {
                Move(Direction.down);
            }
            else
            anim.SetBool("moving", false);
        }
        else
        {
            anim.SetBool("moving", true);
        }
    }

    void Move(Direction dir)
    {
        switch ((int)dir)
        {
            case 0:                
                if (lab.sa.FindVertex(currentCell.x, currentCell.y + 1) != null)
                {
                    currentCell = lab.sa.FindVertex(currentCell.x, currentCell.y + 1);
                }
                break;

            case 1:
                if(lab.sa.FindVertex(currentCell.x, currentCell.y - 1) != null)
                {
                    currentCell = lab.sa.FindVertex(currentCell.x, currentCell.y - 1);
                }
                break;
            case 2:
                if (lab.sa.FindVertex(currentCell.x - 1, currentCell.y) != null)
                {
                    currentCell = lab.sa.FindVertex(currentCell.x - 1, currentCell.y);
                }
                break;

            case 3:
                if (lab.sa.FindVertex(currentCell.x + 1, currentCell.y) != null)
                {
                    currentCell = lab.sa.FindVertex(currentCell.x + 1, currentCell.y);
                }
                break;
        }
    }

}
