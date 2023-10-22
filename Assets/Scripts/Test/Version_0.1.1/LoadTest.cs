using AKIRA.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadTest : MonoBehaviour {
    private void Awake() {
        WaitLoad();
    }

    private async void WaitLoad() {
        StartCoroutine(AssetSystem.Instance.LoadBundles());
    
        await UniTask.WaitUntil(() => AssetSystem.Instance.BundleLoadCompleted);

        var cubePrefab = AssetSystem.Instance.LoadObject<GameObject>("Assets/Res/MainBundle/Cube.prefab");
        for (int i = 0; i < 3; i++) 
            ObjectPool.Instance.Instantiate(cubePrefab, Vector3.right * i, Quaternion.identity);
    }
}