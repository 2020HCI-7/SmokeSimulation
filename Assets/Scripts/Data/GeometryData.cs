using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryData
{
    public enum GeometryType { CUBE, CYLINDER, SPHERE, CONE, CYCLE};
    public GeometryType geometryType;

    //除了锥体外该位置记录的是中心位置
    public Vector3 position;

    public GeometryData(GeometryType _geometryType, Vector3 _position)
    {
        geometryType = _geometryType;
        position = _position;
    }

    public string[] getGeometryTypeStringArray()
    {
        string[] array = { "cube", "cylinder", "sphere", "cone", "cycle" };
        return array;
    }
}

public class CubeGeometryData: GeometryData
{
    public Vector3 size;

    public Vector3 direction;

    public CubeGeometryData(Vector3 _position, Vector3 _size, Vector3 _direction) : base(GeometryType.CUBE, _position)
    {
        size = _size;
        direction = _direction;
    }
}

public class CylinderGeometryData : GeometryData
{
    public float height;
    public float r;
    public Vector3 direction;

    public CylinderGeometryData(int _index, string _name, Data.type _type, bool _deletable, Vector3 _position, float _height, float _r, Vector3 _direction) : base(GeometryType.CYLINDER, _position)
    {
        height = _height;
        r = _r;
        direction = _direction;
    }
}

public class SphereGeometryData : GeometryData
{
    public float r;

    public SphereGeometryData(int _index, string _name, Data.type _type, Vector3 _position, bool _deletable, float _r) : base(GeometryType.SPHERE, _position)
    {
        r = _r;
    }
}

public class ConeGeometryData : GeometryData
{
    //锥体的位置记录的是锥尖的位置，方向是从锥尖指向锥底的方向
    public float r;
    public float height;
    public Vector3 direction;

    public ConeGeometryData(int _index, string _name, Data.type _type, Vector3 _position, bool _deletable, float _r, float _height, Vector3 _direction) : base(GeometryType.CONE, _position)
    {
        r = _r;
        height = _height;
        direction = _direction;
    }
}

public class CycleGeometryData : GeometryData
{
    public float r;
    public Vector3 direction;

    public CycleGeometryData(int _index, string _name, Data.type _type, Vector3 _position, bool _deletable, float _r,Vector3 _direction) : base(GeometryType.CYCLE, _position)
    {
        r = _r;
        direction = _direction;
    }
}
