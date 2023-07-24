using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AKIRA.Behaviour.Camera;

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
    /// 表现字典
    /// </summary>
    /// <typeparam name="Type"></typeparam>
    /// <typeparam name="CameraBehaviour"></typeparam>
    /// <returns></returns>
    private static Dictionary<Type, CameraBehaviour> CameraMap = new Dictionary<Type, CameraBehaviour>();

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
    /// 注册摄像机表现脚本
    /// </summary>
    /// <param name="behaviour"></param>
    /// <typeparam name="T"></typeparam>
    internal static void RegistCameraBehaviour<T>(T behaviour) where T : CameraBehaviour {
        var type = behaviour.GetType();
        if (CameraMap.ContainsKey(type))
            return;
        CameraMap.Add(type, behaviour);
    }

    /// <summary>
    /// 移除摄像机表现脚本
    /// </summary>
    /// <param name="behaviour"></param>
    /// <typeparam name="T"></typeparam>
    internal static void RemoveCameraBehaviour<T>(T behaviour) where T : CameraBehaviour {
        var type = behaviour.GetType();
        if (!CameraMap.ContainsKey(type))
            return;
        CameraMap.Remove(type);
    }

    /// <summary>
    /// 获得摄像机脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static T GetCameraBehaviour<T>() where T : CameraBehaviour {
        var type = typeof(T);
        return CameraMap.ContainsKey(type) ? CameraMap[type] as T : default;
    }
    
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