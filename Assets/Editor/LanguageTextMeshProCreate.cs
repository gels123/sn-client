// using Configs;
// using Newtonsoft.Json;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using TMPro;
// using TMPro.EditorUtilities;
// using UnityEditor;
// using UnityEngine;
//
// public class LanguageTextMeshProCreate
// {
//     [MenuItem("GameObject/UI/LanguageTextMeshPro", priority = 0)]
//     public static void NewWhaleTextMeshPro(MenuCommand menuCommand)
//     {
//         GameObject obj = ObjectFactory.CreateGameObject("LanguageTextMeshPro", typeof(LanguageTextMeshPro));
//         //GameObject obj = new GameObject("WhaleTextMeshPro");//创建新物体
//         //obj.AddComponent<WhaleTextMeshPro>();
//
//         var text = obj.GetComponent<TextMeshProUGUI>();
//         text.text = "text";
//         text.transform.localPosition = Vector3.zero;
//
//         var parent = menuCommand.context as GameObject;
//         if (parent != null)
//         {
//             GameObjectUtility.SetParentAndAlign(obj, parent);//设置父节点为当前选中物体
//         }
//         Undo.RegisterCreatedObjectUndo(obj, "Create" + obj.name);//注册到Undo系统,允许撤销
//         Selection.activeObject = obj;//将新建物体设为当前选中物体
//     }
//
//     [MenuItem("Tools/LanguageTextMeshPro/ReloadLanguageConfig", priority = 0)]
//     public static void ReloadLangeTxt()
//     {
//         LanguageComponentEditorPanel.languageConfig = null;
//     }
//
//     //[MenuItem("Tools/ReplaceTextComponent", priority = 0)]
//     public static void ReplaceComponent()
//     {
//         var extensions = new List<string>() { ".prefab" };
//         var path = "Assets/AssetsPackage/UI";
//         var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
//             .Where(s => extensions.Contains(Path.GetExtension(s)?.ToLower())).ToArray();
//
//         foreach (var file in files)
//         {
//             //var t = AssetDatabase.LoadAssetAtPath<GameObject>(file);  f4688fdb7df04437aeb418b961361dc5    281e11fc6d7ae964bb10c481a4a14f64
//
//             string[] lines = System.IO.File.ReadAllLines(file);
//             for (int i = 0; i < lines.Length; i++)
//             {
//                 lines[i] = lines[i].Replace("f4688fdb7df04437aeb418b961361dc5", "281e11fc6d7ae964bb10c481a4a14f64");
//             }
//             File.WriteAllLines(file, lines);
//         }
//     }
// }
//
//
// [CustomEditor(typeof(LanguageTextMeshPro), true), CanEditMultipleObjects]
// public class LanguageComponentEditorPanel : TMP_EditorPanelUI
// {
//     static readonly GUIContent k_LanguageLabel = new GUIContent("language Id", "语言表ID");
//     static readonly GUIContent k_NoWarp = new GUIContent("NoWarp", "是否开启折行");
//     protected SerializedProperty m_LanguageIdProp;
//     protected SerializedProperty m_NoWarp;
//     protected LanguageTextMeshPro m_LanguageComponent;
//     protected override void OnEnable()
//     {
//         base.OnEnable();
//         m_LanguageIdProp = serializedObject.FindProperty("m_LanguageId");
//         m_NoWarp = serializedObject.FindProperty("m_NoWarp");
//         m_LanguageComponent = (LanguageTextMeshPro)target;
//     }
//
//     public override void OnInspectorGUI()
//     {
//         EditorGUI.BeginChangeCheck();
//         
//         EditorGUILayout.PropertyField(m_LanguageIdProp, k_LanguageLabel);
//         EditorGUILayout.PropertyField(m_NoWarp, k_NoWarp);
//         if (EditorGUI.EndChangeCheck())
//         {
//             //m_LanguageIdProp.intValue = 
//             m_LanguageComponent.languageId = m_LanguageIdProp.intValue;
//             m_LanguageComponent.NoWarp = m_NoWarp.boolValue;
//             m_LanguageComponent.text = LanguageIdChange(m_LanguageComponent.languageId);
//             m_HavePropertiesChanged = true;
//         }
//
//         base.OnInspectorGUI();
//     }
//
//     public static Dictionary<int, LanguageConfig> languageConfig = null;
//     public string LanguageIdChange(int languageId)
//     {
//         //UtilsTool.LanguageType type = UtilsTool.Instance.curLanguage;
//
//         if (languageConfig == null)
//         {
//             var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AssetsPackage/Config/language_config.json");
//             if (textAsset == null)
//             {
//                 Debug.LogError("load language config error");
//                 return "";
//             }
//             var jsObjList = JsonConvert.DeserializeObject<List<LanguageConfig>>(textAsset.text);
//             if (jsObjList == null)
//             {
//                 Debug.LogError("LanguageConfig json error");
//                 return "";
//             }
//
//             languageConfig = jsObjList.ToDictionary(item => item.Id, item => (LanguageConfig)item);
//
//             if (languageConfig == null)
//             {
//                 Debug.LogError("LanguageConfig error");
//                 return "";
//             }
//         }
//
//         if (languageConfig.TryGetValue(languageId, out var cfg))
//         {
//             return cfg.English;
//         }
//
//         return $"no found:{languageId}";
//     }
// }
