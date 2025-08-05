using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ToolUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Button createBtn;
    public InputField pathTxt;

    private void OnEnable()
    {
        createBtn.onClick.AddListener(CreateHandler1);
        //createBtn.onClick.AddListener(CreateHandler);
    }

    private void CreateHandler1()
    {
        string Texpath = string.IsNullOrEmpty(pathTxt.text) ? "Assets/demo/Grass072_A.tga" : pathTxt.text;

        ///�������ɵ�ͼΪ�ļ���
        string path = Path.Combine(Application.persistentDataPath, "Screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
        ///����Ϊԭ���1/2
        ToolDemo.Instance.ReduceTexture(Texpath, 2, path);
    }

    private void CreateHandler()
    {
        string Texpath = string.IsNullOrEmpty(pathTxt.text) ? "Assets/demo/Grass072_A.tga" : pathTxt.text;


        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(Texpath);

        if (tex == null)
        {
            Debug.Log($"û�м��ص���Դ = {Texpath}");
            return;
        }
        ///�������ɵ�ͼΪ�ļ���
        string path = Path.Combine(Application.persistentDataPath, "Screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
        ///����Ϊԭ���1/2
        ToolDemo.Instance.ReduceTexture(tex, 2, path);
    }

}
