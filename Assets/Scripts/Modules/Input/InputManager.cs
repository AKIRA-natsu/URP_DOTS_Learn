using System;
using System.Threading.Tasks;
using AKIRA.Attribute;
using AKIRA.UIFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace AKIRA.Manager {
    /// <summary>
    /// 输入管理，命名防止和Unity的冲突
    /// </summary>
    [SystemLauncher(-1)]
    public class InputManager : MonoSingleton<InputManager>, IUpdate {
        // 输入系统
        [field: SerializeField]
        public PlayerInput Input { get; private set; }

        // 是否是UI
        [field: SerializeField]
        public bool IsUI { get; private set; }

        // 是否锁住暂停
        public bool isLockPause;

        // 因为输入存档和 PlayerInput 挂钩，所以存档直接放 Manager 里面了
        private const string RebindKey = "Rebind";

        public override async Task Initialize() {
            await UniTask.Yield();
            Input = this.GetOrAddComponent<PlayerInput>();
            Input.actions = AssetSystem.Instance.LoadObject<InputActionAsset>(Assets.Asset_InputAction);
            Input.uiInputModule = UI.ManagerGo.GetComponentInChildren<InputSystemUIInputModule>();
            Input.actions.Enable();

            // 读取存档的绑定
            LoadUserRebind();

            // 设置默认的输入类型
            IsUI = false;
            Input.SwitchCurrentActionMap(InputActions.Player);

            // 注册事件
            EventSystem.Instance.AddEventListener(Events.OnGamePaused, OnGamePaused);
            EventSystem.Instance.AddEventListener(Events.OnGameSaving_Setting, SaveUserRebind);

            // 注册更新
            this.Regist(GameData.Group.Other);
        }

        private void OnGamePaused(object obj) {
            var isPaused = obj.ToBoolean();
            // 切换输入模式
            IsUI = isPaused;
            Input.SwitchCurrentActionMap(IsUI ? InputActions.UI : InputActions.Player);
        }

        /// <summary>
        /// 获得输入监听事件
        /// </summary>
        public InputAction GetInputAction(string name) {
            return Input.actions[name];
        }

        /// <summary>
        /// 保存输入设置
        /// </summary>
        private void SaveUserRebind(object obj) {
            var rebinds = Input.actions.SaveBindingOverridesAsJson();
            RebindKey.Save(rebinds);
        }

        /// <summary>
        /// 读取输入设置
        /// </summary>
        private void LoadUserRebind() {
            var rebinds = RebindKey.GetString();
            if (string.IsNullOrEmpty(rebinds))
                return;
            Input.actions.LoadBindingOverridesFromJson(rebinds);
        }

        public void OnUpdate() {
    // #if UNITY_EDITOR || UNITY_STANDALONE
    //         // 电脑下鼠标隐藏/显示
    //         Cursor.visible = IsUI;
    // #endif

            if (!isLockPause && GetInputAction(InputActions.Pause).WasPressedThisFrame())
                EventSystem.Instance.TriggerEvent(Events.OnGamePaused, !IsUI);
        }
    }
}