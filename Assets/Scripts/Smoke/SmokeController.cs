using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeController : MonoBehaviour
{
    public GameObject smokeSource;
    
    private Dictionary<int, SmokeData> smokeObjectsData;

    private bool barrierDirtyFlag;//为true时表示数据为dirty
    private bool windDirtyFlag;//为true时表示数据为dirty

    // Start is called before the first frame update
    void Start()
    {
        smokeObjectsData = new Dictionary<int, SmokeData>();
        barrierDirtyFlag = false;
        windDirtyFlag = false;
    }

    public void addObject(SmokeData data)
    {
        smokeObjectsData.Add(data.index, data);

        GameObject go = Instantiate(smokeSource, data.geometryData.position, Quaternion.identity);
        FogController fogC = go.GetComponent<FogController>();
        // Debug.Log(fogC);
        fogC.Init(data);

        switch (data.geometryData.geometryType)
        {
            case GeometryData.GeometryType.CUBE:
               
                break;
            case GeometryData.GeometryType.SPHERE:

                break;
            case GeometryData.GeometryType.CYLINDER:
               
                break;
            default:
                break;
        }
    }

    public void setObject(SmokeData data)
    {
        smokeObjectsData.Remove(data.index);
        addObject(data);
    }

    public void deleteObject(SmokeData data)
    {
        smokeObjectsData.Remove(data.index);
    }

    public void setFlagDirty(string key)
    {
        switch(key) {
            case "barrier":
                barrierDirtyFlag = true;
                break;
            case "wind":
                windDirtyFlag = true;
                break;
            default:
                break;
        }
    }
}
