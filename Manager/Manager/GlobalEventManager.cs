using Framework.Event;
using System;
using UnityEngine;

namespace Framework.Manager
{
    /// <summary>
    /// 永久性事件管理器（切换场景时不会被清理）
    /// </summary>
    public class GlobalEventManager : AbstractEvent<GlobalEventHead, GlobalEventManager.GlobalEvent>
    {
        private static GlobalEventManager m_instance;
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 事件广播回调
        /// </summary>
        /// <param name="varhead"></param>
        /// <param name="varevent"></param>
        protected override void CallEventDelegate(GlobalEventHead varhead, GlobalEvent varevent)
        {
            varevent.CallBack.DynamicInvoke(varhead);
        }
        /// <summary>
        /// 事件类
        /// </summary>
        public class GlobalEvent : EventClass
        { }
    }
    /// <summary>
    /// 事件定义必须继承该类(用作传输数据内容)
    /// </summary>
    public class GlobalEventHead
    { }
}
