using System;
using AKIRA.Behaviour.GM;
using AKIRA.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AKIRA.UIFramework {
    [Win(WinType.Interlude, "Assets/Res/MainBundle/Prefabs/UI/Console.prefab", 100)]
    public class ConsolePanel : ConsolePanelProp, IConsoleUI {
        public override void Awake(object obj) {
            base.Awake(obj);
            ConsoleInput.onEndEdit.AddListener(OnEndEdit);
        }

        public override void Show(params object[] args) {
            base.Show();
            ConsoleInput.ActivateInputField();
        }

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
            EventSystem.Instance.TriggerEvent(Consts.Event.OnConsoleCommand, value.Trim().Replace("-", "").ToLower());
            ConsoleInput.text = "";
            ConsoleInput.ActivateInputField();
        }
    }
}