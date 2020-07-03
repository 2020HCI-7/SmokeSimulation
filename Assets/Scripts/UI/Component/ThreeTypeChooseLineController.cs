using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeTypeChooseLineController : MonoBehaviour
{
    //key input in 0,1,2
    private string[] key;
    private string topName;
    private GameObject parentPanel;

    private void Start()
    {
        //此处不能使用循环
        Toggle toggle = transform.GetChild(0).GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate (bool _value)
        {
            this.setValue(0, _value);
        });
        toggle = transform.GetChild(1).GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate (bool _value)
        {
            this.setValue(1, _value);
        });
        toggle = transform.GetChild(2).GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate (bool _value)
        {
            this.setValue(2, _value);
        });
        parentPanel = transform.parent.gameObject;
    }

    public void init(string _topName, string[] _key, bool[] value)
    {
        topName = _topName;
        key = _key;
        Debug.Log(transform.childCount);
        transform.GetChild(3).GetComponent<Text>().text = topName;
        for (int i = 0; i < 3; i++)
        {
            Toggle toggle = transform.GetChild(i).GetComponent<Toggle>();
            toggle.transform.GetChild(1).GetComponent<Text>().text = key[i];
            toggle.isOn = value[i];
        }
    }

    public void setValue(int index, bool value)
    {
        if(value) {
            Debug.Log(index);
            for (int i = 0; i < 3; i++) {
                if(index != i) {
                    transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
            parentPanel.GetComponent<ObjectConfiguration>().set(topName, key[index]);
        }
        else {
            transform.GetChild(index).GetComponent<Toggle>().isOn = true;
        }
    }
}
