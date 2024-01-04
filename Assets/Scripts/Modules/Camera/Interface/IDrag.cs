using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 拖拽接口
/// </summary>
public interface IDrag {
    /// <summary>
    /// 鼠标点击
    /// </summary>
    void OnDragDown();
    /// <summary>
    /// 鼠标抬起
    /// </summary>
    void OnDragUp();
    /// <summary>
    /// 鼠标按住
    /// </summary>
    void OnDrag();
}

/// <summary>
/// 拖拽扩展
/// </summary>
public static class DragExtend {
    /// <summary>
    /// 获得拖拽坐标
    /// </summary>
    /// <param name="drag"></param>
    /// <param name="worldPosition"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Vector3 GetDragPosition(this IDrag drag, Vector3 worldPosition, float height = 1) {
        var dis = (CameraExtend.Transform.position - worldPosition).magnitude;
        var mouseCurrentPosition = Mouse.current.position.ReadValue();
        var mousePosition = new Vector3(mouseCurrentPosition.x, mouseCurrentPosition.y, dis);
        var pos = CameraExtend.MainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 fwd = CameraExtend.Transform.forward;
        float k = (height - pos.y) / fwd.y;
        pos += fwd * k;
        return pos;
    }
}