using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConfigurationPanelController : MonoBehaviour
{
    public enum ConfigurationMode { ADD, SET };
    public GameObject commitButtonObject;
    public GameObject groundConfigurationPanel;
    public GameObject lightConfigurationPanel;
    public GameObject LogSmokeDensityConfigurationPanel;
    public GameObject BarrierConfigurationPanel;
    private ConfigurationMode configurationMode;
    private Data.type dataType;
    private string addButtonText = "ADD";
    private string setButtonText = "SET";

    void Start() {
        configurationMode = ConfigurationMode.SET;
        commitButtonObject.GetComponent<CommitButtonController>().setText(setButtonText);

    }

    public void showConfigurePanel(Data data, ConfigurationMode _configurationMode)
    {
        groundConfigurationPanel.SetActive(false);
        lightConfigurationPanel.SetActive(false);
        LogSmokeDensityConfigurationPanel.SetActive(false);
        BarrierConfigurationPanel.SetActive(false);

        dataType = data.dataType;
        switch (dataType)
        {
            case Data.type.GROUND: {
                groundConfigurationPanel.SetActive(true);
                groundConfigurationPanel.GetComponent<GroundConfigurationPanelController>().init((GroundData)data);
                break;
            }
            case Data.type.LIGHT: {
                lightConfigurationPanel.SetActive(true);
                lightConfigurationPanel.GetComponent<LightConfigurationPanelController>().init((LightData)data);
                break;
            }
            case Data.type.LOGDENSITY: {
                LogSmokeDensityConfigurationPanel.SetActive(true);
                LogSmokeDensityConfigurationPanel.GetComponent<LogDensityConfigurationPanelController>().init((LogSmokeDensityData)data);
                break;
            }
            case Data.type.BARRIER: {
                BarrierConfigurationPanel.SetActive(true);
                BarrierConfigurationPanel.GetComponent<BarrierConfigurationPanelController>().init((BarrierData)data);
                break;
            }
            default: {
                break;
            }
        }

        configurationMode = _configurationMode;
        switch (configurationMode)
        {
            case ConfigurationMode.ADD:
                commitButtonObject.GetComponent<CommitButtonController>().setText(addButtonText);
                break;
            case ConfigurationMode.SET:
                commitButtonObject.GetComponent<CommitButtonController>().setText(setButtonText);
                break;
            default:
                break;
        }
        
        
    }

    public void commitData()
    {
        Data data = null;
        switch(dataType) {
            case Data.type.GROUND: {
                data = groundConfigurationPanel.GetComponent<GroundConfigurationPanelController>().getData();
                break;
            }
            case Data.type.LIGHT: {
                data = lightConfigurationPanel.GetComponent<LightConfigurationPanelController>().getData();
                break;
            }
            case Data.type.LOGDENSITY: {
                data = LogSmokeDensityConfigurationPanel.GetComponent<LogDensityConfigurationPanelController>().getData();
                break;
            }
            case Data.type.BARRIER: {
                data = BarrierConfigurationPanel.GetComponent<BarrierConfigurationPanelController>().getData();
                break;
            }
            default : {
                break;
            }
        }

        if(data != null) {
            if (configurationMode == ConfigurationMode.SET) {
                GameManager.instance.setData(data);
            }
            else if(configurationMode == ConfigurationMode.ADD) {
                GameManager.instance.addObject(data);
            }
        }
    }
}
