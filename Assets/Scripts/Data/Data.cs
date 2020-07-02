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
        }
        return "";
    }
}
