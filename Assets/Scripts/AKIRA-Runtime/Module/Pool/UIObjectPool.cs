using System.Reflection;
using AKIRA.UIFramework;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// <para>对象池 UI</para>
    /// </summary>
    public partial class ObjectPool {
        #region 扩展UI
        public T Instantiate<T>(Transform parent) where T : UIComponentProp, new() {
            var attribute = typeof(T).GetCustomAttribute<WinAttribute>();
            var path = attribute.Data.path;
            var prop = new T();
            prop.Awake(Instantiate<GameObject>(path, parent).transform);
            return prop;
        }

        /// <summary>
        /// 对象池销毁池对象
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void DestoryUI<T>(T value) where T : UIComponentProp {
            Destory(value.gameObject);
            // value.Dispose();
        }
        #endregion
    }
}