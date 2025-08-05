using System.Collections.Generic;
using UnityEngine;

public class TestDelDel : MonoBehaviour
{
    // Start is called before the first frame update
    private Dictionary<int, int> dic = new Dictionary<int, int>();
    void Start()
    {
        dic = new Dictionary<int, int>();
        dic.Add(0, 0);
        dic.Add(1, 1);
        dic.Add(2, 2);
        dic.Add(3, 3);
        dic.Add(4, 4);
        dic.Add(5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        TestValue();

    }

    private void TestValue()
    {
        if (dic.ContainsKey(0))
        {
           // Debug.Log(1);
        }
        else
        {
           // Debug.Log(0);
        }
    }
}
