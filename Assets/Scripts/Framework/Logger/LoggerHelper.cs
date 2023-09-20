using UnityEngine;
using System.Collections.Generic;
using XLua;
using System;

[Hotfix]
public class LoggerHelper : MonoSingleton<LoggerHelper>
{
    public enum LOG_TYPE
    {
        LOG = 0,
        LOG_ERR,
    }

    struct LogInfo
    {
        public LOG_TYPE type;
        public string msg;
        public LogInfo(LOG_TYPE type, string msg)
        {
            this.type = type;
            this.msg = msg;
        }
    }
    
    private List<LogInfo> backList = new List<LogInfo>(100);
    private List<LogInfo> frontList = new List<LogInfo>(100);

    protected override void Init()
    {
        if (!Application.isEditor)
        {
            Application.logMessageReceived += (LogHandler);
            // 每5秒上报error
            InvokeRepeating("CheckReport", 5.0f, 5.0f);
        }
    }

    private void LogHandler(string condition, string stackTrace, LogType type)
    {
        if (Application.isEditor)
        {
            return;
        }
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            Logger.LogError(condition + " \n" + stackTrace);
        }
    }

    private void CheckReport()
    {
        Update_(); // 无需每帧更新
        Logger.CheckReportError();
    }
    
    private void Update_()
    {
        lock (backList)
        {
            if (backList.Count > 0)
            {
                List<LogInfo> tmp = frontList;
                frontList = backList;
                backList = tmp;
            }
        }
        if (frontList.Count > 0)
        {
            for (int i = 0; i < frontList.Count; i++)
            {
                var logInfo = frontList[i];
                switch (logInfo.type)
                {
                    case LOG_TYPE.LOG:
                        {
                            Logger.Log(logInfo.msg, null);
                            break;
                        }
                    case LOG_TYPE.LOG_ERR:
                        {
                            Logger.LogError(logInfo.msg, null);
                            break;
                        }
                }
            }
            frontList.Clear();
        }
    }

    public override void Dispose()
    {
        lock (backList)
        {
            backList.Clear();
        }
        frontList.Clear();
        base.Dispose();
    }

    public void LogToMainThread(LOG_TYPE type, string msg)
    {
        lock (backList)
        {
            backList.Add(new LogInfo(type, msg));
        }
    }
}

#if UNITY_EDITOR
public static class LoggerHelperExporter
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(LoggerHelper),
    };
}
#endif
