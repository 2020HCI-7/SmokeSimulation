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
    public GameObject objectAddPanel;
    public GameObject ObjectConfigurationPanel;
    public GameObject CommandLinePanel;
    private int currentIndex;

    //data
    private Dictionary<int, Data> data;
    private int number;

    //configuration
    private GameMode gameMode;
    public enum GameMode { OPERATE, OBSERVE };
    public float deltaTime;

    //object
    public GameObject groundObject;
    public GameObject lightObject;
    public GameObject smokeObject;
    public GameObject barrierObject;
    public GameObject windObject;

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
        GroundData groundData = new GroundData(0, "Ground", Data.type.GROUND, false, 4.0f);
        data.Add(groundData.index, groundData);
        groundObject.GetComponent<GroundController>().setObject(groundData);
        LightData lightData = new LightData(1, "Light", Data.type.LIGHT, false, new Color(255,255,255), 1.0f);
        data.Add(lightData.index, lightData);
        LogSmokeDensityData logSmokeDensityData = new LogSmokeDensityData(2, "SmokeDensity", Data.type.LOGDENSITY, false, false, 1.0f);
        data.Add(logSmokeDensityData.index, logSmokeDensityData);
        number = 2;

        //UI
        objectPanel.GetComponent<ObjectPanelController>().addObject(groundData);
        objectPanel.GetComponent<ObjectPanelController>().addObject(lightData);
        objectPanel.GetComponent<ObjectPanelController>().addObject(logSmokeDensityData);
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

    //methods
    public void changeMode(GameMode _gameMode)
    {
        //configuration
        gameMode = _gameMode;
        if(_gameMode == GameMode.OBSERVE) {
            objectAddPanel.SetActive(false);
            ObjectConfigurationPanel.SetActive(false);
        }
        else {
            objectAddPanel.SetActive(true);
            ObjectConfigurationPanel.SetActive(true);
        }

        //log
        addLog("[mode] change mode to " + gameMode.ToString());
    }
    
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
                groundObject.GetComponent<GroundController>().setObject((GroundData)objectData);
                break;
            }
            case Data.type.LIGHT: {
                lightObject.GetComponent<LightController>().setObject((LightData)objectData);
                break;
            }
            case Data.type.LOGDENSITY: {
                smokeObject.GetComponent<LogSmokeDensityController>().setObject((LogSmokeDensityData)objectData);
                break;
            }
            case Data.type.BARRIER: {
                barrierObject.GetComponent<BarrierController>().setObject((BarrierData)objectData);
                break;
            }case Data.type.WIND: {
                windObject.GetComponent<WindController>().setObject((WindData)objectData);
                break;
            }case Data.type.SMOKE: {
                smokeObject.GetComponent<SmokeController>().setObject((SmokeData)objectData);
                break;
            }
            default : {
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
        switch(objectData.dataType) {
            case Data.type.BARRIER: {
                barrierObject.GetComponent<BarrierController>().addObject((BarrierData)objectData);
                break;
            }
            case Data.type.WIND: {
                windObject.GetComponent<WindController>().addObject((WindData)objectData);
                break;
            }
            case Data.type.SMOKE: {
                smokeObject.GetComponent<SmokeController>().addObject((SmokeData)objectData);
                break;
            }
            default : {
                logString = "[Error] the " + objectData.dataTypeToString(objectData.dataType) + " object can't be added";
                addLog(logString);
                break;
            }
        }

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
        switch(objectData.dataType) {
            case Data.type.BARRIER: {
                barrierObject.GetComponent<BarrierController>().deleteObject((BarrierData)objectData);
                break;
            }
            case Data.type.WIND: {
                windObject.GetComponent<WindController>().deleteObject((WindData)objectData);
                break;
            }
            case Data.type.SMOKE: {
                smokeObject.GetComponent<SmokeController>().deleteObject((SmokeData)objectData);
                break;
            }
            default : {
                string logString = "[Error] the " + objectData.dataTypeToString(objectData.dataType) + " object can't be deleted";
                addLog(logString);
                break;
            }
        }

        //data
        data.Remove(index);
    }

    public void addLog(string line) 
    {
        logPanel.GetComponent<LogPanelController>().addLine(line);
    }

    public void toAddConfiguration(Data.type type)
    {
        // Debug.Log(type);
        Data objectData = null;
        switch(type) {
            case Data.type.BARRIER: {
                number = number + 1;
                CubeGeometryData cubeGeometryData = new CubeGeometryData(
                    new Vector3(0f, 0.5f, 0f),
                    new Vector3(0.2f, 0.2f, 0.2f),
                    new Vector3(0f, 0f, 0f)
                );
                // Debug.Log(number);
                objectData = new BarrierData(number, "Barrier" + number.ToString(), Data.type.BARRIER, true, cubeGeometryData);
                break;
            }
            case Data.type.WIND: {
                number = number + 1;
                CubeGeometryData cubeGeometryData = new CubeGeometryData(
                    new Vector3(0f, 0.5f, 0f),
                    new Vector3(0.2f, 0.2f, 0.2f),
                    new Vector3(0f, 0f, 0f)
                );
                // Debug.Log(number);
                objectData = new WindData(number, "Wind" + number.ToString(), Data.type.WIND, true, cubeGeometryData, 1f, 0f);
                break;
            }
            case Data.type.SMOKE: {
                number = number + 1;
                CycleGeometryData cycleGeometryData = new CycleGeometryData(
                    new Vector3(0f, 0.5f, 0f),
                    0.1f,
                    new Vector3(0f, 0f, 0f)
                );
                PhysicalData physicalData = new PhysicalData(
                    0.01f,
                    10.0f,
                    1000,
                    3f
                );
                objectData = new SmokeData(number, "Smoke" + number.ToString(), Data.type.SMOKE, true, cycleGeometryData, physicalData, SmokeData.SmokeType.SMOKE, new Color(0, 0, 0));
                break;
            }
            default : {
                string logString = "[Error] the " + new Data().dataTypeToString(type) + " object can't be added";
                addLog(logString);
                break;
            }
        }
        
        if (objectData != null) {
            // Debug.Log(objectData.dataType);
            ObjectConfigurationPanel.GetComponent<ObjectConfigurationPanelController>().showConfigurePanel(objectData, ObjectConfigurationPanelController.ConfigurationMode.ADD);
        }
    }
    
    public void setCurrentIndex(int index)
    {
        currentIndex = index;
        ObjectConfigurationPanel.GetComponent<ObjectConfigurationPanelController>().showConfigurePanel(data[index], ObjectConfigurationPanelController.ConfigurationMode.SET);
    }

    //file
    public void save(string path)
    {

    }

    public void load(string path)
    {

    }
}
