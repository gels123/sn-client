using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLogic : MonoBehaviour
{
    public float zspeed = 5f;
    public float rspeed = 720f;
    public GameObject target;
    private bool bMove = false;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.localPosition, target.transform.position) > 0.5f)
        {
            this.transform.LookAt(target.transform);

            //第1种方法
            //Vector3 pos = this.transform.localPosition;
            //pos.z += zspeed * Time.deltaTime;
            //this.transform.localPosition = pos;

            //第2种方法
            //this.transform.Translate(0, 0, zspeed * Time.deltaTime, Space.Self);

            //第3种方法
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, this.target.transform.position, zspeed * Time.deltaTime);

            //第4种方法
            //this.transform.Translate(Vector3.forward * zspeed * Time.deltaTime, Space.Self);

            //Vector3 an = this.transform.localEulerAngles;
            //an.y += rspeed * Time.deltaTime;
            //this.transform.localEulerAngles = an;

            this.transform.Rotate(0, rspeed * Time.deltaTime, 0, Space.Self);
        }
    }

   void DrawLine() 
   {
        GameObject line = new GameObject();
        line.transform.localPosition = Vector3.zero;

   }
}
