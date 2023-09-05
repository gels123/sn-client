using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoWrapText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private float maxWidth;

    private Dictionary<string, int> specialDic = new Dictionary<string, int>(){["<sprite"] = 1};
    private List<string> spList = new List<string>();
    private bool isSpecialStr = false;
    void Start()
    {
        textMeshPro = transform.GetComponent<TextMeshProUGUI>();
        maxWidth = transform.GetComponent<RectTransform>().rect.width;
        InitText();
    }
    void OnEnable()
    {
        
    }

    public void InitText(){
        if (spList.Count > 0)
        {
            spList.Clear();
        }
        if (textMeshPro == null)
        {
            return;
        }
        textMeshPro.enableWordWrapping = false;
        string[] words = textMeshPro.text.Split(' ');
        string line = "";
        string tt = "";
        words = InitSpriteTextWords(words);
        for (int i = 0; i < words.Length; i++)
        {
            if (textMeshPro.GetPreferredValues(line + " " + words[i]).x > maxWidth)
            {
                var allWord = words[i];
                for (int b = 0; b < allWord.Length; b++)
                {
                    if (textMeshPro.GetPreferredValues(line + " " + allWord[b]).x > maxWidth)
                    {
                        if (b == 0)
                        {
                            tt += "\n" + allWord + " ";
                            line = allWord + " ";  
                            break;
                        }else{
                            //Debug.Log("--------------------*********"+ allWord);
                            var slStr = allWord.Substring(b);
                            tt += "-\n" + slStr + " ";
                            line = slStr + " ";
                            break;
                        }
                    }else{
                        tt += allWord[b];
                        line += allWord[b];
                    }
                }
               
            }
            else
            {
                tt += words[i] + " ";
                line += words[i] + " ";
            }
        }
        if (spList.Count > 0)
        {
            var newText = String.Format(tt, spList.ToArray());
            textMeshPro.text = newText;
        }else{
            textMeshPro.text = tt;
        }
        
    }
    string[] InitSpriteTextWords(string[] words){
        List<string> list = new List<string>();
        int index = 0;
        int spIndex = 0;
        int spType = 0;
        for (int i = 0; i < words.Length; i++)
        {
            var allWord = words[i];
            if (spType == 1)
            {
                spList[spList.Count - 1] += " " +allWord;
                spType = 0;
                continue;
            }
            if (allWord.Equals(""))
            {
                continue;
            }
            if (allWord.Contains("<sprite"))
            {
                list.Add("{"+ spIndex +"}");
                spIndex ++;
                spList.Add(allWord);
                spType = 1;
            }else if (allWord.Contains("<color")){
                list.Add("{"+ spIndex +"}");
                spIndex ++;
                spList.Add(allWord);
            }else if (allWord.Contains("<b")){
                list.Add("{"+ spIndex +"}");
                spIndex ++;
                spList.Add(allWord);
            }
            else{
                list.Add(allWord);
                index ++;
            }
        }
        return list.ToArray();
    }
}