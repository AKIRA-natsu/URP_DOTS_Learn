using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AKIRA.Attribute;
using AKIRA.UIFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace AKIRA.Manager {
    /// <summary>
    /// 控制器唤醒/关闭配置
    /// </summary>
    [Serializable]
    public struct PlatformWakeConfig {
        public WinEnum window;

        public RuntimePlatform[] platforms;

        [InputControl]
        public string[] wakeInputs;

        [InputControl]
        public string[] shutInputs;
    }

    /// <summary>
    /// 指令行
    /// </summary>
    [Serializable]
    public struct Command {
        [SelectionPop(typeof(CommandVar))]
        public string command;
        public string description;
    }

    /// <summary>
    /// 指令行
    /// </summary>
    public class CommandVar {
        public const string help = "help";
        public const string clear = "clear";
        public const string shut = "shut";
    }

    [CreateAssetMenu(fileName = "ConsoleConfig", menuName = "AKIRA.Framework/Module/ConsoleConfig", order = 0)]
    public class ConsoleConfig : ScriptableObject {
        [Min(0)]
        public int maxHistoryCount = 50;

        [SerializeField]
        private PlatformWakeConfig[] wakeConfigs;
        // 当前模板配置
        private PlatformWakeConfig usedConfig;

        [SerializeField]
        private Command[] commands;
        // 指令行，对所有平台有效
        public IReadOnlyCollection<Command> Commands => commands;

        /// <summary>
        /// Init by console system
        /// </summary>
        public WinEnum Init() {
            var platform = Application.platform;
            foreach (var config in wakeConfigs) {
                if (config.platforms.Contains(platform)) {
                    usedConfig = config;

                    StringBuilder builder = new StringBuilder($"[{platform}] -> ");
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
            return WinEnum.None;
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