/// <summary>
/// 配置数据管理
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

namespace cfg
{
    public partial class Tables
    {
        // 初始化
        public void Init()
        {
            
        }
        
        // 单例
        private static Tables instance;
        public static Tables Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Tables(loader);
                    if (instance != null)
                    {
                        instance.Init();
                    }
                }
                return instance;
            }
        }
        
        private static JSONNode loader(string fileName)
        {
            return JSON.Parse(File.ReadAllText(Application.streamingAssetsPath +"/Json/" + fileName + ".json", System.Text.Encoding.UTF8));
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
