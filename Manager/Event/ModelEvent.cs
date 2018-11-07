using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Event
{
    /// <summary>
    /// 模型的点击
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ModelEvent:MonoBehaviour
    {
        /// <summary>
        /// 点击模型回调-按下
        /// </summary>
        public ModelEvents onclick_down = new ModelEvents();
        /// <summary>
        /// 点击模型回调-按住
        /// </summary>
        public ModelEvents onclick_center = new ModelEvents();
        /// <summary>
        /// 点击模型回调-抬起
        /// </summary>
        public ModelEvents onclick_up = new ModelEvents();

        private bool IsClick = false;

        private void OnMouseDown()
        {
            onclick_down.Invoke(this.gameObject);
            OnClick_Down();
            IsClick = true;
            StartCoroutine(MouseCenter());
        }
        private IEnumerator MouseCenter()
        {
            while (IsClick)
            {
                onclick_center.Invoke(this.gameObject);
                OnClick_Center();
                yield return new WaitForFixedUpdate();
            }
        }
        private void OnMouseUp()
        {
            IsClick = false;
            OnClick_Up();
            onclick_up.Invoke(this.gameObject);
        }
        /// <summary>
        /// 点击按下
        /// </summary>
        protected virtual void OnClick_Down()
        {
        }
        /// <summary>
        /// 点击按住
        /// </summary>
        protected virtual void OnClick_Center()
        {
        }
        /// <summary>
        /// 点击抬起
        /// </summary>
        protected virtual void OnClick_Up()
        {
        }



        public class ModelEvents : UnityEvent<GameObject>
        {
            public ModelEvents() { }
        }
        //public class MouseEvents : UnityEvent
        //{
        //    public MouseEvents() { }
        //}
    }
}
