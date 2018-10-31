﻿/****************************************************************
* 协助脚本用于设置物体颜色由上至下的渐变
***************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;




namespace Framework.UI
{
    /// <summary>
    /// 设置颜色的上下渐变
    /// </summary>
    [AddComponentMenu("Manager/UI/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        /// <summary>
        /// 上边的颜色
        /// </summary>
        [SerializeField]
        public Color32 topColor = Color.white;
        /// <summary>
        /// 下边的颜色
        /// </summary>
        [SerializeField]
        public Color32 bottomColor = Color.black;
        /// <summary>
        /// 每个字的顶点数
        /// </summary>
        private int m_Everyword = 6;

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
        }

        private void ApplyGradient(List<UIVertex> vertexList, int start, int end)
        {
            //下顶点
            float bottomY = vertexList[0].position.y;
            //上顶点
            float topY = vertexList[0].position.y;
            int idx = 0;
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
                    SettingEveryword(vertexList,topY, bottomY,idx);
                    if (i != end - 1)
                    {
                        topY = vertexList[i + 1].position.y;
                        bottomY = vertexList[i + 1].position.y;
                    }
                    idx = i + 1;
                }
            }
        }
        /// <summary>
        /// 设置每个字的颜色信息
        /// </summary>
        void SettingEveryword(List<UIVertex> vertexList,float topY,float bottomY,int idx)
        {
            float uiElementHeight = Mathf.Abs(topY - bottomY);
            Debug.LogError(uiElementHeight);
            for (int j = idx; j < idx + m_Everyword; j++)
            {
                UIVertex uiVertex = vertexList[j];
                uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - topY) / uiElementHeight);
                vertexList[j] = uiVertex;
            }
        }
    }
}