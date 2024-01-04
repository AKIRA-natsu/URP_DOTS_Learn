using AKIRA;
using AKIRA.Attribute;
using AKIRA.Manager;
using UnityEngine;

/// <summary>
/// 临时加载
/// </summary>
public class TmpLoader : MonoBehaviour {
    [SerializeField]
    [SelectionPop(typeof(GameData.Asset))]
    private string[] loaders;

    private void Awake() {
        EventSystem.Instance.AddEventListener(GameData.Event.OnInitSystemCompleted, Load);
    }

    private void Load(object obj) {
        EventSystem.Instance.RemoveEventListener(GameData.Event.OnInitSystemCompleted, Load);
        foreach (var loader in loaders) {
            var prefab = AssetSystem.Instance.LoadObject<GameObject>(loader);
            var go = prefab.Instantiate();
            go.name = prefab.name;
            go.transform.SetParent(this);
        }
    }
}