using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class fixtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("==fixtest Start==");

        LuaEnv luaenv = new LuaEnv();
        luaenv.DoString("CS.UnityEngine.Debug.Log('hello world11')");
        luaenv.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
