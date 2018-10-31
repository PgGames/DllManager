using Framework.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    /// <summary>
    /// 根据Key值获取其对应的Value值与LanguageManager联合使用
    /// </summary>
    [AddComponentMenu("Manager/UI/TextValue")]
    [RequireComponent(typeof(Text))]
    public class HelpTxtValueToKey : MonoBehaviour
    {
        [SerializeField]
        private string Key;
        /// <summary>
        /// 
        /// </summary>
        public Text m_Content;

        private void Awake()
        {
            LanguageManager.GetManager.Add(GetValueToKey);
            if (m_Content == null)
                m_Content = this.transform.GetComponent<Text>();
            GetValueToKey();
        }
        /// <summary>
        /// 设置Key值并更新Value
        /// </summary>
        /// <param name="varKey"></param>
        public void SettingKey(string varKey)
        {
            Key = varKey;
            GetValueToKey();
        }
        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                GetValueToKey();
            }
        }
        private void GetValueToKey()
        {
            string Value = LanguageManager.GetManager.GetValueToKey(Key);
            if (m_Content != null)
                m_Content.text = Value;
        }
    }
}
