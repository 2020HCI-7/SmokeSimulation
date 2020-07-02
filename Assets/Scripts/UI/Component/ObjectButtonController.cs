using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectButtonController : MonoBehaviour
{
    private Data data;
    // Start is called before the first frame update
    void Start()
    {
        Button button = this.GetComponent<Button>();
        button.onClick.AddListener(delegate ()
        {
            this.setIndex(data.index);
        });
    }

    public void init(Data _data)
    {
        data = _data;
        Text textitem = transform.GetChild(0).GetComponent<Text>();
        textitem.text = "ID: " + data.index.ToString() + "  Name: " + data.name + "  Type: " + data.dataTypeToString(data.dataType);

        if(!_data.deletable) {
            Destroy(transform.GetChild(1).gameObject);
        }
    }
    public void setIndex(int index)
    {
        GameManager.instance.setCurrentIndex(index);
    }
    public void destroy()
    {
        GameManager.instance.destroyObject(data.index);
        
    }

    public int getIndex()
    {
        return data.index;
    }
}
