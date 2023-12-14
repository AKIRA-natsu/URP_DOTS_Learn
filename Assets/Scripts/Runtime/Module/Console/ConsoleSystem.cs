using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AKIRA.Attribute;
using AKIRA.UIFramework;

namespace AKIRA.Manager {
    /// <summary>
    /// 控制器管理器
    /// </summary>
    [SystemLauncher(-10)]
    public class ConsoleSystem : Singleton<ConsoleSystem>, IUpdate {
        // 配置文件
        private ConsoleConfig config;
        // 页面
        private UIComponent panel;
        // 输出
        private StringBuilder output;

        protected ConsoleSystem() {}

        public override async Task Initialize() {
            await Task.Yield();
            config = GameConfig.Instance.GetConfig<ConsoleConfig>();
            var window = config.Init();

            if (window != WinEnum.None) {
                panel = UIManager.Instance.Get(window);
                if (panel != null && panel.GetType().GetInterface("IConsoleUI") != null) {
                    $"Console Init successful!".Log(GameData.Log.Success);
                    AddCommonComands();
                    this.Regist(GameData.Group.Other);
                } else {
                    $"Console panel error: {window}(null or not find {typeof(IConsoleUI)}), failed to init console system".Log(GameData.Log.Error);
                }
            }
        }

        public void GameUpdate() {
            if (panel.Active && config.Shut())
                panel.Hide();
            
            if (!panel.Active && config.Wake())
                panel.Show();
        }

        /// <summary>
        /// 重新绘制页面
        /// </summary>
        private void RepaintPanel() {
            panel.Invoke("Repaint", output.ToString());
        }

        /// <summary>
        /// 添加普遍的事件
        /// </summary>
        private void AddCommonComands() {
            panel.Invoke("Init", config.Commands);

            output = new StringBuilder($"--------Console--------");
            RepaintPanel();

            EventSystem.Instance.AddEventListener(GameData.Event.OnConsoleCommand, OnConsoleCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnConsoleCommand(object obj) {
            var input = obj.ToString();
            output.Append($"\n> {input}");
            switch (obj) {
                case CommandVar.help:
                    var commands = config.Commands;
                    for (int i = 0; i < commands.Count; i++) {
                        var command = commands.ElementAt(i);
                        output.Append($"\n--{command.command}      {command.description}");
                    }
                break;
                case CommandVar.clear:
                    output.Clear();
                    output.Append($"--------Console--------");
                break;
                case CommandVar.shut:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    UnityEngine.Application.Quit();
#endif
                return;
                default:
                    output.Append($"\nUnknown command: {input}");
                break;
            }
            RepaintPanel();
        }
    }
}