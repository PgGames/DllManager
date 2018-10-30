
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
    [AddComponentMenu("Manager/UI/Text")]
    [RequireComponent(typeof(Text), typeof(Text))]
    [ExecuteInEditMode]
    public class HelpText : MonoBehaviour
    {
        public Font font;
        [TextArea(3, 10)]
        public string text;
        public int fontSize = 14;
        public float lineSpacing = 1;
        public TextAnchor alignment = TextAnchor.UpperLeft;
        public Color m_Color = Color.white;


        [Header("文本类型")]
        public Type m_Type = Type.Text;
        [Header("Money Type")]
        public bool Isfloat = true;
        public Text m_text { private set; get; }


        private string Content;
        private RectTransform m_Rect;
        private Type losatType = Type.Text;


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
                if (m_Type == Type.Password)
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
            if (m_text.alignment != alignment)
            {
                m_text.alignment = alignment;
                return true;
            }
            if (m_text.fontSize != fontSize)
            {
                m_text.fontSize = fontSize;
                return true;
            }
            if (lineSpacing != m_text.lineSpacing)
            {
                m_text.lineSpacing = lineSpacing;
                return true;
            }
            if (font != m_text.font)
            {
                m_text.font = font;
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
            int lenght = m_text.text.Length;
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
                m_text.text = ((long)Sum).ToString();
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
        public enum Type
        {
            Password,
            Text,
            Name,
            Money
        }
    }
}
