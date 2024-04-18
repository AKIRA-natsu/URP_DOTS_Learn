using AKIRA;
using AKIRA.Attribute;
using UnityEngine;

/// <summary>
/// 摄像机标签
/// </summary>
public class CameraTagAuthoring : MonoBehaviour {
    /// <summary>
    /// 标签
    /// </summary>
    [SerializeField]
    [SelectionPop(typeof(GameData.Camera))]
    private string cameraTag;

    private void Awake() {
        CameraExtend.AddCamera(cameraTag, this.gameObject);
        GameObject.Destroy(this);
    }
}