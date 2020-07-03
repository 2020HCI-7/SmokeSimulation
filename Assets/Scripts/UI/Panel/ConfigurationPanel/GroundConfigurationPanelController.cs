using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundConfigurationPanelController : ObjectConfiguration
{
    private GroundData groundData;

    private GameObject sizeSetLine;

    public void init(GroundData _groundData)
    {

        groundData = _groundData;

        sizeSetLine = transform.GetChild(0).gameObject;
        sizeSetLine.GetComponent<OneValueSetLineController>().init("size", groundData.size.ToString());
    }

    public override void set(string key, string value)
    {
        try
        {
            if (value != "")
            {
                switch (key)
                {
                    case "size":
                        {
                            groundData.size = float.Parse(value);
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

    public GroundData getData()
    {
        return groundData;
    }
}
