using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandLinePanelController : MonoBehaviour
{
    private GameObject input;
    // Start is called before the first frame update
    void Start()
    {
        input = transform.GetChild(0).gameObject;
    }

    public void endEdit()
    {
        string text = input.GetComponent<InputField>().text;
        bool success = run(text);
        if (success)
        {
            GameManager.instance.addLog("[Command] " + text);
        }
        else
        {
            GameManager.instance.addLog("[Error] " + text);
        }
        input.GetComponent<InputField>().text = "";
    }

    bool run(string command) 
    {
        string[] commands = command.Split(' ');
        if(commands.Length == 1) {
            return false;
        }
        switch (commands[0])
        {
            case "save":
                GameManager.instance.save(commands[1]);
                return true;
            case "load":
                GameManager.instance.load(commands[1]);
                return true;
            default:
                break;
        }

        return false;
    }
}
