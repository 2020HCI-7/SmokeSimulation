using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommitButtonController : MonoBehaviour
{
    public void setText(string text) 
    {
        transform.GetChild(0).GetComponent<Text>().text = text;
    }
}
