using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    //UI
    public Text Timer;
    public Text CoinsUI;
    //level management
    float timer;
    public int Coins { get; set; }
    public int coinCount;
    Labyrinth lab;
    //prefabs
    public GameObject warning;
    public GameObject Zombie;
    public GameObject Mummy;
    public GameObject Coin;
    //events and delegates
    public delegate void MethodContainer();
    public delegate void MethodContainer_2(float percent);
    public event MethodContainer ChasingOrder;
    public event MethodContainer_2 Increase_speed;
    //reference to player
    GameObject player;
    List<Vertex> vertices;
    public GameObject results;
    void Start () {

        InitializeLevel();
        SpawnEnemy(Zombie);
    }


    void InitializeLevel()
    {
        timer = 0f;
        player = GameObject.Find("Player");
        lab = gameObject.GetComponent<Labyrinth>();
        vertices = lab.sa.vertices;
        coinCount = 0;
        StartCoroutine("GenerateCoin", lab);
    }
	
    public void SaveResults(int reason)
    {
        //getting name from objects that passes from menu to level
        string name = GameObject.Find("Data").GetComponent<BetweenSceneData>().name; 
        DateTime date = DateTime.Now;
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(Application.dataPath + "/results.xml");
        XmlElement xRoot = xDoc.DocumentElement;
        XmlElement userElem = xDoc.CreateElement("user");
        XmlAttribute nameAttr = xDoc.CreateAttribute("name");
        XmlElement nameElem = xDoc.CreateElement("name");
        XmlElement coinsElem = xDoc.CreateElement("coins");
        XmlElement timeElem = xDoc.CreateElement("time");
        XmlElement dateElem = xDoc.CreateElement("date");
        XmlElement reasonElem = xDoc.CreateElement("reason");
        XmlText nameText = xDoc.CreateTextNode(name);
        XmlText coinsText = xDoc.CreateTextNode(CoinsUI.transform.GetChild(0).GetComponent<Text>().text);
        XmlText timeText = xDoc.CreateTextNode(Timer.transform.GetChild(0).GetComponent<Text>().text);
        XmlText dateText = xDoc.CreateTextNode(date.ToString());
        string reas="";
        switch(reason){
            case 0:
                reas = "Eaten by zombie";
                break;
            case 1:
                reas = "Killed by mummy";
                break;
            case 2:
                reas = "Quit game";
                break;
        }
        XmlText reasonText = xDoc.CreateTextNode(reas);
        nameElem.AppendChild(nameText);
        coinsElem.AppendChild(coinsText);
        timeElem.AppendChild(timeText);
        dateElem.AppendChild(dateText);
        reasonElem.AppendChild(reasonText);
        userElem.Attributes.Append(nameAttr);
        userElem.AppendChild(nameElem);
        userElem.AppendChild(coinsElem);
        userElem.AppendChild(timeElem);
        userElem.AppendChild(dateElem);
        userElem.AppendChild(reasonElem);
        xRoot.AppendChild(userElem);
        xDoc.Save(Application.dataPath + "/results.xml");
        ShowResultPanel(reason);
    }

    void ShowResultPanel(int reason)
    {
        results.SetActive(true);
        results.transform.GetChild(1).GetComponent<Text>().text = Coins.ToString();
        results.transform.GetChild(2).GetComponent<Text>().text = Timer.transform.GetChild(0).GetComponent<Text>().text;
        switch (reason)
        {
            case 0:
                results.transform.GetChild(3).GetComponent<Text>().text = "Zombie ate your brains.";
                break;
            case 1:
                results.transform.GetChild(3).GetComponent<Text>().text = "Mummy stole your coins and mummyfy you!";
                break;
            case 2:
                results.transform.GetChild(3).GetComponent<Text>().text = "You quit the game.";
                break;
        }

    }

    void SpawnEnemy(GameObject enemy)
    {
        Vertex pntToCreateEnemy = lab.sa.GetRandomVertex();
        while (Mathf.Abs(pntToCreateEnemy.x - player.transform.position.x) < 5 || Mathf.Abs(pntToCreateEnemy.y - player.transform.position.y) < 5)
        {
            pntToCreateEnemy = lab.sa.GetRandomVertex();
        }
        Instantiate(enemy, new Vector3(pntToCreateEnemy.x, pntToCreateEnemy.y), Quaternion.identity);
    }

    //ending the game depending on index: 0 - death by zombie, 1 - death by mummy, 2 - escape button pressed
    public void EndGame(int index)
    {
        SaveResults(index);
        Time.timeScale = 0f;
    }
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame(2);
        }
        TimerUpdateChange();
    }
    void TimerUpdateChange()
    {
        timer += Time.deltaTime;
        float actualTime;
        int mins = 0;
        actualTime = timer;
        if (timer > 60f)
        {
            while (actualTime > 60)
            {
                actualTime -= 60f;
                mins += 1;
            }
        }
        if (mins < 1)
            Timer.transform.GetChild(0).GetComponent<Text>().text = String.Format("{0:0.00}", actualTime) + " s";
        else
            Timer.transform.GetChild(0).GetComponent<Text>().text = mins.ToString() + " min " + String.Format("{0:0.00}", actualTime) + " s";
    }


    public void UpdateCoins()
    {
        CoinsUI.transform.GetChild(0).GetComponent<Text>().text = Coins.ToString();
    }
    public void AddCoin()
    {
        Coins += 1;
        UpdateCoins();
        switch (Coins)
        {
            case 5:
                SpawnEnemy(Zombie);
                break;
            case 10:
                SpawnEnemy(Mummy);
                break;
            case 20:
                StartCoroutine("Warning");
                ChasingOrder();
                break;
        }
        if (Coins > 20)
        {
            
            Increase_speed(0.05f);
        }
    }

    /// <summary>
    /// recursive generation of coins
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateCoin()
    {
        if(coinCount < 10)
        {
            coinCount += 1;
            Vertex point = lab.sa.GetRandomVertex();
            Instantiate(Coin, new Vector3(point.x, point.y), Quaternion.identity);   
        }
        yield return new WaitForSeconds(5);
        StartCoroutine("GenerateCoin");
    }
    /// <summary>
    /// After collecting 20 coins shows warning message
    /// </summary>
    /// <returns></returns>
    IEnumerator Warning()
    {
        warning.SetActive(true);
        yield return new WaitForSeconds(3);
        warning.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
