using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class TestXlua1 : MonoBehaviour
{
    public Button btn;

    public InputField input;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("==TestXlua1:Start begin==");
        LuaEnv env = new LuaEnv();
        string str = "CS.UnityEngine.Debug.Log(\"====C# logs====\")";
        env.DoString(str);
        env.Dispose();
        Debug.Log("==TestXlua1:Start end==");
        
        // btn.onClick.AddListener(onClick);
        // btn.onClick.AddListener(() =>
        // {
        //     onClick();
        // });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void onClick()
    {
        Debug.Log("==TestXlua1:onClick==" + input.text);
    }
}
