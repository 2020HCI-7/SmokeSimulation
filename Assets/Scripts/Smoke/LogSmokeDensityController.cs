using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSmokeDensityController : MonoBehaviour
{
    private float deltaTime;
    LogSmokeDensityData oldData;
    float time;

    private void Start() {
        oldData = new LogSmokeDensityData(0, "", Data.type.LOGDENSITY, false, false, 1f);
        deltaTime = GameManager.instance.deltaTime;
    }

    private void Update() {
        // tryLog();
    }

    public void setObject(LogSmokeDensityData data)
    {
        oldData = data;
    }

    private void tryLog()
    {
        if(oldData.logFlag) {
            time += deltaTime;
            if (time > oldData.interval) {
                time = 0f;
                GameManager.instance.addLog("[Density] *** particles in a 0.001(0.1 * 0.1 * 0.1) space");
            }
        }
        

    }
}
