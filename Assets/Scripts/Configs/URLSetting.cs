/// <summary>
///  URL相关配置
/// </summary>

public class URLSetting
{
    public static string START_UP_URL
    {
        get
        {
            // TODO：外网启动地址，这个地址在发布线上游戏时自行部署和设置
            return "https://chivas.framework.com/startup";
        }
    }

    public static string APP_DOWNLOAD_URL
    {
        get;
        set;
    }

    public static string RES_DOWNLOAD_URL
    {
        get;
        set;
    }

    public static string LOGIN_URL
    {
        get;
        set;
    }

    // 错误日志上报URL
    public static string REPORT_ERROR_URL
    {
        get
        {
            return "https://oapi.dingtalk.com/robot/send?access_token=9848749207a29936a54e559b77be02c9293f5c04e90c6601776fc87b6bd39663";
        }
    }

    public static string SERVER_LIST_URL
    {
        get;
        set;
    }
    
    public static string NOTICE_URL
    {
        get;
        set;
    }
}
