using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModePanelController : MonoBehaviour
{
    public GameObject operateButtonObject;
    public GameObject observeButtonObject;
    private GameManager.GameMode mode;

    void Start() 
    {
        mode = GameManager.GameMode.OPERATE;
    }

    public void toOperateMode()
    {
        if(mode != GameManager.GameMode.OPERATE)
        {
            GameManager.instance.changeMode(GameManager.GameMode.OPERATE);
            changeButtonColor();
            mode = GameManager.GameMode.OPERATE;
        }
    }

    public void toObserveMode()
    {
        
        if (mode != GameManager.GameMode.OBSERVE)
        {
            GameManager.instance.changeMode(GameManager.GameMode.OBSERVE);
            changeButtonColor();
            mode = GameManager.GameMode.OBSERVE;
        }
    }

    void changeButtonColor()
    {
        ColorBlock aColorBlock = operateButtonObject.GetComponent<Button>().colors;
        ColorBlock bColorBlock = observeButtonObject.GetComponent<Button>().colors;
        observeButtonObject.GetComponent<Button>().colors = aColorBlock;
        operateButtonObject.GetComponent<Button>().colors = bColorBlock;
    }
}
