using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierConfigurationPanelController : ObjectConfiguration
{
    private BarrierData barrierData;

    private GameObject geometrySetLine;
    private GameObject positionSetLine;
    private GameObject sphereSetLine;
    private GameObject cylinderSetLine;
    private GameObject cubeSetLine;
    private GameObject directionSetLine;

    public void init(BarrierData _barrierData)
    {
        barrierData = _barrierData;

        geometrySetLine = transform.GetChild(0).gameObject;
        bool[] value0 = { false, false, false };
        value0[(int)_barrierData.geometryData.geometryType] = true;
        geometrySetLine.GetComponent<ThreeTypeChooseLineController>().init("geometry", _barrierData.geometryData.getGeometryTypeStringArray(), value0);

        string[] key1 = { "p_x", "p_y", "p_z" };
        string[] value1 = { barrierData.geometryData.position.x.ToString(), barrierData.geometryData.position.y.ToString(), barrierData.geometryData.position.z.ToString() };
        positionSetLine = transform.GetChild(1).gameObject;
        positionSetLine.GetComponent<ThreeValueSetLineController>().init(key1, value1);

        sphereSetLine = transform.GetChild(2).gameObject;
        cylinderSetLine = transform.GetChild(3).gameObject;
        cubeSetLine = transform.GetChild(4).gameObject;
        sphereSetLine.SetActive(false);
        cylinderSetLine.SetActive(false);
        cubeSetLine.SetActive(false);

        directionSetLine = transform.GetChild(5).gameObject;
        string[] key5 = new string[3];
        string[] value5 = new string[3];

        switch(_barrierData.geometryData.geometryType) {
            case GeometryData.GeometryType.SPHERE:
                directionSetLine.SetActive(false);
                sphereSetLine.SetActive(true);
                sphereSetLine.GetComponent<OneValueSetLineController>().init("s_r", ((SphereGeometryData)_barrierData.geometryData).r.ToString());
                break;
            case GeometryData.GeometryType.CYLINDER:
                directionSetLine.SetActive(true);
                key5[0] = "cy_x";
                key5[1] = "cy_y";
                key5[2] = "cy_z";
                value5[0] = ((CylinderGeometryData)_barrierData.geometryData).direction.x.ToString();
                value5[1] = ((CylinderGeometryData)_barrierData.geometryData).direction.y.ToString();
                value5[2] = ((CylinderGeometryData)_barrierData.geometryData).direction.z.ToString();
                directionSetLine.GetComponent<ThreeValueSetLineController>().init(key5, value5);

                cylinderSetLine.SetActive(true);
                string[] key3 = { "c_r", "c_h" };
                string[] value3 = { ((CylinderGeometryData)_barrierData.geometryData).r.ToString(), ((CylinderGeometryData)_barrierData.geometryData).height.ToString() };
                cylinderSetLine.GetComponent<TwoValueSetLineController>().init(key3, value3);
                break;
            case GeometryData.GeometryType.CUBE:
                cubeSetLine.SetActive(true);
                string[] key4 = { "length", "width", "height" };
                string[] value4 = { ((CubeGeometryData)_barrierData.geometryData).size.x.ToString(), ((CubeGeometryData)_barrierData.geometryData).size.y.ToString(), ((CubeGeometryData)_barrierData.geometryData).size.z.ToString() };
                cubeSetLine.GetComponent<ThreeValueSetLineController>().init(key4, value4);

                key5[0] = "cu_x";
                key5[1] = "cu_y";
                key5[2] = "cu_z";
                directionSetLine.SetActive(true);
                value5[0] = ((CubeGeometryData)_barrierData.geometryData).direction.x.ToString();
                value5[1] = ((CubeGeometryData)_barrierData.geometryData).direction.y.ToString();
                value5[2] = ((CubeGeometryData)_barrierData.geometryData).direction.z.ToString();
                directionSetLine.GetComponent<ThreeValueSetLineController>().init(key5, value5);
                break;
            default:
                break;
        }


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
                            if(value == "cube") {
                                CubeGeometryData cubeGeometryData = new CubeGeometryData(
                                    new Vector3(0f, 0.5f, 0f),
                                    new Vector3(0.2f, 0.2f, 0.2f),
                                    new Vector3(0f, 0f, 0f)
                                );
                                BarrierData newBarrierData = new BarrierData(barrierData.index, barrierData.name, Data.type.BARRIER, true, cubeGeometryData);
                                init(newBarrierData);
                            }
                            else if(value == "sphere") {
                                SphereGeometryData sphereGeometryData = new SphereGeometryData(
                                    new Vector3(0f, 0.5f, 0f),
                                    0.2f
                                );
                                BarrierData newBarrierData = new BarrierData(barrierData.index, barrierData.name, Data.type.BARRIER, true, sphereGeometryData);
                                init(newBarrierData);
                            }
                            else if(value == "cylinder") {
                                CylinderGeometryData cylinderGeometryData = new CylinderGeometryData(
                                    new Vector3(0f, 3f, 0f),
                                    0.2f,
                                    0.4f,
                                    new Vector3(0f, 0f, 0f)
                                );
                                BarrierData newBarrierData = new BarrierData(barrierData.index, barrierData.name, Data.type.BARRIER, true, cylinderGeometryData);
                                init(newBarrierData);
                            }
                            
                            break;
                        }
                    //key1 { "p_x", "p_y", "p_z" }
                    case "p_x":
                        {
                            Vector3 position = new Vector3(float.Parse(value), barrierData.geometryData.position.y, barrierData.geometryData.position.z);
                            barrierData.geometryData.position = position;
                            break;
                        }
                    case "p_y":
                        {
                            Vector3 position = new Vector3(barrierData.geometryData.position.x, float.Parse(value) , barrierData.geometryData.position.z);
                            barrierData.geometryData.position = position;
                            break;
                        }
                    case "p_z":
                        {
                            Vector3 position = new Vector3(barrierData.geometryData.position.x, barrierData.geometryData.position.y, float.Parse(value));
                            barrierData.geometryData.position = position;
                            break;
                        }
                    //key2 s_r
                    case "s_r":
                        {
                            ((SphereGeometryData)barrierData.geometryData).r = float.Parse(value);
                            break;
                        }
                    //key3 { "c_r", "c_h" }
                    case "c_r":
                        {
                            ((CylinderGeometryData)barrierData.geometryData).r = float.Parse(value);
                            break;
                        }
                    case "c_h":
                        {
                            ((CylinderGeometryData)barrierData.geometryData).height = float.Parse(value);
                            break;
                        }
                    //key4 { "length", "width", "height" }
                    case "length":
                        {
                            ((CubeGeometryData)barrierData.geometryData).size.x = float.Parse(value);
                            break;
                        }
                    case "width":
                        {
                            ((CubeGeometryData)barrierData.geometryData).size.z = float.Parse(value);
                            break;
                        }
                    case "height":
                        {
                            ((CubeGeometryData)barrierData.geometryData).size.y = float.Parse(value);
                            break;
                        }
                    //key5 { "cy_x", "cy_y", "cy_z" }
                    case "cy_x":
                        {
                            ((CylinderGeometryData)barrierData.geometryData).direction.x = float.Parse(value);
                            break;
                        }
                    case "cy_y":
                        {
                            ((CylinderGeometryData)barrierData.geometryData).direction.y = float.Parse(value);
                            break;
                        }
                    case "cy_z":
                        {
                            ((CylinderGeometryData)barrierData.geometryData).direction.z = float.Parse(value);
                            break;
                        }
                    //key5 { "cu_x", "cu_y", "cu_z" }
                    case "cu_x":
                        {
                            ((CubeGeometryData)barrierData.geometryData).direction.x = float.Parse(value);
                            break;
                        }
                    case "cu_y":
                        {
                            ((CubeGeometryData)barrierData.geometryData).direction.y = float.Parse(value);
                            break;
                        }
                    case "cu_z":
                        {
                            ((CubeGeometryData)barrierData.geometryData).direction.y = float.Parse(value);
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

    public BarrierData getData()
    {
        // Debug.Log(barrierData.geometryData.geometryType);
        return barrierData;
    }
}
