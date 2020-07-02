using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = offset;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move() 
    {
        Vector3 playerRotation = player.GetComponent<PlayerController>().rotation;
        transform.localEulerAngles = new Vector3(playerRotation.x, 0f, 0f);
    }
}