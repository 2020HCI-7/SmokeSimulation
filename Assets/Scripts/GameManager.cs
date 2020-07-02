using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    //instance
    static public GameManager instance;

    //UI
    public GameObject logPanel;
    public GameObject objectPanel;
    public GameObject ground;
    private int currentIndex;

    //data
    private Dictionary<int, Data> data;

    //configuration
    private GameMode gameMode;
    public enum GameMode { OPERATE, OBSERVE };

    //lifetime
    void Awake()
    {
        //instance
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        //data
        data = new Dictionary<int, Data>();
        GroundData groundData = new GroundData(0, "Ground", Data.type.GROUND, false, 10.0f);
        data.Add(groundData.index, groundData);
        ground.GetComponent<GroundController>().setObject(groundData);

        //UI
        objectPanel.GetComponent<ObjectPanelController>().addObject(groundData);
        currentIndex = 0;

        //configuration
        gameMode = GameMode.OPERATE;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    //methods configuration
    public void changeMode(GameMode _gameMode)
    {
        //configuration
        gameMode = _gameMode;

        //log
        addLog("[mode] change mode to " + gameMode.ToString());
    }
    
    //methods object
    public void setData(Data objectData)
    {
        //data
        data[objectData.index] = objectData;

        //log
        string logString = "[Set] set " + objectData.dataTypeToString(objectData.dataType) + " ID:" + objectData.index.ToString() + "  Name：" + objectData.name;
        addLog(logString);

        //object
        switch(objectData.dataType) {
            case Data.type.GROUND: {
                ground.GetComponent<GroundController>().setObject((GroundData)objectData);
                break;
            }
            default : {
                addLog("[Error] 物品不存在");
                break;
            }
        }
    }

    public void addObject(Data objectData) 
    {
        //UI
        objectPanel.GetComponent<ObjectPanelController>().addObject(objectData);

        //log
        string logString = "[Add] add " + objectData.dataTypeToString(objectData.dataType) + " ID: " + objectData.index.ToString() + "  Name: " + objectData.name;
        addLog(logString);

        //object

        //data
        data.Add(objectData.index, objectData);
    }

    public void destroyObject(int index)
    {
        //UI
        objectPanel.GetComponent<ObjectPanelController>().destroyObject(index);
        Data objectData = data[index];
        
        //log
        if(objectData != null) {
            string logString = "[Delete] delete " + objectData.dataTypeToString(objectData.dataType) + " ID: " + objectData.index.ToString() + "  Name: " + objectData.name;
            addLog(logString);
        }

        //object


        //data
        data.Remove(index);
    }

    //methods UI
    public void addLog(string line) 
    {
        logPanel.GetComponent<LogPanelController>().addLine(line);
    }
    public void setCurrentIndex(int index)
    {
        currentIndex = index;
    }
}
