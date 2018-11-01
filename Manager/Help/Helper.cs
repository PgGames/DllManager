using UnityEngine;
using Framework.Base;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml.Serialization;

namespace Framework.Help
{
    /// <summary>
    /// 帮助工具
    /// </summary>
    public class Helper:DontManager<Helper>
    {
        /// <summary>
        /// 存储文件
        ///     当文件夹路径不存在时会自动创建文件夹
        /// </summary>
        /// <param name="path">存储路径</param>
        /// <param name="date">储存数据</param>
        /// <returns>存储文件成功或失败</returns>
        public static bool SaveFile(string path, byte[] date)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || date == null)
                    return false;
                string[] vs = path.Split('/');
                string paths = null;
                for (int i = 0; i < vs.Length - 1; i++)
                {
                    if (i != 0)
                        paths += "/" + vs[i];
                    else
                        paths += vs[i];
                }
                //判断文件夹是否存在
                if (!Directory.Exists(paths))
                    Directory.CreateDirectory(paths);
                FileStream stream = new FileStream(path, FileMode.Create);
                stream.Write(date, 0, date.Length);
                stream.Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="varPath">文件路径</param>
        /// <returns></returns>
        public static bool FileExists(string varPath)
        {
            if (string.IsNullOrEmpty(varPath))
                return false;
            FileInfo file = new FileInfo(varPath);
            return file.Exists;
        }


        /// <summary>
        /// 从物体的上级(父级)查找组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var_Tran"></param>
        /// <returns></returns>
        public static T GetComponent<T>(Transform var_Tran) where T : Component
        {
            if (var_Tran == null)
            {
                Debug.LogError("Tran Is Null");
                return null;
            }
            Transform Parent = var_Tran.parent;
            if (Parent == null)
                return var_Tran.GetComponent<T>();
            T var_T = Parent.GetComponent<T>();
            if (var_T == null)
                return GetComponent<T>(Parent);
            else
                return var_T;
        }
        /// <summary>
        /// 获取物体下的所有子集（不包含子集的子集）
        /// </summary>
        /// <param name="varTran"></param>
        /// <returns></returns>
        public static List<GameObject> GetGames(Transform varTran)
        {
            if (varTran == null)
            {
                Debug.Log("Tran is Null");
                return null;
            }
            List<GameObject> TrmpList = new List<GameObject>();
            int TempChildCount = varTran.childCount;
            for (int i = 0; i < TempChildCount; i++)
            {
                Transform TempTran = varTran.GetChild(i);
                TrmpList.Add(TempTran.gameObject);
            }
            return TrmpList;
        }
        /// <summary>
        /// 设置物体的状态
        /// </summary>
        /// <param name="varGame"></param>
        /// <param name="path"></param>
        /// <param name="activeSelf"></param>
        public static void SetActive(GameObject varGame, bool activeSelf, string path=null)
        {
            if (varGame == null)
            {
                Debug.LogError("Game is null");
                return;
            }
            if (!string.IsNullOrEmpty(path))
            {
                var Tran = varGame.transform.Find(path);
                if (Tran == null)
                {
                    Debug.LogError("Game path is null");
                    return;
                }
                Tran.gameObject.SetActive(activeSelf);
            }
            else
            {
                varGame.SetActive(activeSelf);
            }
        }



        /// <summary>
        /// 创建物体
        /// </summary>
        /// <param name="varName">物体的名称</param>
        /// <param name="varParent">物体的父级</param>
        /// <param name="varLayer">物体的标签层级</param>
        /// <returns></returns>
        public static GameObject NewGameObject(string varName, GameObject varParent,string varLayer = "UI")
        {
            GameObject TempGame = new GameObject(varName);
            TempGame.transform.SetParent(varParent.transform);
            TempGame.transform.localPosition = Vector3.zero;
            TempGame.transform.localScale = Vector3.one;
            TempGame.transform.localEulerAngles = Vector3.zero;
            TempGame.layer = LayerMask.NameToLayer(varLayer);

            return TempGame;
        }
        /// <summary>
        /// 创建物体并挂载脚本
        /// </summary>
        /// <typeparam name="T">挂载的脚本类型</typeparam>
        /// <param name="varName">物体的名称</param>
        /// <param name="varParent">物体的父级</param>
        /// <param name="varLayer">物体的标签层级</param>
        /// <returns></returns>
        public static T NewGameObject<T>(string varName, GameObject varParent,string varLayer = "UI") where T : Component
        {
            GameObject TempGame = NewGameObject(varName, varParent);
            T Temp = TempGame.AddComponent<T>();
            return Temp;
        }


        #region 文件解析
        /// <summary>
        /// 解析Xml文件
        /// * ===============================
        /// * Xml文件的编写结构
        /// * 
        /// * ===============================
        /// * <!--<?xml version="1.0"?>
        /// * <LogoXmlList>
        /// *   <LogoXmlinfo id="1" png="one"	/>
        /// *   <LogoXmlinfo id="2" png="two"	/>
        /// * </LogoXmlList>
        /// * -->
        /// </summary>
        /// <typeparam name="T">
        /// * ===============================================
        /// * Xml用于解析的函数编写结构
        /// *          数据值用 属性变量编写
        /// * 如下为Xml解析结构的编写示例
        /// * ===============================================
        /// * public class LogoXmlinfo
        /// * {
        /// *     [XmlAttribute(AttributeName = "id")]
        /// *     public string id { set; get; }
        /// *     [XmlAttribute(AttributeName = "png")]
        /// *     public string Png { set; get; }
        /// * }
        /// * [XmlRoot("LogoXmlList")]
        /// * public class LogoXmlList
        /// * {
        /// *     [XmlElement("LogoXmlinfo")]
        /// *     public LogoXmlinfo[] Ins { set; get; }
        /// * }
        /// * ===============================================
        /// </typeparam>
        /// <param name="path"></param>
        /// <param name="ms"></param>
        public static void LoadXml<T>(string path, ref T ms) where T : class
        {
            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var items = (T)serializer.Deserialize(reader);
                    if (items != null)
                    {
                        ms = items;
                    }
                }
            }
            catch (Exception ex)
            {
                string str = string.Format("error: {0} is error {1}", path, ex.Message);
                Debug.LogError(str);
            }
        }
        /// <summary>
        /// 解析Json文件
        /// * ===============================
        /// * Json文件的编写结构
        /// * 
        /// * ===============================
        /// </summary>
        /// <typeparam name="T">
        /// * Json用于解析的函数编写结构
        /// * 数据值用 值变量编写不可用属性变量
        /// * 如下为Json解析结构的编写示例
        /// * =============================================
        /// *  public class JsonNotice
        /// *  {
        /// *      public string success;
        /// *      public string text;
        /// *  }
        /// * ============================================= 
        /// </typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T LoadJson<T>(string content) where T : class
        {
            content = content.Replace('\'', '"');
            Debug.LogWarning(content);
            T TempT = JsonUtility.FromJson<T>(content);
            return TempT;
        }

        #endregion
        
        #region 数字转换
        /// <summary>
        /// 将数字转换成中文
        /// </summary>
        /// <param name="varNum"></param>
        /// <returns></returns>
        public static string Number(uint varNum)
        {

            string Temp = Number(varNum, 0);
            if (Temp.Length > 16)
                return "零";
            string[] Temp_Str = new string[4];
            string Temp_string = "";
            for (int i = 3; i >= 0; i--)
            {

                if (Temp.Length < i * 4)
                {
                    Temp_Str[i] = "";
                    continue;
                }
                else
                {
                    if (Temp.Length > (i + 1) * 4)
                        Temp_Str[i] = Temp.Substring((Temp.Length - (i + 1) * 4), 4);
                    else
                        Temp_Str[i] = Temp.Substring(0, (Temp.Length - i * 4));
                }
                string str = Temp_Str[i];
                Temp_Str[i] = Number(str);
                //1000万0000亿0000万0000
                if (Temp_Str[i] != null)
                {
                    if (i == 3)
                        Temp_Str[i] += "万";
                    if (i == 2)
                        Temp_Str[i] += "亿";
                    if (i == 1)
                        Temp_Str[i] += "万";
                }
                Temp_string += Temp_Str[i];
            }
            while (Temp_string.Contains("零亿") || Temp_string.Contains("零万") || Temp_string.Contains("零千") || Temp_string.Contains("零百") || Temp_string.Contains("零十") || Temp_string.Contains("零零") || Temp_string.Contains("亿万"))
            {
                Temp_string = Temp_string.Replace("零亿", "亿零");
                Temp_string = Temp_string.Replace("零万", "万零");
                Temp_string = Temp_string.Replace("零千", "零");
                Temp_string = Temp_string.Replace("零百", "零");
                Temp_string = Temp_string.Replace("零十", "零");
                Temp_string = Temp_string.Replace("零零", "零");
                Temp_string = Temp_string.Replace("亿万", "亿");
            }
            Temp_string = Temp_string.TrimEnd('零');
            //10000
            return Temp_string;
        }
        private static string Number(uint varNum, int idx)
        {

            uint Temp_shi = varNum / 10;
            uint Tempge = varNum % 10;
            string Temp = "零";
            switch (Tempge)
            {
                case 0:
                    Temp = "零";
                    break;
                case 1:
                    Temp = "一";
                    break;
                case 2:
                    Temp = "二";
                    break;
                case 3:
                    Temp = "三";
                    break;
                case 4:
                    Temp = "四";
                    break;
                case 5:
                    Temp = "五";
                    break;
                case 6:
                    Temp = "六";
                    break;
                case 7:
                    Temp = "七";
                    break;
                case 8:
                    Temp = "八";
                    break;
                case 9:
                    Temp = "九";
                    break;
            }
            if (Temp_shi > 0)
                Temp = Number(Temp_shi, idx + 1) + Temp;
            return Temp;
        }
        private static string Number(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "零";
            char[] car = str.ToCharArray();
            string TempStr = "";
            for (int i = 3; i >= 0; i--)
            {
                char s = '0';
                if (((car.Length - 1) - i) < 0)
                {
                    TempStr = "";
                    continue;
                }
                else
                    s = car[(car.Length - 1) - i];


                if (i == 3)
                    TempStr = TempStr + s + "千";
                else if (i == 2)
                    TempStr = TempStr + s + "百";
                else if (i == 1)
                    TempStr = TempStr + s + "十";
                else if (i == 0)
                    TempStr = TempStr + s;
            }
            return TempStr;
        }

        #endregion


        #region Private


        

        #endregion

    }
}
