using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Data
{
    public int index;
    public string name;
    public type dataType;
    public bool deletable;
    public enum type 
    {
        GROUND,
        LIGHT,
        LOGDENSITY,
        BARRIER,
        WIND,
        SMOKE,    
    }
    public Data()
    {
        index = 0;
        name = "";
        dataType = type.GROUND;
        deletable = false;
    }
    public Data(int _index, string _name, type _type, bool _deletable)
    {
        index = _index;
        name = _name;
        dataType = _type;
        deletable = _deletable;
    }

    public Data(Data _data) 
    {
        index = _data.index;
        name = _data.name;
        dataType = _data.dataType;
        deletable = _data.deletable;
    }

    public string dataTypeToString(type _type)
    {
        switch(_type){
            case type.GROUND:
                return "GROUND";
            case type.LIGHT:
                return "LIGHT";
            case type.LOGDENSITY:
                return "LOGDENSITY";
            case type.BARRIER:
                return "BARRIER";
            case type.WIND:
                return "WIND";
            case type.SMOKE:
                return "SMOKE";
            default :
                return "";
        }
    }
}
