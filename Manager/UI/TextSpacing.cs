using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    /// <summary>
    /// 字间距修改工具
    /// 不支持Text中的alignment的横向对其修改
    /// Text的lineSpacing必须大于0.675或者小于-0.675
    /// </summary>
    [AddComponentMenu("Manager/UI/TextSpacing")]
    [RequireComponent(typeof(Text), typeof(Text))]
    public class TextSpacing : BaseMeshEffect
    {
        /// <summary>
        /// 字间距
        /// </summary>
        public float _Text_Spacing = 0f;

        /// <summary>
        /// 每个字的顶点数
        /// </summary>
        private int m_Everyword = 6;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vh"></param>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);
            int count = vertexList.Count;
            if (count == 0)
            {
                return;
            }
            ApplyGradient(vertexList, 0, count);
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
            //if (!IsActive() || vh.currentVertCount == 0)
            //{
            //    return;
            //}
            //List<UIVertex> vertex = new List<UIVertex>();
            //vh.GetUIVertexStream(vertex);
            //int indexCount = vh.currentIndexCount;
            //UIVertex vt;
            //for (int i = 6; i < indexCount; i++)
            //{
            //    vt = vertex[i];
            //    vt.position += new Vector3(_Text_Spacing * (i / 6), 0, 0);
            //    vertex[i] = vt;
            //    if (i % 6 <= 2)
            //    {
            //        vh.SetUIVertex(vt, (i / 6) * 4 + i % 6);
            //    }
            //    if (i % 6 == 4)
            //    {
            //        vh.SetUIVertex(vt, (i / 6) * 4 + i % 6 - 1);
            //    }

            //}
        }
        private void ApplyGradient(List<UIVertex> vertexList, int start, int end)
        {
            //下顶点
            float bottomY = vertexList[0].position.y;
            //上顶点
            float topY = vertexList[0].position.y;
            int idx = 0;
            //行数
            int line = 1;
            //当前行的字数
            int CurrIdx = 0;
            //上一字体的高度
            float LoadTopY = 0;
            for (int i = start; i < end; ++i)
            {
                float y = vertexList[i].position.y;
                if (y > bottomY)
                {
                    bottomY = y;
                }
                else if (y < topY)
                {
                    topY = y;
                }
                if ((i + 1) % m_Everyword == 0)
                {
                    //第一个字体记录高度
                    if (idx == 0)
                    {
                        LoadTopY = topY;
                    }
                    else if (IsLine(topY, bottomY, LoadTopY))
                    {
                        line += 1;
                        CurrIdx = 0;
                    }
                    SettingEveryword(vertexList, CurrIdx, idx);
                    //更新上一字体的高度
                    LoadTopY = topY;
                    //更新顶点信息
                    idx = i + 1;
                    //更新该行的字体数量
                    CurrIdx++;
                    //更新字体高度
                    if (i != end - 1)
                    {
                        topY = vertexList[i + 1].position.y;
                        bottomY = vertexList[i + 1].position.y;
                    }
                }
            }
        }
        /// <summary>
        /// 判断是否换行
        /// </summary>
        /// <returns></returns>
        private bool IsLine(float topY, float bottomY, float LoadTopY)
        {
            //字体所占高度
            float uiElementHeight = Mathf.Abs(topY - bottomY);
            //当前字体与上一字体间的高度差
            float uilineHeight = Mathf.Abs(topY - LoadTopY);
            if (uilineHeight > uiElementHeight)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置每个字体信息
        /// </summary>
        /// <param name="vertexList"></param>
        /// <param name="curridx">该字符位于该行的第几个字符</param>
        /// <param name="idx">该字符的初始顶点数</param>
        void SettingEveryword(List<UIVertex> vertexList,int curridx, int idx)
        {
            if (idx == 0)
                return;
            for (int j = idx; j < idx + m_Everyword; j++)
            {
                UIVertex vt = vertexList[j];
                vt.position += new Vector3(_Text_Spacing * curridx, 0, 0);
                vertexList[j] = vt;
            }
        }
    }
}
