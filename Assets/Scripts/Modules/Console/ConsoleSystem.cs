using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AKIRA.Attribute;
using AKIRA.Behaviour.GM;
using AKIRA.UIFramework;

namespace AKIRA.Manager {
    /// <summary>
    /// GM管理器
    /// </summary>
    [SystemLauncher(-10)]
    public class ConsoleSystem : Singleton<ConsoleSystem>, IUpdate {
        // 配置文件
        private ConsoleConfig config;
        // 页面
        private IConsoleUI panel;
        // 输出
        public StringBuilder Output { get; private set; }
        // 指令
        public IGMCommand[] Commands { get; private set; }

        protected ConsoleSystem() {}

        public override async Task Initialize() {
            await Task.Yield();
            config = GameConfig.Instance.GetConfig<ConsoleConfig>();
            var window = config.Init();

            if (window != WinEnum.None) {
                var panel = UIManager.Instance.Get(window);
                if (panel != null && panel.GetType().GetInterface("IConsoleUI") != null) {
                    $"Console Init successful!".Log(GameData.Log.Success);
                    // 页面初始化
                    this.panel = panel as IConsoleUI;
                    Output = new StringBuilder($"--------Console--------\n");
                    RepaintPanel();
                    // 获得全部指令
                    var types = typeof(IGMCommand).Name.GetConfigTypeByInterface();
                    Commands = new IGMCommand[types.Length];
                    types.Foreach((i, type) => Commands[i] = type.CreateInstance<IGMCommand>());
                    // 注册发送事件
                    EventSystem.Instance.AddEventListener(GameData.Event.OnConsoleCommand, OnConsoleCommand);
                    // 更新
                    this.Regist(GameData.Group.Other);
                } else {
                    $"Console panel error: {window}(null or not find {typeof(IConsoleUI)}), failed to init console system".Log(GameData.Log.Error);
                }
            }
        }

        public void OnUpdate() {
            if (panel.Active && config.Shut())
                panel.Active = false;
            
            if (!panel.Active && config.Wake())
                panel.Active = true;
        }

        /// <summary>
        /// 重新绘制页面
        /// </summary>
        private void RepaintPanel() {
            // 检查长度
            var lines = Output.ToString().Split("\n").ToList();
            var maxCount = config.maxHistoryCount;
            if (maxCount > 0 && lines.Count >= config.maxHistoryCount) {
                var deleteCount = 0;
                for (int i = 0; i < lines.Count - config.maxHistoryCount; i++)
                    deleteCount += lines[i].Length + 1;
                Output = Output.Remove(0, deleteCount);
            }

            panel.Repaint(Output.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnConsoleCommand(object obj) {
            var input = obj.ToString();
            Output.Append($"> {input}\n");

            var command = GetGMCommand(input.Trim().ToLower(), out string[] @params);
            if (command == null) {
                Output.Append($"Unknown command: {input}\n");
            } else {
                var callbackOutput = command.Excute(@params);
                if (!string.IsNullOrEmpty(callbackOutput))
                    Output.Append($"{callbackOutput}\n");
            }
            RepaintPanel();
        }

        /// <summary>
        /// 获得匹配的指令
        /// </summary>
        /// <param name="input"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public IGMCommand GetGMCommand(in string input, out string[] @params) {
            @params = default;
            
            foreach (var command in Commands) {
                var commandName = command.Name.ToLower();
                if (!input.StartsWith(commandName))
                    continue;
                
                var paramStr = input.Replace(commandName, "").Trim();
                if (!string.IsNullOrEmpty(paramStr))
                    @params = paramStr.Split(" ");
                return command;
            }

            return default;
        }
    }
}