using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace AKIRA.Manager {
    /// <summary>
    /// 控制器唤醒/关闭配置
    /// </summary>
    [Serializable]
    public struct PlatformWakeConfig {
        public string window;

        public RuntimePlatform[] platforms;

        [InputControl]
        public string[] wakeInputs;

        [InputControl]
        public string[] shutInputs;
    }

    [CreateAssetMenu(fileName = "ConsoleConfig", menuName = "AKIRA.Framework/Module/ConsoleConfig", order = 0)]
    public class ConsoleConfig : ScriptableObject {
        [Min(0)]
        public int maxHistoryCount = 50;

        [SerializeField]
        private PlatformWakeConfig[] wakeConfigs;
        // 当前模板配置
        private PlatformWakeConfig usedConfig;

        /// <summary>
        /// Init by console system
        /// </summary>
        public string Init() {
            var platform = Application.platform;
            foreach (var config in wakeConfigs) {
                if (config.platforms.Contains(platform)) {
                    usedConfig = config;

                    StringBuilder builder = new($"[{platform}] -> ");
                    builder.Append("Wake By Key:");
                    foreach (var awake in usedConfig.wakeInputs)
                        builder.Append($"{awake} ");
                    builder.Append(",Shut By Key:");
                    foreach (var shut in usedConfig.shutInputs)
                        builder.Append($"{shut} ");
                    
                    builder.ToString().Log(GameData.Log.Console);
                    return usedConfig.window;
                }
            }

            $"Console not availiable".Log(GameData.Log.Console);
            return default;
        }

        /// <summary>
        /// 是否唤醒
        /// </summary>
        /// <returns></returns>
        public bool Wake() => CheckInputs(usedConfig.wakeInputs);
        /// <summary>
        /// 是否关闭
        /// </summary>
        /// <returns></returns>
        public bool Shut() => CheckInputs(usedConfig.shutInputs);

        /// <summary>
        /// 检查是否输入了
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private bool CheckInputs(string[] inputs) {
            var inputCount = 0;
            foreach (var input in inputs) {
                var control = InputSystem.FindControl(input);
                // 判断一直按下
                if (control.IsPressed())
                    inputCount++;
            }
            return inputCount == inputs.Length;
        }
    }
}