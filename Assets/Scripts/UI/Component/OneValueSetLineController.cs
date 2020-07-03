using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneValueSetLineController : MonoBehaviour
{
    private string key;
    private string value;
    private GameObject parentPanel;

    private void Start() {
        InputField inputField = transform.GetChild(1).GetComponent<InputField>();
        inputField.onValueChanged.AddListener(delegate (string _value)
        {
            this.setValue(key, _value);   
        });
        parentPanel = transform.parent.gameObject;
    }

    public void init(string _key, string _value)
    {
        key = _key;
        value = _value;
        Text keyText = transform.GetChild(0).GetComponent<Text>();
        keyText.text = _key;
        InputField inputField = transform.GetChild(1).GetComponent<InputField>();
        inputField.text = _value;
    }

    public void setValue(string _key, string _value)
    {
        parentPanel.GetComponent<ObjectConfiguration>().set(_key, _value);
        value = _value;
    }
}
