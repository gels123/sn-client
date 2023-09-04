using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestText : MonoBehaviour
{
    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        if (go != null)
        {
            TextMeshProUGUI tmp = go.gameObject.GetComponent<TextMeshProUGUI>();
            tmp.SetText("fsadfasdfadsfad");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
