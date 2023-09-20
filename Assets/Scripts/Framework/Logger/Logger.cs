using UnityEngine;
using System.Diagnostics;
using System.Net;
using System;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Text;

public class Logger
{
    static protected Stopwatch watch = new Stopwatch();
    private static WebClient m_webClient = new WebClient();
    private static List<string> m_errorList = new List<string>();
    private static bool m_canTakeError = true;
    private static bool m_isInit = false;
    private static StringBuilder sb = new StringBuilder();
    public static string appVersion = string.Empty;
    public static string resVersion = string.Empty;
    public static string loginUid = string.Empty;
    public static string localIP = string.Empty;
    public static string platName = string.Empty;
    public static string sceneName = "Launch";
    public static string DEBUG_BUILD_VER = "HOG_ALPHA_1";
    public static string platChannel = "outnet";
    private static LoggerHelper m_helper = LoggerHelper.Instance;
    
    // 打日志
    [Conditional("UNITY_EDITOR")]
    [Conditional("LOGGER_ON")]
    static public void Log(string s, params object[] p)
    {
        Debug.Log(DateTime.Now + " -- " + (p != null && p.Length > 0 ? string.Format(s, p) : s));
    }
    
    // 打日志
    [Conditional("UNITY_EDITOR")]
    [Conditional("LOGGER_ON")]
    static public void Log(object o)
    {
        Debug.Log(o);
    }

    // 非主线程打日志
    [Conditional("UNITY_EDITOR")]
    [Conditional("LOGGER_ON")]
    public static void LogToMainThread(string s, params object[] p)
    {
        string msg = (p != null && p.Length > 0 ? string.Format(s, p) : s);
        m_helper.LogToMainThread(LoggerHelper.LOG_TYPE.LOG, msg);
    }

    [Conditional("UNITY_EDITOR")]
    [Conditional("LOGGER_ON")]
    static public void Assert(bool condition, string s, params object[] p)
    {
        if (condition)
        {
            return;
        }
        LogError("Assert failed! Message:\n" + s, p);
    }
    
    // 打错误日志
    static public void LogError(string s, params object[] p)
    {
#if UNITY_EDITOR
        Debug.LogError((p != null && p.Length > 0 ? string.Format(s, p) : s));
#elif LOGGER_ON
        AddError(string.Format("clientversion:{0} uid: {1} device:{2} ip:{3} platname:{4} platChannel:{5} scenename:{6} debug_build_ver:{7} msg: {8} ",
        appVersion, loginUid, (SystemInfo.deviceModel + "/" + SystemInfo.deviceUniqueIdentifier), localIP, platName, platChannel, sceneName, DEBUG_BUILD_VER,
        (p != null && p.Length > 0 ? string.Format(s, p) : s)));
#endif
    }
    
    // 非主线程打错误日志
    public static void LogErrorToMainThread(string s, params object[] p)
    {
        string msg = (p != null && p.Length > 0 ? string.Format(s, p) : s);
        m_helper.LogToMainThread(LoggerHelper.LOG_TYPE.LOG_ERR, msg);
    }
    
    static public void LogStackTrace(string str)
    {
        StackFrame[] stacks = new StackTrace().GetFrames();
        string result = str + "\r\n";
        if (stacks != null)
        {
            for (int i = 0; i < stacks.Length; i++)
            {
                result += string.Format("{0} {1}\r\n", stacks[i].GetFileName(), stacks[i].GetMethod().ToString());
            }
        }
        LogError(result);
    }

    private static void AddError(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            m_errorList.Add(msg);
        }
    }

    private static void SendToHttpSvr(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            if (!m_isInit)
            {
                // curl:setopt(luacurl.OPT_HTTPHEADER,"Content-Type:application/json;charset=UTF-8")
                m_webClient.Encoding = Encoding.UTF8;
                m_webClient.Headers.Add("Content-Type", "application/json");
                m_webClient.Headers.Add("charset", "UTF-8");
                m_webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(OnUploadStringCompleted);
                m_isInit = true;
            }
            // 上报钉钉
            msg = string.Format("{{\"msgtype\": \"text\",\"text\": {{\"content\":\"{0}\"}}}}", msg);
            m_webClient.UploadStringAsync(new Uri(URLSetting.REPORT_ERROR_URL), msg);
        }
    }

    public static void CheckReportError()
    {
        if (m_canTakeError)
        {
            int count = m_errorList.Count;
            if (count > 0)
            {
                m_canTakeError = false;
                sb.Length = 0;
                for (int i = 0; i < count; i++)
                {
                    sb.Append(m_errorList[i] + " | ");
                }
                m_errorList.Clear();
                SendToHttpSvr(sb.ToString());
            }
        }
    }

    static void OnUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
    {
        m_canTakeError = true;
    }

    [Conditional("UNITY_EDITOR")]
    static public void Watch()
    {
#if UNITY_EDITOR
        watch.Reset();
        watch.Start();
#endif
    }

    static public long useTime
    {
        get
        {
#if UNITY_EDITOR
            return watch.ElapsedMilliseconds;
#else
            return 0;
#endif
        }
    }

    static public string useMemory
    {
        get
        {
#if UNITY_5_6_OR_NEWER
            return (UnityEngine.Profiling.Profiler.usedHeapSizeLong / 1024 / 1024).ToString() + " mb";
#elif UNITY_5_5_OR_NEWER
            return (UnityEngine.Profiling.Profiler.usedHeapSize / 1024 / 1024).ToString() + " mb";
#else
            return (Profiler.usedHeapSize / 1024 / 1024).ToString() + " mb";
#endif
        }
    }
}