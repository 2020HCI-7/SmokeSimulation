using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeData : Data
{
    public enum SmokeType { SMOKE, FIRE, EXPLOSION };
    public GeometryData geometryData;
    public PhysicalData physicalData;
    public SmokeType smokeType;
    public Color color;
    public bool windDirtyFlag;
    public bool barrierDirtyFLag;

    public SmokeData(int _index, string _name, type _type, bool _deletable, GeometryData _geometryData, PhysicalData _physicalData,  SmokeType _smokeType, Color _color, bool _windDirtyFlag, bool _barrierDirtyFLag): base(_index, _name, _type, _deletable)
    {
        geometryData = _geometryData;
        physicalData = _physicalData;
        smokeType = _smokeType;
        color = _color;
        windDirtyFlag = _windDirtyFlag;
        barrierDirtyFLag = _barrierDirtyFLag;
    }
}

public class PhysicalData
{
    public float particleSize;
    public float duration;
    public int maxNumber;
    public float speed;

    public PhysicalData(float _particleSize, float _duration, int _maxNumber, float _speed) 
    {
        particleSize = _particleSize;
        duration = _duration;
        maxNumber = _maxNumber;
        speed = _speed;
    }
}
