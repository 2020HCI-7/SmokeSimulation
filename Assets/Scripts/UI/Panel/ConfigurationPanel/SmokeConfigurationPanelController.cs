﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeConfigurationPanelController : ObjectConfiguration
{
    private SmokeData smokeData;

    private GameObject smokeTypeSetLine;
    private GameObject geometrySetLine;
    private GameObject positionSetLine;
    private GameObject cycleSetLine;
    private GameObject coneSetLine;
    private GameObject physicalSetLine0;
    private GameObject physicalSetLine1;
    private GameObject colorSetLine;

    public void init(SmokeData _smokeData)
    {
        smokeData = _smokeData;

        smokeTypeSetLine = transform.GetChild(0).gameObject;
        bool[] value0 = { false, false , false };
        value0[(int)_smokeData.smokeType] = true;
        smokeTypeSetLine.GetComponent<ThreeTypeChooseLineController>().init("smoke type", _smokeData.geometryData.getGeometryTypeStringArray(), value0);

        if(_smokeData.smokeType != SmokeData.SmokeType.SMOKE) {
            geometrySetLine.SetActive(false);
            positionSetLine.SetActive(false);
            cycleSetLine.SetActive(false);
            coneSetLine.SetActive(false);
            physicalSetLine0.SetActive(false);
            physicalSetLine1.SetActive(false);
            colorSetLine.SetActive(false);
            return;
        }

        geometrySetLine = transform.GetChild(1).gameObject;
        geometrySetLine.SetActive(true);
        string[] key1 = { "cone", "cycle"};
        bool[] value1 = {_smokeData.geometryData.geometryType == GeometryData.GeometryType.CONE , _smokeData.geometryData.geometryType == GeometryData.GeometryType.CYCLE };
        geometrySetLine.GetComponent<TwoTypeChooseLineController>().init("geometry", key1, value1);

        positionSetLine = transform.GetChild(2).gameObject;
        positionSetLine.SetActive(true);
        string[] key2 = { "p_x", "p_y", "p_z" };
        string[] value2 = { smokeData.geometryData.position.x.ToString(), smokeData.geometryData.position.y.ToString(), smokeData.geometryData.position.z.ToString() };
        positionSetLine.GetComponent<ThreeValueSetLineController>().init(key2, value2);

        cycleSetLine = transform.GetChild(3).gameObject;
        coneSetLine = transform.GetChild(4).gameObject;
        cycleSetLine.SetActive(false);
        coneSetLine.SetActive(false);

        switch (_smokeData.geometryData.geometryType)
        {
            case GeometryData.GeometryType.CYCLE:
                cycleSetLine.SetActive(true);
                cycleSetLine.GetComponent<OneValueSetLineController>().init("cy_r", ((SphereGeometryData)_smokeData.geometryData).r.ToString());
                break;
            case GeometryData.GeometryType.CONE:
                coneSetLine.SetActive(true);
                string[] key4 = { "co_r", "co_h" };
                string[] value4 = { ((ConeGeometryData)_smokeData.geometryData).r.ToString(), ((ConeGeometryData)_smokeData.geometryData).height.ToString() };
                coneSetLine.GetComponent<TwoValueSetLineController>().init(key4, value4);
                break;
            default:
                break;
        }

        physicalSetLine0 = transform.GetChild(5).gameObject;
        physicalSetLine0.SetActive(true);
        string[] key5 = { "duration", "maxNumber" };
        string[] value5 = { smokeData.physicalData.duration.ToString(), smokeData.physicalData.maxNumber.ToString() };
        physicalSetLine0.GetComponent<TwoValueSetLineController>().init(key5, value5);

        physicalSetLine1 = transform.GetChild(6).gameObject;
        physicalSetLine1.SetActive(true);
        string[] key6 = { "particle size", "speed" };
        string[] value6 = { smokeData.physicalData.particleSize.ToString(), smokeData.physicalData.speed.ToString() };
        physicalSetLine1.GetComponent<TwoValueSetLineController>().init(key6, value6);

        colorSetLine = transform.GetChild(7).gameObject;
        colorSetLine.SetActive(true);
        string[] key7 = { "R", "G", "B" };
        string[] value7 = { smokeData.color.r.ToString(), smokeData.color.g.ToString(), smokeData.color.b.ToString() };
        colorSetLine.GetComponent<ThreeValueSetLineController>().init(key7, value7);
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
                    case "smoke type":
                        {
                            if (value == "smoke")
                            {
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
                                SmokeData newSmokeData = new SmokeData(smokeData.index, smokeData.name, Data.type.BARRIER, true, cycleGeometryData, physicalData, SmokeData.SmokeType.SMOKE, new Color(0 ,0 ,0));
                                init(newSmokeData);
                            }
                            else if (value == "fire")
                            {
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
                                SmokeData newSmokeData = new SmokeData(smokeData.index, smokeData.name, Data.type.BARRIER, true, cycleGeometryData, physicalData, SmokeData.SmokeType.FIRE, new Color(0, 0, 0));
                                init(newSmokeData);
                            }
                            else if (value == "explosion")
                            {
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
                                SmokeData newSmokeData = new SmokeData(smokeData.index, smokeData.name, Data.type.BARRIER, true, cycleGeometryData, physicalData, SmokeData.SmokeType.EXPLOSION, new Color(0, 0, 0));
                                init(newSmokeData);
                            }
                            break;
                        }
                    //key1 { "cone", "cycle"}
                    case "geometry":
                        {
                            if (value == "cone")
                            {
                                ConeGeometryData coneGeometryData = new ConeGeometryData(
                                    new Vector3(0f, 0.5f, 0f),
                                    0.1f,
                                    0.1f,
                                    new Vector3(0f, 0f, 0f)
                                );
                                PhysicalData physicalData = new PhysicalData(
                                    0.01f,
                                    10.0f,
                                    1000,
                                    3f
                                );
                                SmokeData newSmokeData = new SmokeData(smokeData.index, smokeData.name, Data.type.BARRIER, true, coneGeometryData, physicalData, SmokeData.SmokeType.SMOKE, new Color(0, 0, 0));
                                init(newSmokeData);
                            }
                            else if (value == "cycle")
                            {
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
                                SmokeData newSmokeData = new SmokeData(smokeData.index, smokeData.name, Data.type.BARRIER, true, cycleGeometryData, physicalData, SmokeData.SmokeType.FIRE, new Color(0, 0, 0));
                                init(newSmokeData);
                            }
                            break;
                        }
                    //key2 { "p_x", "p_y", "p_z" }
                    case "p_x":
                        {
                            Vector3 position = new Vector3(float.Parse(value), smokeData.geometryData.position.y, smokeData.geometryData.position.z);
                            smokeData.geometryData.position = position;
                            break;
                        }
                    case "p_y":
                        {
                            Vector3 position = new Vector3(smokeData.geometryData.position.x, float.Parse(value), smokeData.geometryData.position.z);
                            smokeData.geometryData.position = position;
                            break;
                        }
                    case "p_z":
                        {
                            Vector3 position = new Vector3(smokeData.geometryData.position.x, smokeData.geometryData.position.y, float.Parse(value));
                            smokeData.geometryData.position = position;
                            break;
                        }
                    //key3 { "cy_r" }
                    case "cy_r":
                        {
                            ((CycleGeometryData)smokeData.geometryData).r = float.Parse(value);
                            break;
                        }
                    //key4 { "co_r", "co_h" }
                    case "co_r":
                        {
                            ((ConeGeometryData)smokeData.geometryData).r = float.Parse(value);
                            break;
                        }
                    case "co_h":
                        {
                            ((ConeGeometryData)smokeData.geometryData).height = float.Parse(value);
                            break;
                        }
                    //key6 = { "duration", "maxNumber" };
                    case "duration":
                        {
                            smokeData.physicalData.duration = float.Parse(value);
                            break;
                        }
                    case "maxNumber":
                        {
                            smokeData.physicalData.maxNumber = int.Parse(value);
                            break;
                        }
                    //key7 = { "particle size", "speed" };
                    case "particle size":
                        {
                            smokeData.physicalData.particleSize = float.Parse(value);
                            break;
                        }
                    case "speed":
                        {
                            smokeData.physicalData.speed = float.Parse(value);
                            break;
                        }
                    //key8 { "R", "G", "B"}
                    case "R":
                        {
                            Color color = new Color(float.Parse(value), smokeData.color.g, smokeData.color.b);
                            smokeData.color = color;
                            break;
                        }
                    case "G":
                        {
                            Color color = new Color(smokeData.color.r, float.Parse(value), smokeData.color.b);
                            smokeData.color = color;
                            break;
                        }
                    case "B":
                        {
                            Color color = new Color(smokeData.color.r, smokeData.color.g, float.Parse(value));
                            smokeData.color = color;
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

    public SmokeData getData()
    {
        // Debug.Log(smokeData.geometryData.geometryType);
        return smokeData;
    }
}
