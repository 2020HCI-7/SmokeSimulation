using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindData : Data
{
    GeometryData geometryData;
    float intensity;
    float interference;

    public WindData(int _index, string _name, type _type, bool _deletable, GeometryData _geometryData, float _intensity, float _interference): base(_index, _name, _type, _deletable)
    {
        geometryData = _geometryData;
        intensity = _intensity;
        interference = _interference;
    }
}
