using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanelController : MonoBehaviour
{
    private Text line0;
    private Text line1;
    private Text line2;
    // Start is called before the first frame update
    void Start()
    {
        Text[] lines = GetComponentsInChildren<Text>();
        line0 = lines[0];
        line1 = lines[1];
        line2 = lines[2];
    }

    public void addLine(string line) 
    {
        line ="[" + System.DateTime.Now.ToString("HH:mm:ss") + "] " + line;
        if( line0.text == "") {
            line0.text = line;
        }
        else if(line1.text == "") {
            line1.text = line;
        }
        else if(line2.text == ""){
            line2.text = line;
        }
        else {
            line0.text = line1.text;
            line1.text = line2.text;
            line2.text = line;
        }
    }
}
