using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// URP 塞入主摄像头Camera Stack脚本
/// </summary>
[RequireComponent(typeof(UnityEngine.Camera))]
public class CameraStackAdditionHelp : MonoBehaviour {

    private void Start() {
        StartCoroutine(WaitToAddMainCamera());
    }

    /// <summary>
    /// 等待添加UI摄像机到主摄像机
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToAddMainCamera() {
        while (Camera.main == null)
            yield return null;

        var data = Camera.main.GetUniversalAdditionalCameraData();
        data.cameraStack.Add(this.GetComponent<Camera>());
    }
}