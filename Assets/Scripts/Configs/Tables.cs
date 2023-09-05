/// <summary>
/// 配置数据管理
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace cfg
{
    public class TablesEx : Tables
    {
        // 单例
        private static TablesEx instance;
        public static TablesEx Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TablesEx();
                    if (instance != null)
                    {
                        instance.Init();
                    }
                }
                return instance;
            }
        }
        
        // 初始化
        public void Init()
        {
            
        }
        
        public void Dispose()
        {
            if (instance != null)
            {
                instance = null;
            }
        }
    }
}
