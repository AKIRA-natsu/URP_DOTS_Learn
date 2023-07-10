using System;
using System.Reflection;

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
    }
}