
using UnityEngine;

namespace Framework.Base
{
     /// <summary>
     /// 单利类型的基类(切换场景时单利不会被清楚)
     /// </summary>
     /// <typeparam name="T"></typeparam>
    public class DontManager<T> : MonoBehaviour where T : Component
    {
        private static T _Manager;
        /// <summary>
        /// 
        /// </summary>
        public static T GetManager
        {
            get
            {
                if (_Manager == null)
                {
                    var Temp_Type = typeof(T).Name;
                    GameObject Go = new GameObject(Temp_Type);
                    GameObject.DontDestroyOnLoad(Go);
                    _Manager = Go.AddComponent<T>();
                }
                return _Manager;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static T Managers()
        {
            if (_Manager == null)
            {
                var Temp_Type = typeof(T).Name;
                GameObject Go = new GameObject(Temp_Type);
                GameObject.DontDestroyOnLoad(Go);
                _Manager = Go.AddComponent<T>();
            }
            return _Manager;
        }
        void OnDestroy()
        {
            _Manager = null;
        }
        void Awake()
        {
            if (_Manager == null)
                _Manager = this.GetComponent<T>();
            OnAwake();
            Init();
        }
        /// <summary>
        /// 使用OnAwake代替Awake
        /// </summary>
        protected virtual void OnAwake()
        {
        }
        /// <summary>
        /// 初始化信息在Awake之后调用
        /// </summary>
        protected virtual void Init()
        {
        }
    }
}
