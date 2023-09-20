using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public int frameRate = 10;
    public float hourSpeed = 360/ 43200f;
    public float minSpeed = 360 / 3600f;
    public float secSpeed = 360 / 60f;
    public Transform hourBar;
    public Transform minBar;
    public Transform secBar;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = frameRate;
        hourBar = GameObject.Find("/Clock/HourPivot").transform;
        minBar = GameObject.Find("/Clock/MinPivot").transform;
        secBar = GameObject.Find("/Clock/SecPivot").transform;
        Debug.Log("hourBar position=" + hourBar.position + "minBar position=" + minBar.position + "secBar position=" + secBar.position + "hourSpeed="+ hourSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("=============asdfadfadfa" + Time.deltaTime);
        secBar.Rotate(Vector3.up * Time.deltaTime * secSpeed, Space.Self);
        minBar.Rotate(Vector3.up * Time.deltaTime * minSpeed, Space.Self);
        hourBar.Rotate(Vector3.up * Time.deltaTime * hourSpeed, Space.Self);
    }
}
