using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Enemy : Unit {

    public float speed;
    bool ableToGo;

    public Player player;
    public List<Vertex> path;
    public Vertex aim;
    Animator anim;
    public int randomSteps; //amount of steps enemy perform before random changing direction
    public bool isChasing; 


    void OnEnable () {
        speed = 1f;
        aim = new Vertex((int)transform.position.x, (int)transform.position.y);
        ableToGo = false;
        player = GameObject.Find("Player").GetComponent<Player>();
        levelmanager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        levelmanager.ChasingOrder += StartChasing;
        levelmanager.Increase_speed += Increase_Speed;
        GetPathRandom();
        isChasing = false;
        randomSteps = 5;
        anim = gameObject.GetComponent<Animator>();

        
	}
	
    void Increase_Speed(float percent)
    {
        speed = speed + speed * percent;
    }

    int Random()
    {
        System.Random r = new System.Random();
        int i = 3 + r.Next(5);
        return i;
    }

	void Update () {
        if (ableToGo) 
        Walk();
	}

    public void StartChasing()
    {
        isChasing = true;
    }

    public virtual void Kill()
    {
        levelmanager.EndGame(0);
        
    }

    void Walk()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime*speed);
        if(new Vector2(transform.position.x, transform.position.y) == target)
        {
            if (isChasing)
            {
                GetPathToPlayer();
            }
            else
            {
                if(randomSteps > 0)
                {
 
                    randomSteps--;
                    ChangeDir();
                }
                else
                {
                    GetPathRandom();
                    randomSteps = Random();
                }
            }
        }
    }

    void GetPathRandom()
    {
        System.Random rand = new System.Random();
        lab.sa.Annulate();
        Vertex current = lab.sa.FindVertex(aim.x, aim.y);
        current.valueMark = 0;
        Vertex end = null;
        while (end == null)
            end = lab.sa.FindVertex(rand.Next(lab.Width), rand.Next(lab.Height));
        lab.sa.Dejikstra(current, end);
        path = lab.sa.GetPath(end);
        aim = path.First();
        ChangeDir();
    }
    public void GetPathToPlayer()
    {
        lab.sa.Annulate();
        Vertex current = lab.sa.FindVertex(aim.x, aim.y);
        current.valueMark = 0;
        lab.sa.Dejikstra(current, lab.sa.FindVertex(player.currentCell.x, player.currentCell.y));
        path = lab.sa.GetPath(lab.sa.FindVertex(player.currentCell.x, player.currentCell.y));
        aim = path.First();
        ChangeDir();
    }
    public void Go()
    {
        ableToGo = true;
    }
    public void GetSa()
    {
        
        Vertex first = lab.sa.FindVertex(1, 1);
        first.valueMark = 0;
        lab.sa.Dejikstra(first, lab.sa.vertices.Last());
        path = lab.sa.GetPath();
        aim = path.First();

    }

    public void ChangeDir()
    {
        if (path.Last() != aim)
        {
            aim = path[path.IndexOf(aim) + 1];
        }
        else
        {
            GetPathRandom();
        }
        target = new Vector2(aim.x, aim.y);
        if (target.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1.45f, transform.localScale.y);
        }
        else if (target.x < transform.position.x)
            transform.localScale = new Vector3(1.45f, transform.localScale.y);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Kill();
        }
    }
}



