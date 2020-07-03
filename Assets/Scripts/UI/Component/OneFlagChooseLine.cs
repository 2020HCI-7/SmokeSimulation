using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneFlagChooseLine : MonoBehaviour
{
    private string key;
    private GameObject parentPanel;

    private void Start()
    {
        Toggle toggle = transform.GetChild(0).GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate (bool _value)
        {
            this.setValue(key, _value.ToString());
        });
        parentPanel = transform.parent.gameObject;
    }

    public void init(string _key, string _value)
    {
        key = _key;
        Toggle toggle = transform.GetChild(0).GetComponent<Toggle>();
        toggle.isOn = bool.Parse(_value);
        Text keyText = transform.GetChild(1).GetComponent<Text>();
        keyText.text = _key;
        
    }

    public void setValue(string _key, string _value)
    {
        parentPanel.GetComponent<ObjectConfiguration>().set(_key, _value);
    }
}
