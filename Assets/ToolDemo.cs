using System.IO;

using UnityEditor;
using UnityEngine;
public enum TexType : byte
{
    PNG, JPG, TGA
}
public class ToolDemo : MonoBehaviour
{
    private int ID_DepthTexture;
    private int ID_InvSize;


    public Shader hzbShader;
    private Material hzbMat;

    private static ToolDemo _instance;
    public static ToolDemo Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        hzbMat = new Material(hzbShader);
    }



    private void OnGUI()
    {
        if (hzbMat != null)
        {
            if (GUILayout.Button("������������Դ"))
            {
                string Texpath = "Assets/demo/Grass072_A.tga";
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(Texpath);
                if (tex == null)
                {
                    Debug.Log($"û�м��ص���Դ = {Texpath}");
                    return;
                }
                ///�������ɵ�ͼΪ�ļ���
                string path = Path.Combine(Application.persistentDataPath, "Screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                ///����Ϊԭ���1/2
                ReduceTexture(tex, 2, path);

            }
        }
    }

    // ��ͼƬ�ļ��ֽ����ݴ��� Texture2D
    public Texture2D CreateTextureFromImageBytes(byte[] imageData)
    {
        Texture2D tex = new Texture2D(2, 2); // ��ʱ�ߴ磬����ʱ���Զ�����
        if (tex.LoadImage(imageData)) // �Զ�ʶ��PNG/JPG�ȸ�ʽ
        {
            return tex;
        }
        else
        {
            Debug.LogError("Failed to load texture from byte array");
            return null;
        }
    }

    // ʹ��ʾ����
    //byte[] pngBytes = File.ReadAllBytes("path/to/image.png");
    //Texture2D myTexture = CreateTextureFromImageBytes(pngBytes);
    public  void ReduceTexture(string texPath, int factor, string savePath)
    {
        try
        {
            byte[] dt = File.ReadAllBytes(texPath);
           
            Texture2D tex = CreateTextureFromImageBytes(dt);

            ReduceTexture(tex, factor, savePath);
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public void ReduceTexture(Texture2D tex, int factor, string path)
    {
        int w = tex.width / factor;
        int h = tex.height / factor;

        ID_DepthTexture = Shader.PropertyToID("_DepthTexture");
        ID_InvSize = Shader.PropertyToID("_InvSize");

        hzbMat.SetVector(ID_InvSize, new Vector4(1.0f / w, 1.0f / h, 0, 0));
        hzbMat.SetTexture(ID_DepthTexture, tex);

        ///������ʱ��RT
        RenderTexture tempRT = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.Default);
        tempRT.filterMode = FilterMode.Point;
        ///��texִ����С��д�뵽tempRT
        Graphics.Blit(null, tempRT, hzbMat);

        ///ִ��ת��ͱ���
        SaveRenderTextureToPNG(path, tempRT, TexType.TGA);
    }



    /// <summary>
    /// ����
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="targetTexture"></param>
    /// <param name="texType">��ʽ  </param>
    public void SaveRenderTextureToPNG(string filePath, RenderTexture targetTexture, TexType texType)
    {
        // ����1��������ʱTexture2D
        Texture2D tmpTex = new Texture2D(
            targetTexture.width,
            targetTexture.height,
            TextureFormat.RGBA32,
            false
        );

        // ����2�����浱ǰRenderTexture״̬
        RenderTexture prevActive = RenderTexture.active;

        // ����3������Ŀ��RenderTextureΪ��ǰ�״̬
        RenderTexture.active = targetTexture;

        // ����4����ȡ�������ݵ�Texture2D
        tmpTex.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        tmpTex.Apply(); // Ӧ�����ظ���

        // ����5���ָ�֮ǰ��RenderTexture״̬
        RenderTexture.active = prevActive;

        byte[] data = null;
        switch (texType)
        {
            case TexType.PNG:
                data = tmpTex.EncodeToTGA();
                break;
            case TexType.JPG:
                data = tmpTex.EncodeToJPG();
                break;
            case TexType.TGA:
                data = tmpTex.EncodeToTGA();
                break;
            default:
                break;
        }
        // ����6������ΪPNG

        if (data != null)
        {
            // ����7��д���ļ�
            File.WriteAllBytes(filePath, data);
            // ����8��������ʱTexture
            Destroy(tmpTex);
            Debug.Log($"PNG saved to: {filePath}");
        }

    }

}
