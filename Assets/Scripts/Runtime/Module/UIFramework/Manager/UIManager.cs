using System;
using System.Collections.Generic;
using UnityEngine;
using AKIRA.Manager;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI 管理
    /// </summary>
    public class UIManager : Singleton<UIManager> {
        // 字典表
        private Dictionary<string, UIComponent> UIMap = new Dictionary<string, UIComponent>();

        private UIManager() {
            // 默认 [UI] 为UI根节点
            var root = AssetSystem.Instance.LoadObject<GameObject>(GameData.Asset.UIManager);
            if (root == null)
                throw new ArgumentNullException($"{GameData.Asset.UIManager} 不存在");
            UI.Initialize(root.Instantiate());
        }

        /// <summary>
        /// <para>启动只运行一次</para>
        /// <para>Map 添加 UICom</para>
        /// <para>UI Awake</para>
        /// </summary>
        public async override Task Initialize() {
            var wins = ReflectionHelp.Handle<WinAttribute>(GameData.DLL.Default);
            // 按照winenum值排序
            Array.Sort(wins, (a, b) => a.GetCustomAttribute<WinAttribute>().Data.@enum - b.GetCustomAttribute<WinAttribute>().Data.@enum);
            foreach (var win in wins) {
                await Task.Yield();
                // attribute运行了两次！
                var com = win.CreateInstance<UIComponent>();
                // var com = (UIComponent)AttributeHelp<WinAttribute>.Type2Obj(win);
                var info = win.GetCustomAttributes(false)[0] as WinAttribute;
                // 注册在 UIDataManager
                if (info.Data.@enum != WinEnum.None)
                    UIDataManager.Instance.Register(com, info.Data);
                // Awake UI
                com.Awake(info.Data.@type);
                // 注册在 UIManager
                AddUI(com);
            }
        }

        /// <summary>
        /// 添加 UI
        /// </summary>
        /// <param name="com"></param>
        private void AddUI(UIComponent com) {
            var name = $"{com.gameObject.name}Panel";
            if (UIMap.ContainsKey(name)) {
                $"UI has contained {name}".Log(GameData.Log.Error);
                return;
            }
            UIMap[name] = com;
        }

        /// <summary>
        /// 获得UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : UIComponent {
            // var name = typeof(T).Name;
            // if (!UIMap.ContainsKey(name)) {
            //     $"UI dont contains {name}".Colorful(Color.red).Log();
            //     return null;
            // }
            // return UIMap[name] as T;
            return Get(typeof(T)) as T;
        }

        /// <summary>
        /// 获得UIComponent
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public UIComponent Get(Type type) {
            if (type == null)
                return default;
            var name = type.Name;
            if (!UIMap.ContainsKey(name)) {
                $"UI dont contains {name}".Log(GameData.Log.Error);
                return null;
            }
            return UIMap[name];
        }

        /// <summary>
        /// 通过WinType类型获得页面
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public UIComponent Get(WinEnum type) {
            return UIMap.SingleOrDefault(kvp => UIDataManager.Instance.GetUIData(kvp.Value).@enum.Equals(type)).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        public void Invoke<T>(string method, params object[] args) where T : UIComponent {
            Get<T>()?.Invoke(method, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        public void Invoke(Type type, string method, params object[] args) {
            Get(type)?.Invoke(method, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        public void Invoke(WinEnum type, string method, params object[] args) {
            Get(type)?.Invoke(method, args);
        }

        /// <summary>
        /// <para>可适配列表收集</para>
        /// <para>正常只会在开始运行一次，所以list放里面new了一个</para>
        /// </summary>
        /// <returns></returns>
        public List<RectTransform> MatchableColleation() {
            List<RectTransform> list = new List<RectTransform>();
            foreach (var ui in UIMap.Values) {
                foreach (var rect in ui.MatchableList)
                    list.Add(rect);
            }
            return list;
        }

        /// <summary>
        /// 销毁 UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Destory<T>() where T : UIComponent {
            var name = typeof(T).Name;
            if (UIMap.ContainsKey(name)) {
                UIMap[name].Destory();
                UIDataManager.Instance.Remove(UIMap[name]);
                UIMap.Remove(name);
            }
        }
    }
}