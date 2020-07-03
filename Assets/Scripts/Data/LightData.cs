using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightData : Data
{
    public Color color;
    public float intensity;

    public LightData(int _index, string _name, Data.type _type, bool _deletable, Color _color, float _intensity) : base(_index, _name, _type, _deletable)
    {
        color = _color;
        intensity = _intensity;
    }

    public LightData(Data data, Color _color, float _intensity) : base(data)
    {
        color = _color;
        intensity = _intensity;
    }
}
