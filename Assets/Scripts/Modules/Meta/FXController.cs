using System;
using System.Threading.Tasks;
using AKIRA;
using AKIRA.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 特效管理器
/// </summary>
public class FXController : IController {
    public async Task Initialize() {
       await UniTask.Yield();
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="path"></param>
    /// <param name="position"></param>
    public void PlayFX(string path, Vector3 position)
        => PlayFX(path, position, Quaternion.identity);

    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="path"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public async void PlayFX(string path, Vector3 position, Quaternion rotation) {
        if (string.IsNullOrEmpty(path) || path.Equals(GameData.Asset.Null))
            return;
            
        var go = ObjectPool.Instance.Instantiate(path, position, rotation);
        var time = go.GetComponent<ParticleSystem>().main.duration;
        await UniTask.Delay(Convert.ToInt32(time * 1000));
        ObjectPool.Instance.Destory(go);
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    /// <param name="path"></param>
    /// <param name="position"></param>
    /// <param name="destoryCondition">返回true时进行回收</param>
    /// <returns></returns>
    public void PlayFX(string path, Vector3 position, Func<bool> destoryCondition)
        => PlayFX(path, position, Quaternion.identity, destoryCondition);

    /// <summary>
    /// 播放特效并且返回
    /// </summary>
    /// <param name="path"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public async void PlayFX(string path, Vector3 position, Quaternion rotation, Func<bool> destoryCondition) {
        if (string.IsNullOrEmpty(path) || path.Equals(GameData.Asset.Null))
            return;
        
        var go = ObjectPool.Instance.Instantiate(path, position, rotation);
        await UniTask.WaitUntil(() => destoryCondition?.Invoke() ?? true);
        ObjectPool.Instance.Destory(go);
    }
}