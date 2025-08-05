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
            if (GUILayout.Button("生成缩减的资源"))
            {
                string Texpath = "Assets/demo/Grass072_A.tga";
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(Texpath);
                if (tex == null)
                {
                    Debug.Log($"没有加载到资源 = {Texpath}");
                    return;
                }
                ///保存生成的图为文件，
                string path = Path.Combine(Application.persistentDataPath, "Screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                ///缩减为原理的1/2
                ReduceTexture(tex, 2, path);

            }
        }
    }

    // 从图片文件字节数据创建 Texture2D
    public Texture2D CreateTextureFromImageBytes(byte[] imageData)
    {
        Texture2D tex = new Texture2D(2, 2); // 临时尺寸，加载时会自动调整
        if (tex.LoadImage(imageData)) // 自动识别PNG/JPG等格式
        {
            return tex;
        }
        else
        {
            Debug.LogError("Failed to load texture from byte array");
            return null;
        }
    }

    // 使用示例：
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

        ///生成临时的RT
        RenderTexture tempRT = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.Default);
        tempRT.filterMode = FilterMode.Point;
        ///把tex执行缩小，写入到tempRT
        Graphics.Blit(null, tempRT, hzbMat);

        ///执行转码和保存
        SaveRenderTextureToPNG(path, tempRT, TexType.TGA);
    }



    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="targetTexture"></param>
    /// <param name="texType">格式  </param>
    public void SaveRenderTextureToPNG(string filePath, RenderTexture targetTexture, TexType texType)
    {
        // 步骤1：创建临时Texture2D
        Texture2D tmpTex = new Texture2D(
            targetTexture.width,
            targetTexture.height,
            TextureFormat.RGBA32,
            false
        );

        // 步骤2：保存当前RenderTexture状态
        RenderTexture prevActive = RenderTexture.active;

        // 步骤3：设置目标RenderTexture为当前活动状态
        RenderTexture.active = targetTexture;

        // 步骤4：读取像素数据到Texture2D
        tmpTex.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        tmpTex.Apply(); // 应用像素更改

        // 步骤5：恢复之前的RenderTexture状态
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
        // 步骤6：编码为PNG

        if (data != null)
        {
            // 步骤7：写入文件
            File.WriteAllBytes(filePath, data);
            // 步骤8：清理临时Texture
            Destroy(tmpTex);
            Debug.Log($"PNG saved to: {filePath}");
        }

    }

}
