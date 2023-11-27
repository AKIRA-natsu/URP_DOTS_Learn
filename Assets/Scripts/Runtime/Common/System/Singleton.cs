using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AKIRA.Manager {
    /// <summary>
    /// C# 单例
    /// 继承需要一个非公有的无参构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISystem where T : class {
        private static T _instance = null;

        // 多线程安全机制
        private static readonly object locker = new object();

        public static T Instance {
            get {
                // 线程锁
                lock (locker) {
                    if (null == _instance) {
                        // 反射获取实例
                        var octors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic) ;
    
                        // 获取无参数的非公共构造函数
                        var octor = Array.Find(octors, c => c.GetParameters().Length == 0);
    
                        // 没有则提示没有私有的构造函数
                        if (null == octor)
                        {
                            throw new Exception(typeof(T) + "No NonPublic constructor without 0 parameter");
                        }
    
                        // 实例化
                        _instance = octor.Invoke(null) as T;
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// 构造函数，避免外界new
        /// </summary>
        protected Singleton() {}

        public async virtual Task Initialize() {
            await Task.Yield();
        }

        protected List<IController> controllers = new();

        /// <summary>
        /// <para>生成Controller实例</para>
        /// <para>暂不支持MonoBehaviour版Controller</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected void CreateController<U>() where U : IController {
            IController controller = default;
            if (typeof(U).IsSubclassOf(typeof(UnityEngine.Component))) {
                $"在 {this} 中尝试创建 Controller {typeof(U)}".Error();
                return;
            } else {
                controller = typeof(U).CreateInstance<U>();
            }
            controller.Initialize();
            controllers.Add(controller);
        }

        /// <summary>
        /// 获得Controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public U GetController<U>() where U : IController {
            return (U)controllers.SingleOrDefault(controller => controller is U);
        }
    }
}