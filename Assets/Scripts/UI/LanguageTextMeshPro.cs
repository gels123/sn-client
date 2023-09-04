// using System;
// using System.Collections;
// using Configs;
// using Framework.Common;
// using Newtonsoft.Json;
// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using UnityEditor;
// using UnityEngine;
//
// [AddComponentMenu("UI/LanguageTextMeshPro", 1)]
// [ExecuteAlways]
// public class LanguageTextMeshPro : TextMeshProUGUI
// {
//     private int[] m_IdArray = null;
//     // 对话打字机速度
//     //public float m_TyepSpeed = 0.03f;
//     private Coroutine m_PrintCoroutine;
//     private NoWrapText _noWrapCom;
//     protected override void Start()
//     {
//         base.Start();
//         if (NoWarp)
//         {
//             _noWrapCom = transform.GetOrAddComponent<NoWrapText>();
//         }
//
//         if(Application.isPlaying){
//             EventManager.Instance.AddListener(GameEvent.GE_LanguageChange, OnEventLanguageChange);
//
//             if (m_LanguageId > 0)
//             {
//                 SetLanguageId(m_LanguageId);
//             }
//
//             if (m_IdArray != null)
//             {
//                 SetLanguageId(m_IdArray);
//             }
//         }
//
//     }
//
//     protected override void OnDestroy()
//     {
//         base.OnDestroy();
//         if(Application.isPlaying){
//             EventManager.Instance.RemoveListener(GameEvent.GE_LanguageChange, OnEventLanguageChange);
//         }
//     }
//     public void OnEventLanguageChange(GameEvent ge, EventParameter parameter)
//     {
//         if (m_LanguageId > 0)
//         {
//             SetLanguageId(m_LanguageId);
//         }
//         if (m_IdArray != null)
//         {
//             SetLanguageId(m_IdArray);
//         }
//     }
//     
//     public int languageId
//     {
//         get => m_LanguageId;
//         set => m_LanguageId = value; 
//     }
//
//     [SerializeField]
//     private int m_LanguageId = -1;
//     public bool NoWarp {
//         get => m_NoWarp;
//         set => m_NoWarp = value;  
//     }
//     [SerializeField]
//     private bool m_NoWarp = false;
//
//     public void SetLanguageId(String strId)
//     {
//         try
//         {
//             int nId = int.Parse(strId);
//             m_IdArray = null;
//             m_LanguageId = nId;
//             text = UtilsTool.GetLanguageId(nId);
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//
//     }
//     public void SetLanguageId(int id)
//     {
//         m_IdArray = null;
//         m_LanguageId = id;
//         text = UtilsTool.GetLanguageId(id);
//         UseNoWarpText();
//     }
//     public string SetLanguageId(int id, params string[] strArray)
//     {
//         m_IdArray = null;
//         m_LanguageId = id;
//         text = UtilsTool.GetLanguageId(id);
//         text = String.Format(text, strArray);
//         return text;
//     }
//
//     public void UseNoWarpText(){
//         if (_noWrapCom)
//         {
//             _noWrapCom.InitText();
//         }
//     }
//
//     //含参数的第一个放到最前面
//     public string SetLanguageId(params int[] idArray)
//     {
//         m_IdArray = idArray;
//         m_LanguageId = -1;
//         text = UtilsTool.GetLanguageId(m_IdArray[0]);
//         if (m_IdArray.Length > 1)
//         {
//             string[] currTextArray = new string[m_IdArray.Length - 1];
//             for (int i = 1; i < m_IdArray.Length; i++)
//             {
//                 currTextArray[i] = UtilsTool.GetLanguageId(m_IdArray[i]);
//             }
//             text = String.Format(text, currTextArray);
//         }
//
//         return text;
//     }
//    
//     public void SetPrintByLanguageId(Action callBack,int id, params string[] strArray)
//     {
//         var strContent = SetLanguageId(id,strArray);
//         if (m_PrintCoroutine != null)
//         {
//             StopCoroutine(m_PrintCoroutine);
//         }
//         m_PrintCoroutine = StartCoroutine(Print_Text(strContent,callBack));
//     }
//     
//     public void SetPrintLanguageId(Action callBack,params int[] idArray)
//     {
//         if (m_PrintCoroutine != null)
//         {
//             StopCoroutine(m_PrintCoroutine);
//         }
//         text = String.Empty;
//         var strContent = SetLanguageId(idArray);
//         m_PrintCoroutine = StartCoroutine(Print_Text(strContent,callBack));
//     }
//     IEnumerator Print_Text(string content,Action callBack)
//     {
//         yield return new WaitForEndOfFrame();
//         yield return new WaitForEndOfFrame();
//         text = "";
//         for (int i = 0; i < content.Length; i++)
//         {
//             text += content[i];
//             yield return new WaitForSeconds(0.03f);
//         }
//         if (callBack != null)
//         {
//             callBack();
//         }
//     }
// }
