using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public GameObject planeUnit;

    private GroundData oldData;
    // Start is called before the first frame update
    void Start()
    {
        oldData = new GroundData(0, "", Data.type.GROUND, false, 0.0f);
    }

    //methods
    public void setObject(GroundData data){
        //该函数可能在start前被调用
        if(oldData == null) {
            oldData = new GroundData(0, "", Data.type.GROUND, false, 0.0f);
        }
        float nowSize = data.size;
        float lastSize = oldData.size;
        // GameManager.instance.addLog(nowSize.ToString());
        if(nowSize == lastSize) {
            return;
        }
        else if(nowSize > lastSize) {
            GameObject unit = null;
            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            //一圈一圈加
            // 
            //   1321
            //   2003
            //   3002
            //   1231
            //
            for (float i = lastSize; i < nowSize; i+=0.2f) {
                float temp = i / 2 + 0.05f;
                for (float k = 0; k < 2 * temp; k+=0.1f) {
                    position = new Vector3(temp, 0, (-1) * temp + k);
                    unit = Instantiate(planeUnit, position, Quaternion.identity);
                    unit.transform.SetParent(transform);
                    position = new Vector3(temp - k, 0, temp);
                    unit = Instantiate(planeUnit, position, Quaternion.identity);
                    unit.transform.SetParent(transform);
                    position = new Vector3((-1) * temp, 0, temp - k);
                    unit = Instantiate(planeUnit, position, Quaternion.identity);
                    unit.transform.SetParent(transform);
                    position = new Vector3((-1) * temp + k, 0, (-1) * temp);
                    unit = Instantiate(planeUnit, position, Quaternion.identity);
                    unit.transform.SetParent(transform);
                }
            }
            lastSize = nowSize;
            oldData.size = nowSize;
        }
        else if(nowSize < lastSize) {
            float temp = nowSize / 2 - 0.05f;
            //逆序遍历
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                Transform unitTransform = transform.GetChild(i);
                Vector3 position = unitTransform.position;
                if(position.x > temp || position.x < (-1) * temp || position.z > temp || position.z < (-1) * temp){
                    Destroy(unitTransform.gameObject);
                }
                else {
                    break;//编号与位置正相关，编号越大越远离中心
                }
            }
            lastSize = nowSize;
            oldData.size = nowSize;
        }
    }
}
