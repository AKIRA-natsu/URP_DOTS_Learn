using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// <para>视野表现</para>
    /// <para>来源：https://www.bilibili.com/video/BV1NM411Y7HC/?vd_source=b6cbace78cacac5f94717102502c11d0</para>
    /// </summary>
    public class FieldOfView : MonoBehaviour
    {
        // 发射视野位置
        [SerializeField]
        private Transform eye;
        // 扫描角度
        [SerializeField]
        private float angle = 45f;
        // 扫描线数量
        [SerializeField]
        [Min(0)]
        private int count = 5;
        [SerializeField]
        [Min(0)]
        private int opposite = 2;
        // 扫描距离
        [SerializeField]
        private float distance = 3f;

        [Space]
        // 扫描对象
        [SerializeField]
        private LayerMask mask;
        // 扫描颜色
        [SerializeField]
        private Color scanColor = Color.white;

        // 扫描组件
        [SerializeField]
        private MeshFilter scanFilter;
        // 扫描模型
        [SerializeField]
        private Mesh scanModel;
        // 模型顶点
        [SerializeField]
        private Vector3[] vertices;
        // 模型三角面
        [SerializeField]
        private List<int> triangles = new();
        // 使用面显示
        [SerializeField]
        private bool useVertice = false;

        // 扫描列表
        [Space]
        public List<Transform> scans = new();

        private void OnDrawGizmos() {
            DrawView();
        }

        /// <summary>
        /// 绘制视野
        /// </summary>
        private void DrawView() {
            if (eye == null)
                return;

            try {
                if (count * opposite != vertices.Length + 1)
                    CreateScanMesh();
                
                Gizmos.color = scanColor;

                scans = new();
                RaycastHit hit = default;
                for (int j = 0; j < opposite; j++) {
                    for (int i = 0; i < count; i++) {
                        eye.localEulerAngles = Vector3.zero;
                        eye.Rotate(Vector3.forward * (360f / opposite) * j);
                        eye.Rotate(Vector3.up * angle * (i / (float)count));

                        Vector3 endPosition;
                        if (Physics.Raycast(eye.position, eye.forward, out hit, distance, mask)) {
                            endPosition = hit.point;
                            // Gizmos.DrawLine(eye.position, hit.point);
                            // if (!scans.Contains(hit.transform))
                            //     scans.Add(hit.transform);
                            
                            // vertices[j * count + i] = hit.point;
                        } else {
                            endPosition = eye.position + eye.forward * distance;
                            // Gizmos.DrawLine(eye.position, eye.position + eye.forward * distance);
                            
                            // vertices[j * count + i] = hit.point;
                        }

                        Gizmos.DrawLine(eye.position, endPosition);
                        vertices[j * count + i] = this.transform.InverseTransformPoint(endPosition);

                    }
                }

                scanModel = new()
                {
                    vertices = vertices,
                    triangles = triangles.ToArray(),
                };

                scanFilter.mesh = scanModel;
            } catch {  }

        }

        /// <summary>
        /// 生成扫描模型
        /// </summary>
        public void CreateScanMesh() {
            int verticeCount = count * opposite;
            // 绘制正反两面
            triangles = new();
            vertices = new Vector3[verticeCount + 1];
            if (useVertice) {
                for (int j = 0; j < opposite; j++) {
                    for (int i = 0; i < count - 1; i++) {
                        triangles.Add(verticeCount);
                        triangles.Add(i + j * count);
                        triangles.Add(i + 1 + j * count);

                        triangles.Add(verticeCount);
                        triangles.Add(i + 1 + j * count);
                        triangles.Add(i + j * count);
                    }
                }
            } else {
                for (int j = 0; j < opposite; j++) {
                    triangles.Add(verticeCount);
                    if (j < opposite - 1) {
                        triangles.Add((j + 2) * count - 1);
                    } else {
                        triangles.Add(count - 1);
                    }
                    triangles.Add((j + 1) * count - 1);
                }

                for (int j = 0; j < opposite; j++) {
                    for (int i = 0; i < count - 1; i++)
                    {
                        triangles.Add(i + j * count);
                        triangles.Add(i + 1 + j * count);
                        if (j < opposite - 1) {
                            triangles.Add(i + 1 + (j + 1) * count);
                        } else {
                            triangles.Add(i + 1);
                        }

                        triangles.Add(i + j * count);
                        if (j < opposite - 1) {
                            triangles.Add(i + 1 + (j + 1) * count);
                            triangles.Add(i + (j + 1) * count);
                        } else {
                            triangles.Add(i + 1);
                            triangles.Add(i);
                        }
                    }
                }
            }
        }
    }
}
