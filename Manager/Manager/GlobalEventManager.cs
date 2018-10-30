using Framework.Event;
using System;
using UnityEngine;

namespace Framework.Manager
{
    public class GlobalEventManager : AbstractEvent<GlobalEventHead, GlobalEventManager.GlobalEvent>
    {
        private static GlobalEventManager m_instance;
        public static GlobalEventManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    var go = new GameObject("GlobalEventManager");
                    m_instance = go.AddComponent<GlobalEventManager>();
                    GameObject.DontDestroyOnLoad(go);
                }
                return m_instance;
            }
        }
        private void OnDestroy()
        {
            m_instance = null;
        }

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        /// <returns></returns>
        public GlobalEvent RigistEvent<T>(Action<T> callback) where T : GlobalEventHead
        {
            GlobalEvent current = new GlobalEvent();
            current.m_Type = typeof(T);
            current.CallBack = callback;

            base.RigistEvent(current);      //注册
            return current;
        }
        /// <summary>
        /// 解除事件监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public void UnrigistEvent<T>(Action<T> callback) where T : GlobalEventHead
        {
            GlobalEvent current = new GlobalEvent();
            current.m_Type = typeof(T);
            current.CallBack = callback;

            base.UnregistEvent(current);    //解除注册
        }

        protected override void CallEventDelegate(GlobalEventHead varhead, GlobalEvent varevent)
        {
            varevent.CallBack.DynamicInvoke(varhead);
        }
        public class GlobalEvent : EventClass
        { }
    }
    public class GlobalEventHead
    { }
}
