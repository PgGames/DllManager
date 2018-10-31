
/*-----------------------------------
 *------------[功能脚本]-------------*  
 *----创建时间：2017/9/11
 *----脚本功能：对UGUI组件中的Text组件进行优化
 *              脚本仅支持文字内容的中途变化
 *        [Password]---类型对输入的文字进行密码掩饰
 *        [Text]-------类型显示原有文字不做任何掩饰
 *        [Name]-------类型根据文本框的长度进行操作超出用“…”进行掩饰
 *        [Money]------类型根据数值大小显示万、亿
 *----脚本兼容：
 *        该脚本支持组件---[Outline]
 *        该脚本支持组件---[Gradient]
 *        该脚本支持组件---[Shadow]
 *        该脚本支持组件---[TextSpacing]
 *        该脚本支持组件---[Text]
 *-----------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Framework.UI
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Manager/UI/Text")]
    [RequireComponent(typeof(Text), typeof(Text))]
    [ExecuteInEditMode]
    public class HelpText : MonoBehaviour
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        [TextArea(3, 10)]
        public string text;
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color m_Color = Color.white;


        /// <summary>
        /// 文本类型
        /// </summary>
        public Type m_Type = Type.Text;
        /// <summary>
        /// 金币类型是否采用浮点数显示
        /// </summary>
        public bool Isfloat = true;
        /// <summary>
        /// 显示文本
        /// </summary>
        public Text m_text { private set; get; }


        private Font font;
        private int fontSize = 14;
        private float lineSpacing = 1;
        private TextAnchor alignment = TextAnchor.UpperLeft;
        private string Content;
        private RectTransform m_Rect;
        private Type losatType = Type.Text;
        private float m_RectWidth =0;
        private float m_RectHeight=0;


        private void Awake()
        {
            m_text = gameObject.GetComponent<Text>();
            m_Rect = gameObject.GetComponent<RectTransform>();


            font = m_text.font;
            losatType = m_Type;
        }

        void Update()
        {
            if (losatType != m_Type || text != Content || IsUpdate())
            {
                if (m_Type == Type.Null)
                    m_text.text = text;
                else if (m_Type == Type.Password)
                    PasswordText();
                else if (m_Type == Type.Name)
                    NameText();
                else if (m_Type == Type.Money)
                    MoneyText();
                else
                    TextText();
                losatType = m_Type;
                Content = text;
            }
        }

        private bool IsUpdate()
        {
            if (m_text == null)
            {
                m_text = gameObject.GetComponent<Text>();
                m_Rect = gameObject.GetComponent<RectTransform>();
            }
            if (m_RectHeight != m_Rect.rect.height)
            {
                m_RectHeight = m_Rect.rect.height;
                return true;
            }
            if (m_RectWidth != m_Rect.rect.width)
            {
                m_RectWidth = m_Rect.rect.width;
                return true;
            }
            if (m_text.alignment != alignment)
            {
                alignment = m_text.alignment;
                return true;
            }
            if (m_text.fontSize != fontSize)
            {
                fontSize = m_text.fontSize;
                return true;
            }
            if (lineSpacing != m_text.lineSpacing)
            {
                lineSpacing = m_text.lineSpacing;
                return true;
            }
            if (font != m_text.font)
            {
                font = m_text.font;
                return true;
            }
            if (m_Color != m_text.color)
            {
                m_text.color = m_Color;
                return true;
            }
            return false;
        }





        /// <summary>
        /// 密码文字
        /// </summary>
        private void PasswordText()
        {
            m_text.text = text;
            int lenght = m_text.text.Length;
            if (lenght >= 6)
                lenght = 6;
            string content = "";
            for (int i = 0; i < lenght; i++)
            {
                content += "*";
            }
            m_text.text = content;
        }
        /// <summary>
        /// 文本文字
        /// </summary>
        private void TextText()
        {
            m_text.text = text;

            float temp_Height = m_text.preferredHeight;
            int lenght = m_text.text.Length;
            //实际大小超出应有大小
            if (temp_Height > m_Rect.rect.height)
            {
                do
                {
                    lenght--;
                    if (lenght <= 0)
                    {
                        m_text.text = null;
                        return;
                    }
                    string content = text.Substring(0, lenght);
                    m_text.text = content + "...";


                    temp_Height = m_text.preferredHeight;
                }
                while (temp_Height > m_Rect.rect.height);
            }
        }
        /// <summary>
        /// 昵称文字
        /// </summary>
        private void NameText()
        {
            m_text.text = text;
            float temp_Width = m_text.preferredWidth;

            if (temp_Width > m_Rect.rect.width)
            {
                float width = temp_Width / text.Length;
                int sum = (int)(m_Rect.rect.width / width);
                string content = text.Substring(0, sum);
                m_text.text = content + "...";
                try
                {
                    while (m_text.preferredWidth > m_Rect.rect.width)
                    {
                        sum--;
                        content = text.Substring(0, sum);
                        m_text.text = content + "...";
                    }
                }
                catch (Exception e)
                {
                    m_text.text = text;
                    Debug.LogError(e.Message);
                }
            }
            else
            {
                m_text.text = text;
            }
        }
        /// <summary>
        /// 金币文字
        /// </summary>
        private void MoneyText()
        {
            float Sum = 0;
            float.TryParse(text, out Sum);
            if (Mathf.Abs(Sum) < 10000)
            {
                if (Isfloat)
                    if (Sum % 1 != 0)
                        m_text.text = string.Format("{0:N2}", Sum);
                    else
                        m_text.text = string.Format("{0:N0}", Sum);
                else
                    m_text.text = string.Format("{0}", (long)Sum);
            }
            else if ((Mathf.Abs(Sum) >= 10000) && (Mathf.Abs(Sum) < 100000000))
            {
                float a = Sum / 10000.0f;
                if (Isfloat)
                {
                    if (Sum % 10000 >= 50 && Sum % 10000 <= 9950)
                        m_text.text = string.Format("{0:N2}万", a);
                    else
                        m_text.text = string.Format("{0:N0}万", a);
                }
                else
                    m_text.text = string.Format("{0}万", (long)a);
            }
            else if ((Mathf.Abs(Sum) >= 100000000) && (Mathf.Abs(Sum) < 1000000000000))
            {
                float a = Sum / 100000000.0f;
                if (Isfloat)
                    if (Sum % 100000000.0f >= 10000 * 50 && Sum % 10000 <= 10000 * 9950)
                        m_text.text = string.Format("{0:N2}亿", a);
                    else
                        m_text.text = string.Format("{0:N0}亿", a);
                else
                    m_text.text = string.Format("{0}亿", (long)a);
            }
            else if (Mathf.Abs(Sum) >= 1000000000000)
            {
                float a = Sum / 100000000.0f;
                int idx = 0;
                while (true)
                {
                    idx++;
                    a = a / 10.0f;
                    if (Mathf.Abs((float)a) < 10)
                        break;
                }
                if (Isfloat)
                    m_text.text = string.Format("{0:N2}^{1}亿", a, idx);
                else
                    m_text.text = string.Format("{0}^{1}亿", (long)a, idx);
            }
        }

        
        /// <summary>
        /// 文本类型
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 默认
            /// 显示最原本的Text不做任何修改
            /// </summary>
            Null,
            /// <summary>
            /// 密码
            /// 使用*代替字符且最多显示6个*
            /// </summary>
            Password,
            /// <summary>
            /// 文本
            /// 超出文本框的内容用...显示
            /// </summary>
            Text,
            /// <summary>
            /// 昵称
            /// 只会显示一行多出的字体用...显示
            /// </summary>
            Name,
            /// <summary>
            /// 金币
            /// 只显示数字且会显示..万或..亿
            /// 采用浮点数是会保留两位小数
            /// </summary>
            Money
        }
    }
}
