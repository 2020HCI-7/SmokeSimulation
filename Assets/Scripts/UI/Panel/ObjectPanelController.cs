using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPanelController : MonoBehaviour
{
    public GameObject sizeFilter;
    public GameObject ObjectButton;
    private int objectNumber;
    // Start is called before the first frame update
    void Start()
    {
        objectNumber = 0;
    }

    public void addObject(Data data)
    {
        if(objectNumber < 7) {
            Vector3 position = new Vector3(-10, 105 - objectNumber * 35f, 0);
            GameObject unit = Instantiate(ObjectButton, new Vector3(0f,0f,0f), Quaternion.identity);
            unit.transform.SetParent(sizeFilter.transform);
            unit.GetComponent<RectTransform>().localPosition = position;
            unit.GetComponent<ObjectButtonController>().init(data);
        }
        else {
            RectTransform rt = sizeFilter.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, (objectNumber + 1) * 35f);

            for (int i = 0; i < sizeFilter.transform.childCount; i++)
            {
                rt = sizeFilter.transform.GetChild(i).GetComponent<RectTransform>();
                rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y + 17.5f, rt.localPosition.z);
            }

            Vector3 position = new Vector3(-10, 105 - objectNumber * 35f + 17.5f * (objectNumber + 1 - 7), 0);
            GameObject unit = Instantiate(ObjectButton, position, Quaternion.identity);
            unit.transform.SetParent(sizeFilter.transform);
            unit.GetComponent<RectTransform>().localPosition = position;
            unit.GetComponent<ObjectButtonController>().init(data);
        }
        objectNumber += 1;
    }

    public void destroyObject(int index) {
        objectNumber = sizeFilter.transform.childCount;
        RectTransform rt = sizeFilter.GetComponent<RectTransform>();
        if(objectNumber > 7) {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, (objectNumber - 1) * 35f);
            
            for (int i = 0; i < sizeFilter.transform.childCount; i++)
            {
                rt = sizeFilter.transform.GetChild(i).GetComponent<RectTransform>();
                rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y - 17.5f, rt.localPosition.z);
            }
        }

        bool up = false;
        GameObject deleteButton = null;
        for (int i = 0; i < sizeFilter.transform.childCount; i++)
        {
            GameObject objectButton = sizeFilter.transform.GetChild(i).gameObject;
            if(objectButton.GetComponent<ObjectButtonController>().getIndex() == index) {
                up = true;
                deleteButton = objectButton;
                continue;
            }
            if(up) {
                rt = objectButton.GetComponent<RectTransform>();
                rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y + 35f, rt.localPosition.z);
            }
        }

        if(deleteButton != null) {
            Destroy(deleteButton);
        }
        objectNumber--;
    }
}
