using UnityEngine;
using Framework.Base;
using UnityEngine.Events;
using System.Collections;
using System.IO;
using System;

namespace Framework.Help
{
    /// <summary>
    /// 保存图片
    /// </summary>
    public class SavePicture: DontManager<SavePicture>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="varSuccess">是否保存成功</param>
        /// <param name="ImagePath">图片路径</param>
        public delegate void mothod(bool varSuccess, string ImagePath);
        
        private string m_path = "";
        /// <summary>
        /// 存储图片的文件夹路径
        /// </summary>
        public string SavePath
        {
            set {
                string path = value;
                try
                {
                    if(Application.platform == RuntimePlatform.Android)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android")) + "/DCIM/Camera";
                        }
                        else
                            path = value;
                    }
                    //移除尾部的分割线
                    path = path.TrimEnd('/');
                    if(Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    m_path = path;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Setting Save Iamge Path Error: "+ ex.Message);
                }
            }
            get {
                return m_path;
            }
        }
        /// <summary>
        /// 保存图片（当前界面的截图）
        /// 安卓默认保存到相册
        /// </summary>
        /// <param name="varMothod">存储回调</param>
        public void SaveImage(mothod varMothod = null)
        {
            if (string.IsNullOrEmpty(m_path))
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    m_path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android")) + "/DCIM/Camera";
                }
                else
                    m_path = Application.persistentDataPath + "/Image";
            }
            StartCoroutine(SavePNG(m_path, varMothod));
        }
        IEnumerator SavePNG(string varPath, mothod varMothod)
        {
            //等待界面渲染完成
            yield return new WaitForEndOfFrame();
            string tempPath = "";
            try
            {
                int width = Screen.width;
                int height = Screen.height;
                // 创建一个屏幕大小的纹理，RGB24 位格（24位格没有透明通道，32位的有）  
                Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                // 读取屏幕内容到我们自定义的纹理图片中  
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                // 保存前面对纹理的修改  
                tex.Apply();
                // 编码纹理为JPG格式数据  
                byte[] bytes = tex.EncodeToJPG();
                // 销毁无用的图片纹理  
                GameObject.Destroy(tex);
                string ImageName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                tempPath = varPath + "/" + ImageName + ".jpg";
                //当文件夹不存在时先创建文件夹在保存图片
                if (!Directory.Exists(varPath))
                    Directory.CreateDirectory(varPath);
                //存储图片
                File.WriteAllBytes(tempPath, bytes);
            }
            catch (Exception ex)
            {
                Debug.LogError("Save Image Error:" + ex.Message);
                tempPath = null;
            }
            if (!string.IsNullOrEmpty(tempPath))
            {
                float time = 0;
                do
                {
                    if (time >= 0.5f)
                        break;
                    time += 0.002f;
                    yield return new WaitForFixedUpdate();
                }
                while (!Helper.FileExists(tempPath));

                if (Application.platform == RuntimePlatform.Android)
                {
                    //刷新相册
                    string[] path = new string[1];
                    path[0] = tempPath;
                    using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    {
                        AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
                        using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
                        {
                            Conn.CallStatic("scanFile", playerActivity, path, null, null);
                        }
                    }
                }
                if (Helper.FileExists(tempPath))
                    varMothod?.Invoke(true, tempPath);
                else
                    varMothod?.Invoke(false, null);
            }
            else
                varMothod?.Invoke(false, null);
        }

        /// <summary>
        /// 保存图片（当前界面的截图）
        /// </summary>
        /// <param name="varPath">保存路径(文件夹路径)安卓默认为相册路径</param>
        /// <param name="varMothod">保存回调</param>
        public void SaveImage(string varPath, mothod varMothod = null)
        {
            if (string.IsNullOrEmpty(varPath))
            {
                if (Application.platform == RuntimePlatform.Android)
                    varPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android")) + "/DCIM/Camera";
                else
                    varPath = Application.persistentDataPath + "/Image";
            }
            varPath = varPath.TrimEnd('/');
            StartCoroutine(SavePNG(varPath, varMothod));
        }
    }
}
