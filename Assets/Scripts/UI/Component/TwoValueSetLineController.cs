using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoValueSetLineController : MonoBehaviour
{
    //key in 2,3 input in 0,1
    private string[] key;
    private GameObject parentPanel;

    private void Start()
    {
        //此处不能使用循环
        InputField inputField = transform.GetChild(0).GetComponent<InputField>();
        inputField.onValueChanged.AddListener(delegate (string _value)
        {
            this.setValue(key[0], _value);
        });
        inputField = transform.GetChild(1).GetComponent<InputField>();
        inputField.onValueChanged.AddListener(delegate (string _value)
        {
            this.setValue(key[1], _value);
        });
        parentPanel = transform.parent.gameObject;
    }

    public void init(string[] _key, string[] _value)
    {
        key = _key;
        for (int i = 0; i < 2; i++)
        {
            Text keyText = transform.GetChild(i + 2).GetComponent<Text>();
            keyText.text = key[i];
            InputField inputField = transform.GetChild(i).GetComponent<InputField>();
            inputField.text = _value[i];
        }
    }

    public void setValue(string _key, string _value)
    {
        parentPanel.GetComponent<ObjectConfiguration>().set(_key, _value);
    }
}
