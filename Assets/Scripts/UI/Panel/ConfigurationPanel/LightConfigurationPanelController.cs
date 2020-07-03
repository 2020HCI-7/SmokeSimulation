using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightConfigurationPanelController : ObjectConfiguration
{
    private LightData lightData;

    private GameObject intensitySetLine;
    private GameObject colorSetLine;

    public void init(LightData _lightData)
    {

        lightData = _lightData;

        intensitySetLine = transform.GetChild(0).gameObject;
        intensitySetLine.GetComponent<OneValueSetLineController>().init("intensity", lightData.intensity.ToString());

        string[] RGB = { "R", "G", "B" };
        string[] value = { lightData.color.r.ToString(), lightData.color.g.ToString(), lightData.color.b.ToString()};
        colorSetLine = transform.GetChild(1).gameObject;
        colorSetLine.GetComponent<ThreeValueSetLineController>().init(RGB, value);
    }

    public override void set(string key, string value)
    {
        try
        {
            if (value != "")
            {
                switch (key)
                {
                    case "intensity":
                        {
                            lightData.intensity = float.Parse(value);
                            break;
                        }
                    case "R":
                        {
                            Color color = new Color(float.Parse(value), lightData.color.g, lightData.color.b);
                            lightData.color = color;
                            break;
                        }
                    case "G":
                        {
                            Color color = new Color(lightData.color.r, float.Parse(value), lightData.color.b);
                            lightData.color = color;
                            break;
                        }
                    case "B":
                        {
                            Color color = new Color(lightData.color.r, lightData.color.g, float.Parse(value));
                            lightData.color = color;
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

    public LightData getData()
    {
        return lightData;
    }
}
