using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    //methods
    public void setObject(LightData data)
    {
        Light light = GetComponent<Light>();
        light.color = data.color;
        light.intensity = data.intensity;
    }
}
