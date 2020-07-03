using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindConfigurationPanelController : ObjectConfiguration
{
    private WindData windData;

    private GameObject geometrySetLine;
    private GameObject positionSetLine;
    private GameObject sphereSetLine;
    private GameObject cylinderSetLine;
    private GameObject cubeSetLine;
    private GameObject physicalSetLine;

    public void init(WindData _windData)
    {
        windData = _windData;

        geometrySetLine = transform.GetChild(0).gameObject;
        bool[] value0 = { false, false, false };
        value0[(int)_windData.geometryData.geometryType] = true;
        geometrySetLine.GetComponent<ThreeTypeChooseLineController>().init("geometry", _windData.geometryData.getGeometryTypeStringArray(), value0);

        string[] key1 = { "p_x", "p_y", "p_z" };
        string[] value1 = { windData.geometryData.position.x.ToString(), windData.geometryData.position.y.ToString(), windData.geometryData.position.z.ToString() };
        positionSetLine = transform.GetChild(1).gameObject;
        positionSetLine.GetComponent<ThreeValueSetLineController>().init(key1, value1);

        sphereSetLine = transform.GetChild(2).gameObject;
        cylinderSetLine = transform.GetChild(3).gameObject;
        cubeSetLine = transform.GetChild(4).gameObject;
        sphereSetLine.SetActive(false);
        cylinderSetLine.SetActive(false);
        cubeSetLine.SetActive(false);

        switch (_windData.geometryData.geometryType)
        {
            case GeometryData.GeometryType.SPHERE:
           
                sphereSetLine.SetActive(true);
                sphereSetLine.GetComponent<OneValueSetLineController>().init("s_r", ((SphereGeometryData)_windData.geometryData).r.ToString());
                break;
            case GeometryData.GeometryType.CYLINDER:
                cylinderSetLine.SetActive(true);
                string[] key3 = { "c_r", "c_h" };
                string[] value3 = { ((CylinderGeometryData)_windData.geometryData).r.ToString(), ((CylinderGeometryData)_windData.geometryData).height.ToString() };
                cylinderSetLine.GetComponent<TwoValueSetLineController>().init(key3, value3);
                break;
            case GeometryData.GeometryType.CUBE:
                cubeSetLine.SetActive(true);
                string[] key4 = { "length", "width", "height" };
                string[] value4 = { ((CubeGeometryData)_windData.geometryData).size.x.ToString(), ((CubeGeometryData)_windData.geometryData).size.y.ToString(), ((CubeGeometryData)_windData.geometryData).size.z.ToString() };
                cubeSetLine.GetComponent<ThreeValueSetLineController>().init(key4, value4);
                break;
            default:
                break;
        }

        physicalSetLine = transform.GetChild(5).gameObject;
        string[] key5 = { "intensity", "interfence" };
        string[] value5 = { windData.intensity.ToString(), windData.interference.ToString() };
        physicalSetLine.GetComponent<TwoValueSetLineController>().init(key5, value5);
    }

    public override void set(string key, string value)
    {
        // Debug.Log(key);
        // Debug.Log(value);
        try
        {
            if (value != "")
            {
                switch (key)
                {
                    case "geometry":
                        {
                            if (value == "cube")
                            {
                                CubeGeometryData cubeGeometryData = new CubeGeometryData(
                                    new Vector3(0f, 0.5f, 0f),
                                    new Vector3(1f, 1f, 1f),
                                    new Vector3(0f, 5f, 0f)
                                );
                                WindData newWindData = new WindData(windData.index, windData.name, Data.type.BARRIER, true, cubeGeometryData, 1f, 0f);
                                init(newWindData);
                            }
                            else if (value == "sphere")
                            {
                                SphereGeometryData sphereGeometryData = new SphereGeometryData(
                                    new Vector3(0f, 0.5f, 0f),
                                    0.2f
                                );
                                WindData newWindData = new WindData(windData.index, windData.name, Data.type.BARRIER, true, sphereGeometryData, 1f, 0f);
                                init(newWindData);
                            }
                            else if (value == "cylinder")
                            {
                                CylinderGeometryData cylinderGeometryData = new CylinderGeometryData(
                                    new Vector3(0f, 3f, 0f),
                                    0.2f,
                                    0.4f,
                                    new Vector3(0f, 0f, 0f)
                                );
                                WindData newWindData = new WindData(windData.index, windData.name, Data.type.BARRIER, true, cylinderGeometryData, 1f, 0f);
                                init(newWindData);
                            }

                            break;
                        }
                    //key1 { "p_x", "p_y", "p_z" }
                    case "p_x":
                        {
                            Vector3 position = new Vector3(float.Parse(value), windData.geometryData.position.y, windData.geometryData.position.z);
                            windData.geometryData.position = position;
                            break;
                        }
                    case "p_y":
                        {
                            Vector3 position = new Vector3(windData.geometryData.position.x, float.Parse(value), windData.geometryData.position.z);
                            windData.geometryData.position = position;
                            break;
                        }
                    case "p_z":
                        {
                            Vector3 position = new Vector3(windData.geometryData.position.x, windData.geometryData.position.y, float.Parse(value));
                            windData.geometryData.position = position;
                            break;
                        }
                    //key2 s_r
                    case "s_r":
                        {
                            ((SphereGeometryData)windData.geometryData).r = float.Parse(value);
                            break;
                        }
                    //key3 { "c_r", "c_h" }
                    case "c_r":
                        {
                            ((CylinderGeometryData)windData.geometryData).r = float.Parse(value);
                            break;
                        }
                    case "c_h":
                        {
                            ((CylinderGeometryData)windData.geometryData).height = float.Parse(value);
                            break;
                        }
                    //key4 { "length", "width", "height" }
                    case "length":
                        {
                            ((CubeGeometryData)windData.geometryData).size.x = float.Parse(value);
                            break;
                        }
                    case "width":
                        {
                            ((CubeGeometryData)windData.geometryData).size.z = float.Parse(value);
                            break;
                        }
                    case "height":
                        {
                            ((CubeGeometryData)windData.geometryData).size.y = float.Parse(value);
                            break;
                        }
                    //key5 { "intensity", "interfence" }
                    case "intensity":
                        {
                            windData.intensity = float.Parse(value);
                            break;
                        }
                    case "interfence":
                        {
                            windData.interference = float.Parse(value);
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

    public WindData getData()
    {
        // Debug.Log(windData.geometryData.geometryType);
        return windData;
    }
}
