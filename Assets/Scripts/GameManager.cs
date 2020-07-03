using System.IO;
using System.Text;
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
        LightData lightData = new LightData(1, "Light", Data.type.LIGHT, false, new Color(1,1,1), 0f);
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
            // Time.timeScale = 1;
        }
        else {
            objectAddPanel.SetActive(true);
            ObjectConfigurationPanel.SetActive(true);
            // Time.timeScale = 0;
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
                SphereGeometryData sphereGeometryData = new SphereGeometryData(
                    new Vector3(1f, 0.5f, 0f),
                    0.5f
                );
                // Debug.Log(number);
                objectData = new BarrierData(number, "Barrier" + number.ToString(), Data.type.BARRIER, true, sphereGeometryData);
                break;
            }
            case Data.type.WIND: {
                number = number + 1;
                CubeGeometryData cubeGeometryData = new CubeGeometryData(
                    new Vector3(0f, 0.5f, 0f),
                    new Vector3(1f, 1f, 1f),
                    new Vector3(0f, 5f, 0f)
                );
                // Debug.Log(number);
                objectData = new WindData(number, "Wind" + number.ToString(), Data.type.WIND, true, cubeGeometryData, 1f, 0f);
                break;
            }
            case Data.type.SMOKE: {
                number = number + 1;
                ConeGeometryData coneGeometryData = new ConeGeometryData(
                    new Vector3(0f, 0.5f, 0f),
                    0.2f,
                    0.5f,
                    new Vector3(1f, 0f, 0f)
                );
                PhysicalData physicalData = new PhysicalData(
                    0.01f,
                    10.0f,
                    1000,
                    3f
                );
                objectData = new SmokeData(number, "Smoke" + number.ToString(), Data.type.SMOKE, true, coneGeometryData, physicalData, SmokeData.SmokeType.SMOKE, new Color(1f, 1f, 1f));
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
        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        string line = "";

        //write configuration
        line = "configuration" + "-" + "number=" + number.ToString();
        sw.Write(line);
        sw.Write("\n");

        //write data
        foreach (var item in data)
        {
            line = "data" + "-";
            line = line + "index=" + item.Key + "-";
            line = line + "name=" + item.Value.name + "-";
            line = line + "type=" + item.Value.dataTypeToString(item.Value.dataType) + "-";
            line = line + "deletable=" + item.Value.deletable.ToString() + "-";
            switch(item.Value.dataType) {
                case Data.type.GROUND: {
                    GroundData groundData = ((GroundData)item.Value);
                    line = line + "size=" + groundData.size;
                    break;
                }
                case Data.type.LIGHT: {
                    LightData lightData = ((LightData)item.Value);
                    line = line + "intensity=" + lightData.intensity.ToString() + "-";
                    line = line + "color:R=" + lightData.color.r.ToString() + ":G=" + lightData.color.g.ToString() + ":B=" + lightData.color.b.ToString();
                    break;
                }
                case Data.type.LOGDENSITY: {
                    LogSmokeDensityData logSmokeDensityData = ((LogSmokeDensityData)item.Value);
                    line = line + "logFlag=" + logSmokeDensityData.logFlag.ToString() + "-";
                    line = line + "interval=" + logSmokeDensityData.interval.ToString();
                    break;
                }
                case Data.type.BARRIER: {
                    BarrierData barrierData = ((BarrierData)item.Value);
                    line = line + "geometryType=" + barrierData.geometryData.getGeometryTypeStringArray()[(int)barrierData.geometryData.geometryType] + "-";
                    line = line + "position=" + barrierData.geometryData.position.ToString() + "-";
                    switch(barrierData.geometryData.geometryType) {
                        case GeometryData.GeometryType.CUBE: {
                            line = line + "size=" + ((CubeGeometryData)barrierData.geometryData).size.ToString() + "-";
                            line = line + "direction=" + ((CubeGeometryData)barrierData.geometryData).direction.ToString();
                            break;
                        }
                        case GeometryData.GeometryType.SPHERE: {
                            line = line + "r=" + ((SphereGeometryData)barrierData.geometryData).r.ToString();
                            break;
                        }
                        case GeometryData.GeometryType.CYLINDER: {
                            line = line + "r=" + ((CylinderGeometryData)barrierData.geometryData).r.ToString() + "-";
                            line = line + "height=" + ((CylinderGeometryData)barrierData.geometryData).height.ToString() + "-";
                            line = line + "direction=" + ((CylinderGeometryData)barrierData.geometryData).direction.ToString();
                            break;
                        }
                    }
                    break;
                }case Data.type.WIND: {
                    WindData windData = ((WindData)item.Value);
                    line = line + "intensity=" + windData.intensity.ToString() + "-";
                    line = line + "interference=" + windData.interference.ToString() + "-";
                    line = line + "geometryType=" + windData.geometryData.getGeometryTypeStringArray()[(int)windData.geometryData.geometryType] + "-";
                    line = line + "position=" + windData.geometryData.position.ToString() + "-";
                    switch (windData.geometryData.geometryType)
                    {
                        case GeometryData.GeometryType.CUBE: {
                            line = line + "size=" + ((CubeGeometryData)windData.geometryData).size.ToString() + "-";
                            line = line + "direction=" + ((CubeGeometryData)windData.geometryData).direction.ToString();
                            break;
                        }
                        case GeometryData.GeometryType.SPHERE: {
                            line = line + "r=" + ((SphereGeometryData)windData.geometryData).r.ToString();
                            break;
                        }
                        case GeometryData.GeometryType.CYLINDER: {
                            line = line + "r=" + ((CylinderGeometryData)windData.geometryData).r.ToString() + "-";
                            line = line + "height=" + ((CylinderGeometryData)windData.geometryData).height.ToString();
                            break;
                        }
                    }
                    break;
                }case Data.type.SMOKE: {
                    SmokeData smokeData = ((SmokeData)item.Value);
                    switch(smokeData.smokeType) {
                        case SmokeData.SmokeType.SMOKE: {
                            line = line + "smokeType=smoke-";
                            line = line + "duration=" + smokeData.physicalData.duration.ToString() + "-";
                            line = line + "maxNumber=" + smokeData.physicalData.maxNumber.ToString() + "-";
                            line = line + "particleSize=" + smokeData.physicalData.particleSize.ToString() + "-";
                            line = line + "speed=" + smokeData.physicalData.speed.ToString() + "-";
                            line = line + "color:R=" + smokeData.color.r.ToString() + ":G=" + smokeData.color.g.ToString() + ":B=" + smokeData.color.b.ToString() + "-";
                            line = line + "geometryType=" + smokeData.geometryData.getGeometryTypeStringArray()[(int)smokeData.geometryData.geometryType] + "-";
                            line = line + "position=" + smokeData.geometryData.position.ToString() + "-";
                            switch (smokeData.geometryData.geometryType)
                            {
                                case GeometryData.GeometryType.CYCLE: {
                                    line = line + "r=" + ((CycleGeometryData)smokeData.geometryData).r.ToString() + "-";
                                    line = line + "direction=" + ((CycleGeometryData)smokeData.geometryData).direction.ToString();
                                    break;
                                }
                                case GeometryData.GeometryType.CONE: {
                                    line = line + "r=" + ((ConeGeometryData)smokeData.geometryData).r.ToString() + "-";
                                    line = line + "height=" + ((ConeGeometryData)smokeData.geometryData).height.ToString() + "-";
                                    line = line + "direction=" + ((ConeGeometryData)smokeData.geometryData).direction.ToString();
                                    break;
                                }
                            }
                            break;
                        }
                        case SmokeData.SmokeType.FIRE: {
                            line = line + "smokeType=fire-";
                            break;
                        }
                        case SmokeData.SmokeType.EXPLOSION: {
                            line = line + "smokeType=explosion-";
                            break;
                        }
                    }
                    break;
                }
                default : {
                    break;
                }
            }
            sw.Write(line);
            sw.Write("\n");
        }
        
        sw.Flush();
        
        sw.Close();
        fs.Close();
    }

    public void load(string path)
    {
        //删除现有的object
        List<int> deleteIndex = new List<int>();
        foreach (var item in data) {
            if(item.Key > 2) {
                deleteIndex.Add(item.Key);
            }
        }
        foreach(var item in deleteIndex) {
            destroyObject(item);
        }
        

        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        string[] lineParts;
        //configuration
        line = sr.ReadLine();
        lineParts = line.Split('-');
        number = int.Parse(lineParts[1].Substring(lineParts[1].IndexOf('=') + 1));

        //ground
        line = sr.ReadLine();
        lineParts = line.Split('-');
        ((GroundData)data[0]).size = float.Parse(lineParts[5].Substring(lineParts[5].IndexOf('=') + 1));
        setData(data[0]);

        //light
        line = sr.ReadLine();
        lineParts = line.Split('-');
        ((LightData)data[1]).intensity = float.Parse(lineParts[5].Substring(lineParts[5].IndexOf('=') + 1));
        string[] RGB = lineParts[6].Split(':');
        int r = int.Parse(RGB[1].Substring(RGB[1].IndexOf('=') + 1));
        int g = int.Parse(RGB[2].Substring(RGB[2].IndexOf('=') + 1));
        int b = int.Parse(RGB[3].Substring(RGB[3].IndexOf('=') + 1));
        ((LightData)data[1]).color = new Color(r, g, b);
        setData(data[1]);

        //logDensity
        line = sr.ReadLine();
        lineParts = line.Split('-');
        ((LogSmokeDensityData)data[2]).logFlag = bool.Parse(lineParts[5].Substring(lineParts[5].IndexOf('=') + 1));
        ((LogSmokeDensityData)data[2]).interval = float.Parse(lineParts[6].Substring(lineParts[6].IndexOf('=') + 1));
        setData(data[2]);

        //data
        while ((line = sr.ReadLine()) != null)
        {
            lineParts = line.Split('-');
            Data newData = new Data(0, "", 0, false);
            newData.index = int.Parse(lineParts[1].Substring(lineParts[1].IndexOf('=') + 1));
            newData.name = lineParts[2].Substring(lineParts[2].IndexOf('=') + 1);
            newData.deletable = bool.Parse(lineParts[4].Substring(lineParts[4].IndexOf('=') + 1));
            switch(lineParts[3].Substring(lineParts[3].IndexOf('=') + 1)) {
                case "BARRIER": {
                    newData.dataType = Data.type.BARRIER;
                    BarrierData barrierData = null;
                    Vector3 position = vector3Parse(lineParts[6].Substring(lineParts[6].IndexOf('=') + 1));
                    string typeString = lineParts[5].Substring(lineParts[5].IndexOf('=') + 1);
                    switch(typeString) {
                        case "cube": {
                            Vector3 size = vector3Parse(lineParts[7].Substring(lineParts[7].IndexOf('=') + 1));
                            Vector3 direction = vector3Parse(lineParts[8].Substring(lineParts[8].IndexOf('=') + 1));
                            CubeGeometryData cubeGeometryData = new CubeGeometryData(position, size, direction);
                            barrierData = new BarrierData(newData.index, newData.name, newData.dataType, newData.deletable, cubeGeometryData);
                            addObject(barrierData);
                            break;
                        }
                        case "sphere": {
                            float radius = float.Parse((lineParts[7].Substring(lineParts[7].IndexOf('=') + 1)));
                            SphereGeometryData sphereGeometryData = new SphereGeometryData(position, radius);
                            barrierData = new BarrierData(newData.index, newData.name, newData.dataType, newData.deletable, sphereGeometryData);
                            addObject(barrierData);
                            break;
                        }
                        case "cylinder": {
                            float radius = float.Parse((lineParts[7].Substring(lineParts[7].IndexOf('=') + 1)));
                            float height = float.Parse((lineParts[8].Substring(lineParts[8].IndexOf('=') + 1)));
                            Vector3 direction = vector3Parse(lineParts[9].Substring(lineParts[9].IndexOf('=') + 1));
                            CylinderGeometryData cylinderGeometryData = new CylinderGeometryData(position, radius, height, direction);
                            barrierData = new BarrierData(newData.index, newData.name, newData.dataType, newData.deletable, cylinderGeometryData);
                            addObject(barrierData);
                            break;
                        }
                    }
                    break;
                }
                case "WIND": {
                    newData.dataType = Data.type.WIND;
                    WindData windData = null;
                    float intensity = float.Parse(lineParts[5].Substring(lineParts[5].IndexOf('=') + 1));
                    float interfence = float.Parse(lineParts[6].Substring(lineParts[6].IndexOf('=') + 1));
                    Vector3 position = vector3Parse(lineParts[8].Substring(lineParts[8].IndexOf('=') + 1));
                    string typeString = lineParts[7].Substring(lineParts[7].IndexOf('=') + 1);
                    switch(typeString) {
                        case "cube": {
                            Vector3 size = vector3Parse(lineParts[9].Substring(lineParts[9].IndexOf('=') + 1));
                            Vector3 direction = vector3Parse(lineParts[10].Substring(lineParts[10].IndexOf('=') + 1));
                            CubeGeometryData cubeGeometryData = new CubeGeometryData(position, size, direction);
                            windData = new WindData(newData.index, newData.name, newData.dataType, newData.deletable, cubeGeometryData, intensity, interfence);
                            addObject(windData);
                            break;
                        }
                        case "sphere": {
                            float radius = float.Parse((lineParts[9].Substring(lineParts[9].IndexOf('=') + 1)));
                            SphereGeometryData sphereGeometryData = new SphereGeometryData(position, radius);
                            windData = new WindData(newData.index, newData.name, newData.dataType, newData.deletable, sphereGeometryData, intensity, interfence);
                            addObject(windData);
                            break;
                        }
                        case "cylinder": {
                            float radius = float.Parse((lineParts[9].Substring(lineParts[9].IndexOf('=') + 1)));
                            float height = float.Parse((lineParts[10].Substring(lineParts[10].IndexOf('=') + 1)));
                            Vector3 direction = Vector3.zero;
                            CylinderGeometryData cylinderGeometryData = new CylinderGeometryData(position, radius, height, direction);
                            windData = new WindData(newData.index, newData.name, newData.dataType, newData.deletable, cylinderGeometryData, intensity, interfence);
                            addObject(windData);
                            break;
                        }
                    }
                    break;
                }
                case "SMOKE": {
                    newData.dataType = Data.type.SMOKE;
                    SmokeData smokeData = null;
                    string smokeTypeString = lineParts[5].Substring(lineParts[5].IndexOf('=') + 1);
                    switch(smokeTypeString) {
                        case "smoke": {
                            float duration = float.Parse(lineParts[6].Substring(lineParts[6].IndexOf('=') + 1));
                            int maxNumber = int.Parse(lineParts[7].Substring(lineParts[7].IndexOf('=') + 1));
                            float particleSize = float.Parse(lineParts[8].Substring(lineParts[8].IndexOf('=') + 1));
                            float speed = float.Parse(lineParts[9].Substring(lineParts[9].IndexOf('=') + 1));
                            RGB = lineParts[10].Split(':');
                            int r_value = int.Parse(RGB[1].Substring(RGB[1].IndexOf('=') + 1));
                            int g_value = int.Parse(RGB[2].Substring(RGB[2].IndexOf('=') + 1));
                            int b_value = int.Parse(RGB[3].Substring(RGB[3].IndexOf('=') + 1));
                            Color color = new Color(r_value, g_value, b_value);
                            Vector3 position = vector3Parse(lineParts[12].Substring(lineParts[12].IndexOf('=') + 1));
                            string typeString = lineParts[11].Substring(lineParts[11].IndexOf('=') + 1);
                            switch(typeString) {
                                case "cycle": {
                                    float radius = float.Parse((lineParts[13].Substring(lineParts[13].IndexOf('=') + 1)));
                                    Vector3 direction = vector3Parse(lineParts[14].Substring(lineParts[14].IndexOf('=') + 1));
                                    CycleGeometryData cycleGeometryData = new CycleGeometryData(position, radius, direction);
                                    smokeData = new SmokeData(newData.index, newData.name, newData.dataType, newData.deletable, cycleGeometryData, new PhysicalData(particleSize, duration, maxNumber, speed),SmokeData.SmokeType.SMOKE, color);
                                    addObject(smokeData);
                                    break;
                                }
                                case "cylinder": {
                                    float radius = float.Parse((lineParts[13].Substring(lineParts[13].IndexOf('=') + 1)));
                                    float height = float.Parse((lineParts[14].Substring(lineParts[14].IndexOf('=') + 1)));
                                    Vector3 direction = vector3Parse((lineParts[15].Substring(lineParts[15].IndexOf('=') + 1)));
                                    CylinderGeometryData cylinderGeometryData = new CylinderGeometryData(position, radius, height, direction);
                                    smokeData = new SmokeData(newData.index, newData.name, newData.dataType, newData.deletable, cylinderGeometryData, new PhysicalData(particleSize, duration, maxNumber, speed), SmokeData.SmokeType.SMOKE, color);
                                    addObject(smokeData);
                                    break;
                                }
                            }
                            break;
                        }
                        case "fire": {
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
                            smokeData = new SmokeData(smokeData.index, smokeData.name, Data.type.BARRIER, true, cycleGeometryData, physicalData, SmokeData.SmokeType.FIRE, new Color(0, 0, 0));
                            addObject(smokeData);
                            break;
                        }
                        case "explosion": {
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
                            smokeData = new SmokeData(smokeData.index, smokeData.name, Data.type.BARRIER, true, cycleGeometryData, physicalData, SmokeData.SmokeType.FIRE, new Color(0, 0, 0));
                            addObject(smokeData);
                            break;
                        }
                    }
                    break;
                }
    
            }
        }
    }

    private Vector3 vector3Parse(string vector3String)
    {
        vector3String = vector3String.Substring(1, vector3String.Length - 2);
        string[] vector3Values = vector3String.Split(',');
        return new Vector3(float.Parse(vector3Values[0]), float.Parse(vector3Values[1]), float.Parse(vector3Values[2]));
    }
}
