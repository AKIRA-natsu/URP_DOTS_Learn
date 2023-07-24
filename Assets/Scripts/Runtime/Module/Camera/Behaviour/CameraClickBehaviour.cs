using UnityEngine;
using UnityEngine.InputSystem;

namespace AKIRA.Behaviour.Camera {
    /// <summary>
    /// 摄像机点击表现
    /// </summary>
    public class CameraClickBehaviour : CameraBehaviour {
        public override void GameUpdate() {
            if (Mouse.current.leftButton.wasPressedThisFrame) {
                Ray ray = CameraExtend.MainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, System.Single.MaxValue)) {
                    if (hit.transform.TryGetComponent<IClick>(out IClick click)) {
                        click.OnClick();
                    }
                }
            }
        }
    }
}