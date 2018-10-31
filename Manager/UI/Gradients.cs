/****************************************************************
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
    //[AddComponentMenu("Manager/UI/Gradients")]
    [System.Obsolete("Please use Gradient Instead of Gradients")]
    class Gradients : BaseMeshEffect
    {
        [SerializeField]
        public Color32 topColor = Color.white;
        [SerializeField]
        public Color32 bottomColor = Color.black;

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
            float bottomY = vertexList[0].position.y;
            float topY = vertexList[0].position.y;
            for (int i = start; i < end; ++i)
            {
                float y = vertexList[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }

            float uiElementHeight = topY - bottomY;
            for (int i = start; i < end; ++i)
            {
                UIVertex uiVertex = vertexList[i];
                uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
                vertexList[i] = uiVertex;
            }
        }
    }
}
