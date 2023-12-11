using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Behaviour.AI {
    /// <summary>
    /// AI控制器
    /// </summary>
    public partial class CharacterNavigationController : AIAgent {
        // // 视野内的侵略者
        // private GameObject invader;
        // // 移动速度
        // public float moveSpeed = 1f;
        // // 旋转速度
        // public float turnSpeed = 1f;

        public override void Recycle(object data = null) {
            StopUpdate();
        }

        public override void Wake(object data = null) {
            BeginUpdate();
        }
        
        public override void GameUpdate() {
            base.GameUpdate();

        //     // FIXME: 测试视野
        //     if (this.TryGetComponent<FieldView>(out FieldView view)) {
        //         if (view.ViewRayHit(out RaycastHit hit)) {
        //             hit.transform.Log();
        //         }
        //     }
        }

        public override void OnUpdateStop() {}

        public override void OnUpdateResume() {}

        // /// <summary>
        // /// 是否朝向侵略者
        // /// </summary>
        // /// <returns></returns>
        // private bool IsFacingInvader() {
        //     if (invader == null)
        //         return false;
        //     Vector3 v1 = invader.transform.position - this.transform.position;
        //     v1.y = 0;
        //     return Vector3.Angle(this.transform.forward, v1) < 1;
        // }

        // /// <summary>
        // /// 转向侵略者
        // /// </summary>
        // private void RotateToInvader() {
        //     if (invader == null)
        //         return;
        //     Vector3 v1 = invader.transform.position - this.transform.position;
        //     v1.y = 0;
        //     Vector3 cross = Vector3.Cross(this.transform.forward, v1);
        //     float angle = Vector3.Angle(this.transform.forward, v1);
        //     this.transform.Rotate(cross, Mathf.Min(turnSpeed, Mathf.Abs(angle)));
        // }
        
        // /// <summary>
        // /// 朝向方向
        // /// </summary>
        // /// <param name="rotation"></param>
        // private void RotateToDirection(Quaternion rotation) {
        //     Quaternion.RotateTowards(this.transform.rotation, rotation, turnSpeed);
        // }

        // /// <summary>
        // /// 是否到达目标
        // /// </summary>
        // /// <param name="position"></param>
        // /// <returns></returns>
        // private bool Reach(Vector3 position) {
        //     Vector3 v = position - this.transform.position;
        //     v.y = 0;
        //     return v.magnitude < 0.05;
        // }

        // /// <summary>
        // /// 移动到目标
        // /// </summary>
        // /// <param name="position"></param>
        // private void MoveToPosition(Vector3 position) {
        //     Vector3 v = position - this.transform.position;
        //     v.y = 0;
        //     this.transform.position += v.normalized * moveSpeed * Time.deltaTime;
        // }
    }
}
