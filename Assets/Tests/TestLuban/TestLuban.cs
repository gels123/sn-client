using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class TestLuban : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("== TestLuban Start ==" + Application.streamingAssetsPath);
        // var tables = new cfg.Tables(LoadByteBuf);
        // Debug.Log("============sdfadsf=====" + tables.TbItem[10001].ToString());
        // Debug.Log("============l10n=====" + tables.TbItem[10001].Name);
        
        Debug.Log(cfg.TablesEx.Instance.TbItem[10001].ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private static JSONNode LoadByteBuf(string file)
    {
        return JSON.Parse(File.ReadAllText(Application.streamingAssetsPath +"/json/" + file + ".json", System.Text.Encoding.UTF8));
    }
}
