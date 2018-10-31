using Framework.Event;
using System;
using UnityEngine;

namespace Framework.Manager
{
    /// <summary>
    /// 临时的事件管理器（切换场景时事件会被清空）
    /// </summary>
    public class CurrentSceneEventManager : AbstractEvent<CurrentEventHead, CurrentSceneEventManager.CurrentEvent>
    {
        private static CurrentSceneEventManager m_instance;
        public static CurrentSceneEventManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    var go = new GameObject("CurrentSceneEventManager");
                    m_instance = go.AddComponent<CurrentSceneEventManager>();
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
        public CurrentEvent RigistEvent<T>(Action<T> callback) where T : CurrentEventHead
        {
            CurrentEvent current = new CurrentEvent();
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
        public void UnrigistEvent<T>(Action<T> callback) where T : CurrentEventHead
        {
            CurrentEvent current = new CurrentEvent();
            current.m_Type = typeof(T);
            current.CallBack = callback;

            base.UnregistEvent(current);    //解除注册
        }

        protected override void CallEventDelegate(CurrentEventHead varhead, CurrentEvent varevent)
        {
            varevent.CallBack.DynamicInvoke(varhead);
        }
        public class CurrentEvent : EventClass
        { }
    }
    public class CurrentEventHead
    { }
}
