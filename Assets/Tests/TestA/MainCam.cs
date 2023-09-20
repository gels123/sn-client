using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Debug.Log("==MainCam Start1==");
        Debug.Log("==MainCam pos== " + this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 vec = this.transform.localPosition;
        //float speed = 0.5f;
        //vec.x += speed * Time.deltaTime;
        //this.transform.localPosition = vec;
        this.transform.Translate(0.5f * Time.deltaTime, 0, 0, Space.World);
    }
}
