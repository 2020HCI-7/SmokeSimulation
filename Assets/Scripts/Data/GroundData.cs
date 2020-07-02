using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GroundData: Data
{
    public float size;

    public GroundData(int _index, string _name, Data.type _type, bool _deletable, float _size) : base(_index, _name, _type, _deletable)
    {
        size = _size;
    }

    public GroundData(Data _data, float _size): base(_data)
    {
        size = _size;
    }
}