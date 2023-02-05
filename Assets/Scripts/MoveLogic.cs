using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLogic : MonoBehaviour
{
    public float rotateSpeed = 120;
    public float fowardSpeed = 2;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(Vector3.forward * fowardSpeed * Time.deltaTime, Space.Self);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(Vector3.back * fowardSpeed * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime, Space.Self);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
}
