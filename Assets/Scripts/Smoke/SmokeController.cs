using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeController : MonoBehaviour
{
    public GameObject smokeSource;

    public GameObject barrierObject;
    
    private Dictionary<int, SmokeData> smokeObjectsData;
    private Dictionary<int, GameObject> smokeObjects;

    private bool barrierDirtyFlag;//为true时表示数据为dirty
    private bool windDirtyFlag;//为true时表示数据为dirty

    // Start is called before the first frame update
    void Start()
    {
        smokeObjectsData = new Dictionary<int, SmokeData>();
        smokeObjects = new Dictionary<int, GameObject>();
        barrierDirtyFlag = false;
        windDirtyFlag = false;
    }

    public void addObject(SmokeData data)
    {
        smokeObjectsData.Add(data.index, data);

        GameObject go = Instantiate(smokeSource, data.geometryData.position, Quaternion.identity, transform);
        FogController fogC = go.GetComponent<FogController>();
        fogC.Init(data);
        smokeObjects.Add(data.index, go);

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
        deleteObject(data);
        addObject(data);
    }

    public void deleteObject(SmokeData data)
    {
        smokeObjectsData.Remove(data.index);
        GameObject go = smokeObjects[data.index];
        smokeObjects.Remove(data.index);
        Destroy(go);
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

    public int getSmokeDensity(Vector3 position)
    {
        int density = 0;
        foreach(var item in smokeObjects) 
        {
            density += item.Value.GetComponent<FogController>().GetSmokeDensity(position);
        }
        return density;
    }

    public Dictionary<int, BarrierData> getBarrierData()
    {
        return barrierObject.GetComponent<BarrierController>().getBarrierData();
    }
}
