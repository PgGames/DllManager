using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Event
{
    /// <summary>
    /// 事件类
    /// </summary>
    public abstract class EventClass
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public Type m_Type;
        /// <summary>
        /// 事件回调
        /// </summary>
        public Delegate CallBack;
    }
    /// <summary>
    /// 事件管理基类
    /// </summary>
    /// <typeparam name="TEventHead"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class AbstractEvent<TEventHead, TEvent> : MonoBehaviour where TEvent : EventClass, new()
    {
        private List<TEvent> events = new List<TEvent>();
        /// <summary>
        /// 注册信息
        /// </summary>
        /// <param name="varEvent"></param>
        protected void RigistEvent(TEvent varEvent)
        {
            var temp_Type = varEvent.m_Type;
            foreach (var item in events)
            {
                if (temp_Type == item.m_Type || temp_Type.IsSubclassOf(item.m_Type))
                {
                    if (varEvent.CallBack == item.CallBack)
                    {
                        Debug.Log("This type of listening event already exists.");
                        return;
                    }
                }
            }
            events.Add(varEvent);
        }
        /// <summary>
        /// 解除注册
        /// </summary>
        /// <param name="varEvent"></param>
        protected void UnregistEvent(TEvent varEvent)
        {
            var temp_Type = varEvent.m_Type;
            foreach (var item in events)
            {
                if (temp_Type == item.m_Type || temp_Type.IsSubclassOf(item.m_Type))
                {
                    if (varEvent.CallBack == item.CallBack)
                    {
                        events.Remove(item);
                        return;
                    }
                }
            }
            Debug.Log("This type of listening event does not exist.");
        }
        /// <summary>
        /// 事件通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varEvent"></param>
        public void TriggerEvent<T>(T varEvent) where T : TEventHead
        {
            int idx = 0;
            var temp_Type = varEvent.GetType();
            foreach (var item in events)
            {
                if (temp_Type == item.m_Type || temp_Type.IsSubclassOf(item.m_Type))
                {
                    CallEventDelegate(varEvent, item);
                    idx++;
                }
            }
            if (idx == 0)
                Debug.Log("This type of listening event does not exist.");
        }
        /// <summary>
        /// 执行回调
        /// </summary>
        /// <param name="varhead"></param>
        /// <param name="varevent"></param>
        protected abstract void CallEventDelegate(TEventHead varhead, TEvent varevent);
    }
}