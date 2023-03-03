using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphCtrl : MonoBehaviour
{
    public Transform pointCubePrefab;
    public int solution = 200;
    private Dictionary<int, Transform> dict = new Dictionary<int, Transform>();

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;

        Debug.Log("==GraphCtrl:Start==");
        float step = 2f / solution;
        Vector3 scale = Vector3.one * step;
        for (int i = -solution; i <= solution; i++)
        {
            Transform point = Object.Instantiate(pointCubePrefab);
            point.SetParent(this.transform, false);
            Vector3 pos = Vector3.zero;
            pos.x = i * step;
            pos.y = Mathf.Sin(pos.x * Mathf.PI);
            Debug.Log("create pointCubePrefab i=" + i + " x=" + pos.x + " y=" + pos.y + " time=" + Time.time);
            point.localPosition = pos;
            point.localScale = scale;
            dict.Add(i, point);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("GraphCtrl:Update time=" + Time.time);
        foreach(KeyValuePair<int, Transform> kv in dict)
        {
            Vector3 pos = kv.Value.localPosition;
            pos.y = Mathf.Sin((pos.x + Time.time) * Mathf.PI);
            kv.Value.localPosition = pos;
        }
    }
}
