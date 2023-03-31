using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

[System.Serializable]
public class Injection
{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class MyLuaBehaviour : MonoBehaviour
{
    public TextAsset luaScript;
    public Injection[] injections;

    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;

    void Awake()
    {
        scriptEnv = luaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        foreach (var injection in injections)
        {
            scriptEnv.Set(injection.name, injection.value);
        }

        luaEnv.DoString(luaScript.text, "TestXlua2", scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        if (Time.time - MyLuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            MyLuaBehaviour.lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }

    static int ff1(int num1, int num2)
    {
        Debug.Log("MyLuaBehaviour:ff1 num1="+num1 + " num2=" + num2);
        return num1 + num2;
    }
}

static public class MySta
{
    static public int ff1(int num1, int num2)
    {
        Debug.Log("MySta:ff1 num1="+num1 + " num2=" + num2);
        return num1 + num2;
    }
}

public class Father
{
    public Father()
    {
        this.name = "";
        this.age = 0;
    }
    public Father(string name, int age)
    {
        this.name = name;
        this.age = age;
    }
    public string name { get; set; }
    public  int age { get; set; }

    public virtual void Say()
    {
        Debug.Log("Father:Say " + name + " " + age);
    }
}

public class Son : Father
{
    public Son(string name, int age)
    {
        this.name = name;
        this.age = age;
    }
    public override void Say()
    {
        Debug.Log("Son:Say " + name + " " + age);
    }
}
