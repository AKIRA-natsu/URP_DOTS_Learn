using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机扩展
/// </summary>
public static class CameraExtend {
    private static Camera mainCamera;
    /// <summary>
    /// 主摄像机
    /// </summary>
    /// <value></value>
    public static Camera MainCamera {
        get {
            if (mainCamera == null)
                mainCamera = Camera.main;
            return mainCamera;
        }
    }

    /// <summary>
    /// Transform
    /// </summary>
    public static Transform Transform => MainCamera.transform;

    /// <summary>
    /// 是否存在主摄像机
    /// </summary>
    public static bool ExistMainCamera => MainCamera != null;

    /// <summary>
    /// 摄像机标签
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="GameObject"></typeparam>
    /// <returns></returns>
    private static Dictionary<string, GameObject> TagMap = new Dictionary<string, GameObject>();
    
    /// <summary>
    /// 添加摄像机
    /// </summary>
    /// <param name="tag"></param>
    internal static void AddCamera(string tag, GameObject camera) {
        if (TagMap.ContainsKey(tag)) {
            $"{camera} add error: Camera contains tag => {tag}".Error();
        } else {
            $"{camera} add, tag => {tag}".Log();
            TagMap.Add(tag, camera);
        }
    }

    /// <summary>
    /// 获得摄像机，主要用于Cine虚拟摄像机，主摄像机还是通过MainCamera获得
    /// </summary>
    /// <param name="tag"></param>
    public static GameObject GetCamera(string tag) {
        if (!TagMap.ContainsKey(tag))
            return default;
        else
            return TagMap[tag];
    }

}