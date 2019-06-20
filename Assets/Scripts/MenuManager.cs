using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Xml;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Xml.Xsl;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour {

    public InputField input;
    public GameObject grid;
    public GameObject gridElement;
	void Start () {
        FillTableRecords();        
    }

    void FillTableRecords()
    {
        input.text = "Player";
        DateTime date = DateTime.Today;
        XElement root = XElement.Load(Application.dataPath + "/results.xml");
        var orderedtabs = root.Elements("user").OrderByDescending(xtab => (DateTime)xtab.Element("date")).ToArray();
        Debug.Log(orderedtabs.Length);
        orderedtabs.Reverse();
        root.RemoveAll();
        foreach (XElement tab in orderedtabs)
        {
            root.Add(tab);
        }
        root.Save(Application.dataPath + "/results.xml");
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(Application.dataPath + "/results.xml");
        XmlElement xRoot = xDoc.DocumentElement;
        foreach (XmlNode xnode in xRoot)
        {
            GameObject element = Instantiate(gridElement, grid.transform);

            // обходим все дочерние узлы элемента user
            foreach (XmlNode childnode in xnode.ChildNodes)
            {
                if (childnode.Name == "name")
                {
                    element.transform.GetChild(4).GetComponent<Text>().text = childnode.InnerText;
                }
                if (childnode.Name == "coins")
                {
                    element.transform.GetChild(5).GetComponent<Text>().text = childnode.InnerText;
                }
                if (childnode.Name == "time")
                {
                    element.transform.GetChild(6).GetComponent<Text>().text = childnode.InnerText;
                }
                if (childnode.Name == "date")
                {
                    element.transform.GetChild(7).GetComponent<Text>().text = childnode.InnerText;
                }
                if (childnode.Name == "reason")
                {
                    element.transform.GetChild(8).GetComponent<Text>().text = childnode.InnerText;
                }
            }

        }
    }

	public void LoadLevel()
    {
        GameObject data = GameObject.Find("Data");
        data.GetComponent<BetweenSceneData>().name = input.text;
        SceneManager.LoadScene("Level");
    }
    public void Quit()
    {
        Application.Quit();
    }

}
