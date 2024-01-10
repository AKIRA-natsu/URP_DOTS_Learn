using System.Collections.Generic;
using System.Threading.Tasks;

namespace AKIRA.Manager {
    /// <summary>
    /// 红点接口
    /// </summary>
    public interface IReddot {
        /// <summary>
        /// 注册标签
        /// </summary>
        /// <value></value>
        public string Tag { get; }

        /// <summary>
        /// 显示/隐藏
        /// </summary>
        /// <param name="active"></param>
        void Active(bool active);
    }

    /// <summary>
    /// 红点管理
    /// </summary>
    public class ReddotController : IController {
        private Dictionary<string, IReddot> reddotMap = new();
        private Dictionary<string, BindableValue<bool>> listenerMap = new();

        public async Task Initialize()
        {
            await Task.Yield();
        }

        /// <summary>
        /// 注册红点
        /// </summary>
        /// <param name="reddot"></param>
        public void RegistReddot(IReddot reddot) {
            var tag = reddot.Tag;
            if (string.IsNullOrEmpty(tag) || reddotMap.ContainsKey(tag))
                return;
            reddotMap.Add(tag, reddot);
            
            // 如果事件中包含键值，进行监听
            if (listenerMap.ContainsKey(tag))
                listenerMap[tag].RegistBindAction(reddot.Active);
        }

        /// <summary>
        /// 移除红点，如果包含事件，一并移除
        /// </summary>
        /// <param name="reddot"></param>
        public void RemoveReddot(IReddot reddot) {
            var tag = reddot.Tag;
            if (string.IsNullOrEmpty(tag))
                return;
            if (reddotMap.ContainsKey(tag))
                reddotMap.Remove(tag);
            if (listenerMap.ContainsKey(tag))
                listenerMap.Remove(tag);
        }

        /// <summary>
        /// 注册监听对象
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        public void RegistBindableValue(string tag, BindableValue<bool> value) {
            if (string.IsNullOrEmpty(tag) || listenerMap.ContainsKey(tag))
                return;
            
            listenerMap.Add(tag, value);
            if (reddotMap.ContainsKey(tag))
                value.RegistBindAction(reddotMap[tag].Active);
        }

        /// <summary>
        /// 移除监听对象
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveBindableValue(string tag) {
            if (string.IsNullOrEmpty(tag) || !listenerMap.ContainsKey(tag))
                return;
            
            var value = listenerMap[tag];
            listenerMap.Remove(tag);
            
            if (reddotMap.ContainsKey(tag))
                value.RemoveBindAction(reddotMap[tag].Active);
        }
    }
}