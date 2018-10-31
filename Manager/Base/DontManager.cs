
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
    }
}
