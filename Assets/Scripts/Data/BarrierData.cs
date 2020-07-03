using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierData: Data
{
    public GeometryData geometryData;

    public BarrierData(int _index, string _name, type _type, bool _deletable, GeometryData _geometryData): base(_index, _name, _type, _deletable)
    {
        geometryData = _geometryData;
    }
}

public class CubeBarrierData : BarrierData
{
    public CubeBarrierData(int _index, string _name, type _type, bool _deletable, CubeGeometryData _cubeGeometryData): base(_index, _name, _type, _deletable, _cubeGeometryData)
    {
    }
}

public class CylinderBarrierData : BarrierData
{
    public CylinderBarrierData(int _index, string _name, type _type, bool _deletable, CylinderGeometryData _cylinderGeometryData) : base(_index, _name, _type, _deletable, _cylinderGeometryData)
    {
    }
}

public class SphereBarrierData : BarrierData
{
    public SphereBarrierData(int _index, string _name, type _type, bool _deletable, SphereGeometryData _sphereGeometryData) : base(_index, _name, _type, _deletable, _sphereGeometryData)
    {
    }
}
