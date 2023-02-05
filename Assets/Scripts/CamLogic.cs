using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CamLogic : MonoBehaviour
{

    //旋转变量
    float rotateY;
    float initialY;

    float Camerdis;
    Vector3 rayPoint;

    float newDis;
    float oldDis;
    private Vector3 point;

    private int isforward;//标记摄像机的移动方向

    private float eulerAngles_x;
    private float eulerAngles_y;
    private float distancePoint;//绕点旋转距离

    // public Texture2D ZoomView, PanView, OrbitView, FPSView;
    //public Text PO;

    public float Xpos;
    public float Ypos;
    public Camera cam;

    public bool UIclipMove = false;//是否在ui上限制移动

    bool LookFps = false;

    //高度限制
    public float maxHigh = 4000;
    public float minHigh = 0;
    public float maxX = 13000;
    public float minX = -27000;
    public float maxZ = 25000;
    public float minZ = -35000;
    // Use this for initialization


    public GameObject escObj;

    //public GameObject LightOBJ;

    //private void Awake()
    //{
    //    Screen.SetResolution(1920, 1080, false);
    //}



    void Start()
    {
        Camerdis = 1;
        rayPoint = cam.transform.forward * 800 + cam.transform.position;
        cameraRot = transform.localRotation;
    }
    //void FixedUpdate()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Escape))
    //    //{
    //    //    escObj.SetActive(true);
    //    //}
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        return;
    //    }
    //    //Camerdis = (float)Math.Round(cam.transform.position.y, 2);
    //    if (Input.touchCount == 1)
    //    {
    //        if (Input.touches[0].phase == TouchPhase.Moved&& Camera.main.ScreenToViewportPoint(Input.touches[0].position).x>0.2f)
    //        {
    //            Translation(0.005f);
    //        }
    //        else if (Input.touches[0].phase == TouchPhase.Moved && Camera.main.ScreenToViewportPoint(Input.touches[0].position).x <0.2f)
    //        {
    //            float moveX = Input.GetAxis("Mouse X") * Camerdis * 0.002f;
    //            Camera.main.transform.Translate(Vector3.forward * moveX);
    //        }

    //    }
    //    if (Input.touchCount == 2)
    //    {
    //        记录两个手指的位置
    //        //Vector2 nposition1 = new Vector2();
    //        //Vector2 nposition2 = new Vector2();

    //        记录手指的每帧移动距离
    //        //Vector2 deltaDis1 = new Vector2();
    //        //Vector2 deltaDis2 = new Vector2();


    //        //if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) //第二个手指
    //        //{
    //        //    nposition1 = Input.GetTouch(0).position; //第一个手指屏幕坐标
    //        //    nposition2 = Input.GetTouch(1).position; //第二个手指屏幕坐标
    //        //    deltaDis1 = Input.GetTouch(0).deltaPosition;
    //        //    deltaDis2 = Input.GetTouch(1).deltaPosition;

    //        //    newDis = Vector3.Distance(nposition1, nposition2);
    //        //    if (newDis > oldDis)
    //        //    {
    //        //        isforward = 1;
    //        //    }
    //        //    else
    //        //    {
    //        //        isforward = -1;
    //        //    }

    //        //    //记录旧的触摸位置
    //        //    oldDis = newDis;

    //        //    //移动摄像机
    //        //    // Camera.main.transform.Translate(isforward * Vector3.forward * Time.deltaTime * (Mathf.Abs(deltaDis2.x + deltaDis1.x) + Mathf.Abs(deltaDis1.y + deltaDis2.y)) * Camerdis * 0.1f);
    //        //    Camera.main.transform.Translate(isforward * Vector3.forward * Time.deltaTime  * Camerdis * 0.1f);
    //        //    PO.text = isforward.ToString();
    //        //}
    //        //else
    //        //{
    //        if (Input.touches[0].phase == TouchPhase.Moved)
    //        { 
    //            RotatePoint();
    //        }       
    //    }
    //}

    public void ExitQ()
    {
        Application.Quit();
    }

    //控制高度
    void ControlHigh()
    {
        float highY = transform.position.y;
        float highX = transform.position.x;
        float highZ = transform.position.z;
        highY = Mathf.Clamp(highY, minHigh, maxHigh);
        highX = Mathf.Clamp(highX, minX, maxX);
        highZ = Mathf.Clamp(highZ, minZ, maxZ);
        //transform.position = new Vector3(transform.position.x, highY, transform.position.z);
        transform.position = new Vector3(highX, highY, highZ);


        //AmbientOcclus();//控制AO
    }
    // Update is called once per frame
    void Update()
    {
        //if (UIclipMove)
        //{
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}
        //}


        //transform.LookAt(escObj.transform);

        //if (Gamemanager._instatic.isMove)
        //{
        //    return;
        //}

        //if (Input.touchCount <= 0 && Camera.main.ScreenToViewportPoint(Input.mousePosition).x < Xpos && Camera.main.ScreenToViewportPoint(Input.mousePosition).y > Ypos)
        //{
        //if (EventSystem.current.IsPointerOverGameObject())
        //{

        //    return;
        //}
        //if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2))
        //光标重置
            //{
            //    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            //}

        if (Input.GetMouseButton(2))
        //按住左键平移
        {
            // Cursor.SetCursor(PanView, Vector2.zero, CursorMode.Auto);
            Translation(0.03f);
            ControlHigh();
        }
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                RayPoint();
                transform.Translate(Vector3.back * Camerdis * 0.1f);
                ControlHigh();
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                RayPoint();
                transform.Translate(Vector3.forward * Camerdis * 0.1f);
                ControlHigh();
            }
        }
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            eulerAngles_y = this.transform.eulerAngles.x;
            eulerAngles_x = this.transform.eulerAngles.y;

            RayPoint();
            ControlHigh();
            distancePoint = Vector3.Distance(transform.position, rayPoint);

        }
        if (Input.GetMouseButton(1))
        //右键旋转
        {
            // Cursor.SetCursor(OrbitView, Vector2.zero, CursorMode.Auto);
            RotatePoint();
            ControlHigh();
        }
        KeyTranslation();
        //}

        //if (Input.GetKeyDown(KeyCode.F10))
        //{
        //    if (LookFps == false)
        //    {
        //        LookFps = true;
        //    }
        //    else
        //    {
        //        LookFps = false;
        //    }
        //}
        //if (LookFps)
        //{
        //    lookFPS();
        //}
    }

    void Translation(float sheep)
    //平移控制
    {
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");
        //自身坐标的z轴投影到世界坐标的z轴，用自身坐标的y轴和z轴的值乘 自身的相对欧拉角的x的三角函数。
        float tranY = moveY * (float)Math.Sin(Math.Round(this.transform.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
        float tranZ = moveY * (float)Math.Cos(Math.Round(this.transform.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
        transform.Translate(new Vector3(-moveX, -tranY, -tranZ) * Camerdis * sheep, Space.Self);
    }
    void Rotation()
    //旋转控制
    {
        initialY = this.transform.localRotation.eulerAngles.y;
        rotateY = 0;
        rotateY += Input.GetAxis("Mouse X") * 2f;
        var rotation = Quaternion.Euler(this.transform.localRotation.eulerAngles.x, rotateY + initialY, 0);
        transform.rotation = rotation;

    }
    void RayPoint()
    //射线得到碰撞点,针对绕点旋转
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));//射线  
                                                                                            //  Ray ray = cam.ScreenPointToRay(Input.mousePosition);//射线  

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))//发射射线(射线，射线碰撞信息，射线长度，射线会检测的层级)  
        {
            rayPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            Camerdis = (float)Math.Round(cam.transform.position.y - hit.point.y, 2) + 10;
        }
        //else
        //{
        //    rayPoint = transform.forward*0.005f  + transform.position;//摄像机前方 800 点
        //    //Camerdis = (float)Math.Round(cam.transform.position.y, 2) ;
        //}

    }
    float a = 0;

    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    //是否平滑
    public bool smooth;
    //平滑参数
    public float smoothTime = 5f;

    //相机
    private Quaternion cameraRot;
    Quaternion quaternion;
    void RotatePoint()
    //绕点旋转
    {

        eulerAngles_x += (Input.GetAxis("Mouse X") * 2);
        eulerAngles_y -= (Input.GetAxis("Mouse Y") * 2);
        eulerAngles_y = ClampAngle(eulerAngles_y, 3, 89);
        quaternion = Quaternion.Euler(eulerAngles_y, eulerAngles_x, 0);
        Vector3 vector = quaternion * new Vector3(0, 0, -distancePoint) + rayPoint;
        transform.rotation = Quaternion.Slerp(transform.localRotation, quaternion, 20f * Time.deltaTime);
        if (Input.GetKey(KeyCode.LeftControl))
        //右键旋转
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, vector, Time.deltaTime * 20f);
        //

    }
    void Euler()
    //当前物体的欧拉角
    {
        Vector3 eulerAngles = this.transform.eulerAngles;
        this.eulerAngles_x = eulerAngles.y;


        if (eulerAngles.x > 80)
        {
            this.eulerAngles_y = 80;
        }
        else if (eulerAngles.x < 10)
        {
            this.eulerAngles_y = 10;
        }
        else
        {
            this.eulerAngles_y = eulerAngles.x;
        }


    }
    //键盘控制
    void KeyTranslation()
    //平移控制
    {
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 6;
        float moveY = Input.GetAxis("Vertical") * Time.deltaTime * 6;
        if (moveX == 0 && moveY == 0)
        {
            return;
        }
        //自身坐标的z轴投影到世界坐标的z轴，用自身坐标的y轴和z轴的值乘 自身的相对欧拉角的x的三角函数。
        float tranY = moveY * (float)Math.Sin(Math.Round(this.transform.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
        float tranZ = moveY * (float)Math.Cos(Math.Round(this.transform.localRotation.eulerAngles.x, 2) * Math.PI / 180.0);
        transform.Translate(new Vector3(moveX, tranY, tranZ) * Camerdis * 0.05f, Space.Self);
    }

    //角度限制
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}

