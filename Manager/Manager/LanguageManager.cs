using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework.Base;

namespace Framework.Manager
{
    /*/===========================================================
     * 文本的有效格式 Text Key = Value;
     * Txt文本的编码格式必须为UTF-8的编码格式
     *      key 于 Value 形成键值对的格式
     *      key中不能有空格的出现
     *      key值只能有字母数字和下划线构成
     *      Value 值的前后不可有空格
     *      Value 值使用[]
    //===========================================================*/

    public class LanguageManager : DontManager<LanguageManager>
    {
        //private static LanguageManager m_Manager;
        //public static LanguageManager GetManager
        //{
        //    get
        //    {
        //        if (m_Manager == null)
        //        {
        //            GameObject tempGame = new GameObject("LanguageManager");
        //            m_Manager = tempGame.AddComponent<LanguageManager>();
        //            GameObject.DontDestroyOnLoad(tempGame);
        //        }
        //        return m_Manager;
        //    }
        //}

        private List<Action> CallBack = new List<Action>();
        /// <summary>
        /// 语言类型
        /// </summary>
        public enum LanguageType
        {
            /// <summary>
            /// 中文
            /// </summary>
            Chinese,
            /// <summary>
            /// 英文
            /// </summary>
            English,
            /// <summary>
            /// 韩文
            /// </summary>
            Korean,
            /// <summary>
            /// 日文
            /// </summary>
            Japanese,
            /// <summary>
            /// 德文
            /// </summary>
            German,
            /// <summary>
            /// 藏文
            /// </summary>
            Tibetan,
            /// <summary>
            /// 俄文
            /// </summary>
            Russian,
            /// <summary>
            /// 西班牙文
            /// </summary>
            Spanish,
            /// <summary>
            /// 其他语言
            /// </summary>
            Other
        }
        private LanguageType m_Language = LanguageType.Chinese;


        #region 公开方法

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetValueToKey(string Key)
        {
            if (!Dic_Language.ContainsKey(m_Language))
                return Key;
            Dictionary<string, string> Temp_Language = new Dictionary<string, string>();
            if (!Dic_Language.TryGetValue(m_Language, out Temp_Language))
                return Key;
            return GetValue(Key, Temp_Language);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string GetValueToKeys(params string[] Key)
        {
            if (Key == null)
                return null;
            if (Key.Length == 0)
                return null;
            string str = "";
            for (int i = 0; i < Key.Length; i++)
            {
                str += GetValueToKey(Key[i]);
            }
            return str;
        }
        /// <summary>
        /// 添加语言切换回调
        /// </summary>
        /// <param name="action"></param>
        public void Add(Action action)
        {
            if (CallBack.Contains(action))
                return;
            CallBack.Add(action);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        public LanguageType Setting_LanguageType
        {
            set
            {
                m_Language = value;
                for (int i = 0; i < CallBack.Count; i++)
                {
                    var fn = CallBack[i];
                    if (fn != null)
                        fn();
                }
            }
        }
        /// <summary>
        /// 初始化语言包
        /// </summary>
        /// <param name="language"></param>
        public void Init(params Language[] language)
        {
            ReadText(language);
        }

        #endregion


        protected Dictionary<LanguageType, Dictionary<string, string>> Dic_Language = new Dictionary<LanguageType, Dictionary<string, string>>();



        private void ReadText(Language[] varLanguage)
        {
            for (int i = 0; i < varLanguage.Length; i++)
            {
                Language Temp_Language = varLanguage[i];
                if (Dic_Language.ContainsKey(Temp_Language.m_Type))
                    continue;
                Dictionary<string, string> Temp_DIC_Language = new Dictionary<string, string>();

                ReadTextAsset(Temp_Language.m_Txt, Temp_DIC_Language);
                Dic_Language.Add(Temp_Language.m_Type, Temp_DIC_Language);

            }
        }
        /// <summary>
        /// 读取Txt文本
        /// </summary>
        /// <param name="varTextAsset"></param>
        /// <param name="varDic"></param>
        private void ReadTextAsset(TextAsset varTextAsset, Dictionary<string, string> varDic)
        {
            if (varTextAsset == null)
                return;
            string temp_Content = varTextAsset.text.Replace("\r\n", "\f");

            string[] varContent = temp_Content.Split('\f');
            if (varContent == null)
                return;
            List<string> LanguageList = RemovedNullString(varContent);
            if (LanguageList.Count == 0)
                return;
            for (int i = 0; i < LanguageList.Count; i++)
            {
                string Temp_Content = LanguageList[i];

                if (!Temp_Content.Contains("Text"))
                    continue;
                string Key;
                string Value = "";
                string[] TempLanguage = Temp_Content.Split('=');
                if (TempLanguage.Length < 2)
                    continue;
                Key = TempLanguage[0];
                if (!TestingKeyLegal(ref Key))
                    continue;
                for (int jx = 1; jx < TempLanguage.Length; jx++)
                {
                    if (jx == 1)
                        Value += TempLanguage[jx];
                    else
                        Value += "=" + TempLanguage[jx];
                }
                Value = TestingValueLegal(Value);
                if (!varDic.ContainsKey(Key))
                    varDic.Add(Key, Value);
            }
        }
        /// <summary>
        /// 移除数组中无效的字符串
        /// </summary>
        /// <param name="varContent"></param>
        /// <returns></returns>
        private List<string> RemovedNullString(string[] varContent)
        {
            List<string> LanguageList = new List<string>();
            for (int i = 0; i < varContent.Length; i++)
            {
                if (string.IsNullOrEmpty(varContent[i]))
                    continue;
                LanguageList.Add(varContent[i]);
            }
            return LanguageList;
        }
        /// <summary>
        /// 检测Key的合法性
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private bool TestingKeyLegal(ref string Key)
        {
            //string TempKey = Key;
            Key = Key.Trim();
            string[] content = Key.Split(' ');
            if (content.Length != 2)
                return false;
            if (content[0] != "Text")
                return false;
            Key = content[1];

            string TestingString = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
            char[] tempkey = Key.ToCharArray();
            for (int i = 0; i < tempkey.Length; i++)
            {
                char ms = tempkey[i];
                if (!TestingString.Contains(ms.ToString()))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 检测Value值的合法性
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        private string TestingValueLegal(string Value)
        {
            //先移除两头无效的空格
            Value = Value.Trim();
            //当值中存在中括号时，移除两头的中括号
            if (Value.Contains("[") && Value.Contains("]"))
            {
                Value = Value.Trim('[', ']');
            }
            return Value;
        }


        private string GetValue(string Key, Dictionary<string, string> varDic)
        {
            if (!varDic.ContainsKey(Key))
                return Key;
            string Value = "";
            if (varDic.TryGetValue(Key, out Value))
                return ClearSpilth(Value);
            return Key;
        }
        /// <summary>
        /// 移除多余的字符
        /// </summary>
        /// <param name="varValue"></param>
        /// <returns></returns>
        private string ClearSpilth(string varValue)
        {
            string Value = varValue;
            if (Value.Contains("\\n"))
                Value = Value.Replace("\\n", "\n");
            if (Value.Contains("\\r"))
                Value = Value.Replace("\\r", "\r");
            if (Value.Contains("\\\\"))
                Value = Value.Replace("\\\\", "\\");
            if (Value.Contains("\\t"))
                Value = Value.Replace("\\t", "\t");
            if (Value.Contains("\\v"))
                Value = Value.Replace("\\v", "\v");
            return Value;
        }


        public struct Language
        {
            public LanguageType m_Type;
            public TextAsset m_Txt;
        }
    }
}
