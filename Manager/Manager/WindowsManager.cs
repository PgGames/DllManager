using Framework.Base;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Framework.Help;

namespace Framework.Manager
{

    /// <summary>
    /// 窗口管理器（挂载在窗口上时可手动设置配置信息）
    /// 
    /// 持久化问题:挂载时可手动设置（默认为持久化状态）
    /// 
    /// 相机画布:挂载时可手动设置是否使用特殊相机（默认为使用）
    /// 
    /// </summary>
    public class WindowsManager :Manager<WindowsManager>
    {
        /// <summary>
        /// 画布（挂载在窗口上时需要手动设置）
        /// </summary>
        [SerializeField]
        protected Canvas m_Canvas;
        /// <summary>
        /// 画布的响应信息
        /// </summary>
        [SerializeField]
        protected EventSystem m_EventSystem;
        /// <summary>
        /// 相机
        /// </summary>
        [SerializeField]
        protected Camera m_Camera;
        /// <summary>
        /// 遮挡蒙版
        /// </summary>
        [SerializeField]
        protected Transform m_Mask;
        /// <summary>
        /// 废弃窗口
        /// </summary>
        [SerializeField]
        protected Transform m_WastWindows;
        /// <summary>
        /// 打开窗口
        /// </summary>
        [SerializeField]    
        protected Transform m_OpenWindows;
        /// <summary>
        /// 设置窗口是否持久化
        /// </summary>
        [SerializeField]
        protected bool IsDontDestroyOnLoad = true;
        /// <summary>
        /// 设置使用特殊相机
        /// </summary>
        [SerializeField]
        protected bool IsPeculiarCamera = true;

        /// <summary>
        /// 所有打开的窗口
        /// </summary>
        Dictionary<string, Windows> All_OpenWindow = new Dictionary<string, Windows>();
        /// <summary>
        /// 所有打开的窗口(用于计算蒙版位置)
        /// </summary>
        List<Windows> m_OpenWindow_List = new List<Windows>();
        /// <summary>
        /// 所有关闭的窗口且为被销毁
        /// </summary>
        Dictionary<string, Windows> All_WastWindow = new Dictionary<string, Windows>();
        /// <summary>
        /// 所有的窗口预制信息
        /// </summary>
        Dictionary<string, GameObject> All_Windows = new Dictionary<string, GameObject>();

        /// <summary>
        /// OnAwake代替Awake，在Awake之后Start之前调用
        /// </summary>
        protected override void OnAwake()
        {
            base.OnAwake();
        }
        /// <summary>
        /// 窗口初始化，在OnAwake之后Start之前调用
        /// </summary>
        protected override void Init()
        {
            CanvasInit();
            EventSystemInit();
            WindowsInit();

            if (IsDontDestroyOnLoad)
                GameObject.DontDestroyOnLoad(this.transform);
            base.Init();
        }

        #region 初始化信息

        /// <summary>
        /// 相机的初始化
        /// </summary>
        private void CameraInit(GameObject Parent)
        {
            if (m_Camera == null)
            {
                GameObject varCamera = new GameObject("UI_Camera") { layer = LayerMask.NameToLayer("UI") };
                varCamera.transform.SetParent(Parent.transform);
                varCamera.transform.localPosition = (Vector3.back) * 5000;
                Camera TempCamera = varCamera.AddComponent<Camera>();
                TempCamera.clearFlags = CameraClearFlags.Depth;
                TempCamera.orthographic = true;
                TempCamera.orthographicSize = 1;
                TempCamera.cullingMask = 32;
                TempCamera.depth = 100;
                TempCamera.allowHDR = false;
                TempCamera.allowMSAA = false;
                varCamera.AddComponent<FlareLayer>();
                m_Camera = TempCamera;
            }
        }
        /// <summary>
        /// 画布初始化
        /// </summary>
        private void CanvasInit()
        {
            if (m_Canvas == null)
            {
                GameObject TempWindows = new GameObject("Canvas") { layer = LayerMask.NameToLayer("UI") };
                TempWindows.transform.SetParent(this.transform);
                TempWindows.transform.position = new Vector3(100000, 100000, 100000);
                TempWindows.transform.localScale = Vector3.one;
                TempWindows.transform.localEulerAngles = Vector3.zero;

                //设置画布
                Canvas TempCanvas = TempWindows.AddComponent<Canvas>();
                
                CanvasScaler TempCanvasScaler = TempWindows.AddComponent<CanvasScaler>();
                TempCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                TempCanvasScaler.referenceResolution = new Vector2(1920, 1080);
                TempWindows.AddComponent<GraphicRaycaster>();

                if (!IsPeculiarCamera)
                {
                    TempCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    TempCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    TempCanvasScaler.matchWidthOrHeight = 0;
                }
                else
                {
                    TempCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                    TempCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    TempCanvas.referencePixelsPerUnit = 100;
                    CameraInit(TempCanvas.gameObject);

                    TempCanvas.worldCamera = m_Camera;
                    
                }
                m_Canvas = TempCanvas;
            }
        }
        /// <summary>
        /// 画布的响应事件初始化
        /// </summary>
        private void EventSystemInit()
        {
            if (m_EventSystem == null)
            {
                //设置画布响应
                GameObject TempEvent = Helper.NewGameObject("EventSystem", this.gameObject);
                m_EventSystem = TempEvent.AddComponent<EventSystem>();
                TempEvent.AddComponent<StandaloneInputModule>();
            }
        }

        /// <summary>
        /// 存放信息初始化
        /// </summary>
        private void WindowsInit()
        {
            //设置废弃窗口存放点
            if (m_WastWindows == null)
            {
                Transform TempWeast = NewGameObject("WastWindow", m_Canvas.transform);
                TempWeast.gameObject.SetActive(false);
                m_WastWindows = TempWeast;
            }
            //设置激活窗口存放点
            if (m_OpenWindows == null)
            {
                Transform TempOpen = NewGameObject("OpenWindow", m_Canvas.transform);
                TempOpen.gameObject.SetActive(true);
                m_OpenWindows = TempOpen;
            }
            //设置蒙版
            if (m_Mask == null)
            {
                Transform TempMask = NewGameObject("UIMask", m_OpenWindows);
                TempMask.gameObject.SetActive(false);

                Image TempUI = TempMask.gameObject.AddComponent<Image>();
                TempUI.color = new Color(0, 0, 0, 0.3f);

                m_Mask = TempMask;
            }
        }
        private Transform NewGameObject(string varName, Transform varParent)
        {
            GameObject TempGame = new GameObject(varName);
            RectTransform TempRect = TempGame.AddComponent<RectTransform>();

            TempRect.pivot = Vector2.one * 0.5f;
            TempRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            TempRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            TempRect.anchorMax = Vector2.one;
            TempRect.anchorMin = Vector2.zero;

            TempGame.transform.SetParent(varParent, false);
            TempGame.transform.localPosition = Vector3.zero;
            TempGame.transform.localScale = Vector3.one;
            TempGame.transform.localEulerAngles = Vector3.zero;
            TempGame.layer = LayerMask.NameToLayer("UI");

            return TempGame.transform;
        }

        #endregion

        #region public

        /// <summary>
        /// 添加一个Resources路径的窗口预制
        /// </summary>
        /// <param name="varName">窗口名称</param>
        /// <param name="varResourcesPath">预制的Resources路口</param>
        public void AddResourcesWindows(string varName, string varResourcesPath)
        {
            GameObject TempGame = Resources.Load<GameObject>(varResourcesPath);
            if (TempGame == null)
            {
                string str = string.Format("Windows Name:{0} Path:{1} does not exist", varName, varResourcesPath);
                Debug.LogWarning(str);
                return;
            }
            TempGame.SetActive(false);
            if (All_Windows.ContainsKey(varName))
            {
                string str = string.Format("Windows Name:{0} has been load", varName);
                Debug.Log(str);
                All_Windows[varName] = TempGame;
            }
            else
            {
                All_Windows.Add(varName, TempGame);
            }
        }
        /// <summary>
        /// 添加一个窗口预制
        /// </summary>
        /// <param name="varName">窗口名称</param>
        /// <param name="TempGame">窗口预制</param>
        public void AddWindows(string varName, GameObject TempGame)
        {
            if (TempGame == null)
            {
                string str = string.Format("Windows Name:{0} Windows does not exist", varName);
                Debug.LogWarning(str);
                return;
            }
            TempGame.SetActive(false);
            if (All_Windows.ContainsKey(varName))
            {
                string str = string.Format("Windows Name:{0} has been load", varName);
                Debug.Log(str);
                All_Windows[varName] = TempGame;
            }
            else
            {
                All_Windows.Add(varName, TempGame);
            }
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="varName">窗口名称</param>
        /// <param name="IsMask">是否使用蒙版</param>
        public GameObject OpenWindows(string varName, bool IsMask = false)
        {
            return Open_Winsows(varName, IsMask, true);
        }


        #endregion

        #region private

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="varName">窗口名称</param>
        /// <param name="IsMask">是否需要蒙版</param>
        /// <param name="IsLevel">是否参与顶层回调计算</param>
        /// <returns></returns>
        private GameObject Open_Winsows(string varName, bool IsMask,bool IsLevel)
        {
            Windows Temp_CurrentWindows = null;
            if (All_WastWindow.ContainsKey(varName))
            {
                //将窗口从垃圾站中移除
                Temp_CurrentWindows = All_WastWindow[varName];
                All_WastWindow.Remove(varName);
                All_OpenWindow.Add(varName, Temp_CurrentWindows);
            }
            else if (All_OpenWindow.ContainsKey(varName))
            {
                //获取窗口
                Temp_CurrentWindows = All_OpenWindow[varName];
            }
            else if (All_Windows.ContainsKey(varName))
            {
                //创建窗口
                Temp_CurrentWindows = new Windows();
                Temp_CurrentWindows.m_WindowType = varName;
                Temp_CurrentWindows.m_Windows = NewWindows(varName);
                if (Temp_CurrentWindows.m_Windows == null)
                    return null;
                //新建的窗口层级最大
                Temp_CurrentWindows.Level = int.MaxValue;
                All_OpenWindow.Add(varName, Temp_CurrentWindows);
            }
            else
            {
                //窗口不存在
                return null;
            }
            //设置窗口的蒙版信息
            Temp_CurrentWindows.m_IsMask = IsMask;
            //设置窗口的层数计算
            Temp_CurrentWindows.m_IsLevel = IsLevel;
            //将窗口打开置顶
            ActivateWindowsTop(Temp_CurrentWindows);

            return Temp_CurrentWindows.m_Windows;
        }
        /// <summary>
        /// 创建一个窗口
        /// </summary>
        /// <param name="varName">窗口名称</param>
        /// <returns></returns>
        private GameObject NewWindows(string varName)
        {
            if (!All_Windows.ContainsKey(varName))
            {
                Debug.LogError("Windows Name:"+varName+" not exist");
                return null;
            }
            GameObject TempGame = All_Windows[varName];
            GameObject Temp_Game = GameObject.Instantiate(TempGame);
            Temp_Game.name = TempGame.name;
            return Temp_Game;
        }
        /// <summary>
        /// 打开一个窗口置顶
        /// </summary>
        private void ActivateWindowsTop(Windows varWindows)
        {
            foreach (var item in All_OpenWindow)
            {
                if (item.Value.Level > varWindows.Level)
                {
                    item.Value.Level--;
                }
            }
            varWindows.m_Windows.gameObject.SetActive(true);
            varWindows.m_Windows.transform.SetParent(m_OpenWindows, false);
            varWindows.m_Windows.transform.SetAsLastSibling();
            varWindows.Level = All_OpenWindow.Count - 1;
        }
        /// <summary>
        /// 控制蒙版
        /// </summary>
        private void CommandMask()
        {
            m_Mask.gameObject.SetActive(false);
            List<Windows> TempList = new List<Windows>(All_OpenWindow.Values);
            TempList.Sort((s1, s2) => { return s1.Level - s2.Level; });
            if (TempList.Count > 0)
            {
                for (int i = TempList.Count - 1; i >= 0; i--)
                {
                    if (TempList[i].m_IsMask)
                    {
                        m_Mask.SetSiblingIndex(TempList[i].Level);
                        m_Mask.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        #endregion


        /// <summary>
        /// 窗口信息
        /// </summary>
        public class Windows
        {
            /// <summary>
            /// 窗口类型
            /// </summary>
            public string m_WindowType;
            /// <summary>
            /// 窗口的示例
            /// </summary>
            public GameObject m_Windows;
            /// <summary>
            /// 窗口是否有蒙版
            /// </summary>
            public bool m_IsMask;
            /// <summary>
            /// 窗口是否参与层数计算
            /// </summary>
            public bool m_IsLevel;
            /// <summary>
            /// 当前窗口位于的层数
            /// </summary>
            public int Level;
        }
    }
}
