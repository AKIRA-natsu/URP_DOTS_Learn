using AKIRA.Manager;

namespace AKIRA.Behaviour.GM {
    // 帮助
    public struct Help : IGMCommand {
        public string Name => "Help";
        public string Description => "Show list of commands";

        public string Excute(params string[] values) {
            var commands = ConsoleSystem.Instance.Commands;

            string helpTxt = default;
            commands.Foreach((index, command) =>  helpTxt += $"    -> {command.Name} [<param>] : {command.Description}\n");

            return helpTxt.Remove(helpTxt.Length - 1);
        }
    }

    // 退出游戏
    public struct Shut : IGMCommand {
        public string Name => "Shut";
        public string Description => "Shut game";

        public string Excute(params string[] values) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
            return default;
        }
    }

    // 清空输出
    public struct ClearOutput : IGMCommand {
        public string Name => "Clear";
        public string Description => "Clear output";

        public string Excute(params string[] values) {
            ConsoleSystem.Instance.Output.Clear();
            return "Clear successfully";
        }
    }

    // 发送事件
    public struct EventSender : IGMCommand {
        public string Name => "Send Event";
        public string Description => "Send event named <param> with 0 or 1 string param";

        public string Excute(params string[] values) {
            if (values == null || values.Length == 0)
                return "Send evnet failed => dont know <EventName>.\nTry command \"Send Event <EventName> [<EventStringParam>]\" instead";
            
            EventSystem.Instance.TriggerEvent(values[0], values.Length == 1 ? null : values[2]);
            return $"Try Send Event {values[0]}";
        }
    }
}