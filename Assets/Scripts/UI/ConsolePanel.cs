using System;
using AKIRA.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AKIRA.UIFramework {
    [Win(WinEnum.Console, "Assets/Res/MainBundle/Prefabs/UI/Console.prefab", WinType.Interlude)]
    public class ConsolePanel : ConsolePanelProp, IConsoleUI {
        public override void Awake(object obj) {
            base.Awake(obj);
            ConsoleInput.onEndEdit.AddListener(OnEndEdit);
        }

        public override void Show(params object[] args) {
            base.Show();
            ConsoleInput.ActivateInputField();
        }

        public override void Invoke(string name, params object[] args) {
            switch (name) {
                case "Init":
                    Init((Command[])args[0]);
                break;
                case "Repaint":
                    Repaint(args);
                break;
            }
        }

        public void Init(Command[] commands) { }

        public async void Repaint(params object[] values) {
            ContentFitter.enabled = false;
            ConsoleTxt.text = values[0].ToString();
            await UniTask.Delay(0);
            if (Application.isPlaying)
                ContentFitter.enabled = true;
            await UniTask.Delay(0);
            if (Application.isPlaying)
                ScrollView.verticalNormalizedPosition = 0f;
        }

        private void OnEndEdit(string value) {
            EventSystem.Instance.TriggerEvent(GameData.Event.OnConsoleCommand, value.Trim().Replace("-", "").ToLower());
            ConsoleInput.text = "";
            ConsoleInput.ActivateInputField();
        }
    }
}