using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float rSpeed;
    private float deltaTime;
    public Vector3 rotation;
    float xMouse;
    float yMouse;
    private bool moveFlag;
    private float interval;
    // Start is called before the first frame update
    void Start()
    {
        moveFlag = true;
        interval = 0;
        deltaTime = GameManager.instance.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if(Input.GetMouseButton(2)) {
            if(interval > 0.5f) {
                moveFlag = !moveFlag;
                interval = 0;
            }
        }
        interval += deltaTime;

        if(!moveFlag) {
            return;
        }
        // Rotation
        xMouse += Input.GetAxis("Mouse X");
        yMouse += Input.GetAxis("Mouse Y");
        rotation.y = xMouse * rSpeed;

        float angle = -yMouse * rSpeed;
        rotation.x = angle;

        // Movement
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        float moveHorizontal = xInput * Mathf.Cos(rotation.y * Mathf.Deg2Rad) + zInput * Mathf.Sin(rotation.y * Mathf.Deg2Rad);
        float moveVertical = - xInput * Mathf.Sin(rotation.y * Mathf.Deg2Rad) + zInput * Mathf.Cos(rotation.y * Mathf.Deg2Rad);
        float moveY = 0.0f;
        if(Input.GetKey(KeyCode.LeftShift)) {
            moveY = -1;
        }
        else if(Input.GetKey(KeyCode.Space)) {
            moveY = 1;
        }

        Vector3 movement = new Vector3(moveHorizontal, moveY, moveVertical);

        // change
        Vector3 newPosition = transform.position + movement * deltaTime * speed;
        if(newPosition.y < 0.5f) {
            transform.position = new Vector3(newPosition.x, 0.5f, newPosition.z);
        }
        else {
            transform.position = newPosition;
        }
        transform.localEulerAngles = new Vector3(0f, rotation.y, 0f);
    }
}
