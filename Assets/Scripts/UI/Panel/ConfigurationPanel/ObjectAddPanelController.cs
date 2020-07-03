using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAddPanelController : MonoBehaviour
{
    public void toBarrierAdd() 
    {
        GameManager.instance.toAddConfiguration(Data.type.BARRIER);
    }

    public void toWindAdd()
    {
        GameManager.instance.toAddConfiguration(Data.type.WIND);
    }

    public void toSmokeAdd()
    {
        GameManager.instance.toAddConfiguration(Data.type.SMOKE);
    }
}
