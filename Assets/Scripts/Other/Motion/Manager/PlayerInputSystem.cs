using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace AKIRA.Manager {
    /// <summary>
    /// 输入管理
    /// </summary>
    public class PlayerInputSystem : MonoSingleton<PlayerInputSystem>, IUpdate {
        private PlayerInputAction inputActions;
        /// <summary>
        /// 输入系统 C#类
        /// </summary>
        public PlayerInputAction InputActions => inputActions;

        /// <summary>
        /// 新版的输入系统
        /// </summary>
        private PlayerInput playerInput;
        /// <summary>
        /// 是否切换到UI
        /// </summary>
        public bool IsUI { get; private set; } = true;

        /// <summary>
        /// 输入系统切换UI
        /// </summary>
        private Action OnInputSwitchUI;
        /// <summary>
        /// 输入切换玩家
        /// </summary>
        private Action OnInputSwitchPlayer;

        private const string ActionMap_Player = "Player";
        private const string ActionMap_UI = "UI";
        private const string Rebind = "Rebinds";

        protected override void Awake() {
            base.Awake();
            inputActions = new PlayerInputAction();
            playerInput = this.GetComponent<PlayerInput>();
            
            if (playerInput == null)
                throw new System.NullReferenceException($"{this.name} dont have PlayInput MonoBehaviour, InputSystem Unavailable");
            
            IsUI = playerInput.defaultActionMap.Equals("UI");
                
            inputActions.Enable();
        }

        private void Start() {
            this.Regist();
        }

        public void GameUpdate() {
#if UNITY_EDITOR
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
#elif UNITY_ANDROID || UNITY_IOS
#elif UNITY_STANDALONE
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
#elif UNITY_PS4 || UNITY_PS5
#elif UNITY_XBOX
#elif UNITY_SWITCH
#endif
            {
                if (IsUI) {
                    SwtichPlayer();
                } else {
                    SwitchUI();
                }
            }

                // FIXME: 更换键盘绑定的一个案例
                // InputActions.Disable();
                // InputActions.Player.Run.PerformInteractiveRebinding()
                //     // .WithControlsExcluding("Jump")
                //     .OnComplete(callback => {
                //         callback.action.bindings[0].overridePath.Log();
                //         callback.Dispose();
                //         InputActions.Enable();
                //     })
                //     .Start();

            // FIXME: 电脑下鼠标隐藏/显示
#if UNITY_EDITOR || UNITY_STANDALONE
            if (IsUI) {
                Cursor.visible = true;
            } else {
                Cursor.visible = false;
                Cursor.visible = Keyboard.current.leftAltKey.isPressed;
            }
#endif
        }

        /// <summary>
        /// 切换到UI
        /// </summary>
        public void SwitchUI() {
            playerInput.SwitchCurrentActionMap(ActionMap_UI);
            inputActions.Player.Disable();
            inputActions.UI.Enable();
            OnInputSwitchUI?.Invoke();
            IsUI = true;
        }

        /// <summary>
        /// 切换到玩家
        /// </summary>
        public void SwtichPlayer() {
            playerInput.SwitchCurrentActionMap(ActionMap_Player);
            inputActions.Player.Enable();
            inputActions.UI.Disable();
            OnInputSwitchPlayer?.Invoke();
            IsUI = false;
        }

        /// <summary>
        /// FIXME: PlayerInput Camera 暂时不知道什么用
        /// </summary>
        /// <param name="camera"></param>
        public void PlayerInputCamera(Camera camera) {
            playerInput.camera = camera;
        }

        /// <summary>
        /// FIXME: PlayerInput InputSystemUIInputModule 暂时不知道什么用
        /// </summary>
        /// <param name="uIInputModule"></param>
        public void PlayerInputUIInputModule(InputSystemUIInputModule uIInputModule) {
            playerInput.uiInputModule = uIInputModule;
        }

        /// <summary>
        /// 输入系统切换UI事件
        /// </summary>
        public void RegistOnInputSwitchUI(Action OnInputSwitchUI) {
            this.OnInputSwitchUI += OnInputSwitchUI;
        }

        /// <summary>
        /// 输入系统切换玩家事件
        /// </summary>
        public void RegistOnInputSwitchPlayer(Action OnInputSwitchPlayer) {
            this.OnInputSwitchPlayer += OnInputSwitchPlayer;
        }

        /// <summary>
        /// 保存输入设置
        /// </summary>
        public void SaveUserRebind() {
            var rebinds = playerInput.actions.SaveBindingOverridesAsJson();
            Rebind.Save(rebinds);
        }

        /// <summary>
        /// 读取输入设置
        /// </summary>
        public void LoadUserRebind() {
            var rebinds = Rebind.GetString();
            if (String.IsNullOrEmpty(rebinds))
                return;
            playerInput.actions.LoadBindingOverridesFromJson(rebinds);
        }
    }
}