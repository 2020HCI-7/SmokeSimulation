using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSmokeDensityData : Data
{
    public bool logFlag;
    public float interval;

    public LogSmokeDensityData(int _index, string _name, Data.type _type, bool _deletable, bool _logFlag, float _interval) : base(_index, _name, _type, _deletable)
    {
        logFlag = _logFlag;
        interval = _interval;
    }

    public LogSmokeDensityData(Data data, bool _logFlag, float _interval) : base(data)
    {
        logFlag = _logFlag;
        interval = _interval;
    }
}
