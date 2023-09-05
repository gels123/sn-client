using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("UI/LanguageTextMeshProUGUI", 1)]
[ExecuteAlways]
public class LanguageTextMeshPro : TextMeshProUGUI
{
    private string language = "";
    private string textkey = "";
    private object[] textargs = null;
    
    protected override void Start()
    {
        base.Start();
        if(Application.isPlaying) {
            Messeger.AddListener(MessageName.MSG_LANGUAGE_UPDATE, (Callback<string>)OnLanguageChange);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Application.isPlaying)
        {
            Messeger.RemoveListener(MessageName.MSG_LANGUAGE_UPDATE, (Callback<string>)OnLanguageChange);
        }
    }
    
    public void OnLanguageChange(string lang)
    {
        if (language != lang)
        {
            language = lang;
            SetText(textkey, textargs);
        }
    }
    
    public void SetText(string key, params object[] args)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        textkey = key;
        if (args != null && args.Length > 0)
        {
            textargs = args;
        }

        string text = (textargs != null && textargs.Length > 0 ? string.Format(textkey, textargs) : textkey);
        base.SetText(text);
    }
}
