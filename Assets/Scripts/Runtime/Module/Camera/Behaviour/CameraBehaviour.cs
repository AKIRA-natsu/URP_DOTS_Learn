using AKIRA;
using UnityEngine;

namespace AKIRA.Behaviour.Camera {
    /// <summary>
    /// 摄像机表现基类
    /// </summary>
    public abstract class CameraBehaviour : MonoBehaviour/*, IUpdate*/ {
        // 更新模式
        // [SerializeField]
        // protected UpdateMode mode = UpdateMode.Update;

        protected virtual void Awake() {
            CameraExtend.RegistCameraBehaviour(this);
        }

        protected virtual void OnEnable() {
            // this.Regist(GameData.Group.Camera, mode);
        }

        protected virtual void OnDisable() {
            // this.Remove(GameData.Group.Camera, mode);
        }

        protected virtual void OnDestroy() {
            CameraExtend.RemoveCameraBehaviour(this);
        }

        public abstract void GameUpdate();
    }
}