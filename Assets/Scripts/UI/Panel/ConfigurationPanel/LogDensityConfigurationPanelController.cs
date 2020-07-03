using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogDensityConfigurationPanelController : ObjectConfiguration
{
    private LogSmokeDensityData logSmokeDensityData;

    private GameObject logFlagSetLine;
    private GameObject intervalSetLine;

    public void init(LogSmokeDensityData _logSmokeDensityData)
    {

        logSmokeDensityData = _logSmokeDensityData;

        logFlagSetLine = transform.GetChild(0).gameObject;
        logFlagSetLine.GetComponent<OneFlagChooseLine>().init("logFlag", logSmokeDensityData.logFlag.ToString());

        intervalSetLine = transform.GetChild(1).gameObject;
        intervalSetLine.GetComponent<OneValueSetLineController>().init("interval", logSmokeDensityData.interval.ToString());        
    }

    public override void set(string key, string value)
    {
        try
        {
            if (value != "")
            {
                switch (key)
                {
                    case "interval":
                        {
                            logSmokeDensityData.interval = float.Parse(value);
                            break;
                        }
                    case "logFlag":
                        {
                            logSmokeDensityData.logFlag = bool.Parse(value);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        catch (System.Exception)
        {

        }

    }

    public LogSmokeDensityData getData()
    {
        return logSmokeDensityData;
    }
}
