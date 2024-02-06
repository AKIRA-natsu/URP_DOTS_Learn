using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// <para>对象池</para>
    /// </summary>
    public partial class ObjectPool : Singleton<ObjectPool> {
        // 泛型池
        private Dictionary<string, IPoolBase> poolMap = new();
        // 父节点
        private Transform root;
        
        private ObjectPool() {
            // 构造函数，创建父节点
            root = new GameObject("[ObjectPool]").DontDestory().transform;
        }

        #region path 获得对象
        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path) where T : Object
            // 父物体设空，等于在对象池父物体下移动，相当于世界坐标
            => Instantiate<T>(path, Vector3.zero, Quaternion.identity, null);

        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, Transform parent) where T : Object
            // 父物体下的原点
            => Instantiate<T>(path, Vector3.zero, Quaternion.identity, parent);

        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, Vector3 position) where T : Object
            // 父物体设空，等于在对象池父物体下移动，相当于世界坐标
            => Instantiate<T>(path, position, Quaternion.identity, null);

        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position">world position</param>
        /// <param name="rotation">world rotation</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, Vector3 position, Quaternion rotation) where T : Object
            // 父物体设空，等于在对象池父物体下移动，相当于世界坐标
            => Instantiate<T>(path, position, rotation, null);

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position">local position</param>
        /// <param name="rotation">local rotation</param>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, Vector3 position, Quaternion rotation, Transform parent) where T : Object {
            var name = path.Split("/")[^1].Split('.')[0];
            if (!poolMap.ContainsKey(name))
                poolMap[name] = new OPool<T>(root, name);
            return (poolMap[name] as OPool<T>).Instantiate(path, position, rotation, parent);
        }
        #endregion

        #region T 获得对象
        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="origin"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T origin) where T : Object
            // 父物体设空，等于在对象池父物体下移动，相当于世界坐标
            => Instantiate<T>(origin, Vector3.zero, Quaternion.identity, null);

        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T origin, Transform parent) where T : Object
            // 父物体下的原点
            => Instantiate<T>(origin, Vector3.zero, Quaternion.identity, parent);

        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="position"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T origin, Vector3 position) where T : Object
            // 父物体设空，等于在对象池父物体下移动，相当于世界坐标
            => Instantiate<T>(origin, position, Quaternion.identity, null);

        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="position">world position</param>
        /// <param name="rotation">world rotation</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T origin, Vector3 position, Quaternion rotation) where T : Object
            // 父物体设空，等于在对象池父物体下移动，相当于世界坐标
            => Instantiate<T>(origin, position, rotation, null);

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="com"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T origin, Vector3 position, Quaternion rotation, Transform parent) where T : Object {
            var name = origin.name;
            if (!poolMap.ContainsKey(name))
                poolMap[name] = new OPool<T>(root, name);
            return (poolMap[name] as OPool<T>).Instantiate(origin, position, rotation, parent);
        }
        #endregion

        #region 销毁 和 预加载
        /// <summary>
        /// 对象池销毁池对象
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void Destory<T>(T value) where T : Object {
            var name = value.name;
            // FIXME: 继承类是空
            if (!poolMap.ContainsKey(name)) 
                poolMap[name] = new OPool<T>(root, name);
            (poolMap[name] as OPool<T>).Destroy(value);
        }

        /// <summary>
        /// 对象池释放池
        /// </summary>
        /// <param name="com"></param>
        /// <typeparam name="T"></typeparam>
        public void Free<T>(T com) where T : Object {
            // var name = typeof(T).Name;
            var name = com.name;
            if (!poolMap.ContainsKey(name)) {
                $"{name} 池子不存在！".Log(GameData.Log.Error);
                return;
            }
            var pool = poolMap[name] as OPool<T>;
            pool.Free();
            poolMap.Remove(name);
        }

        /// <summary>
        /// 预加载
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public void PreLoad<T>(string path, int count) where T : Object {
            var temp = new List<T>();
            for (int i = 0; i < count; i++) {
                var com = Instantiate<T>(path);
                if (null == com)
                    break;
                temp.Add(com);
            }
            for (int i = 0; i < temp.Count; i++)
                Destory(temp[i]);
        }

        /// <summary>
        /// 预加载
        /// </summary>
        /// <param name="com"></param>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        public void PreLoad<T>(T com, int count) where T : Object {
            var temp = new List<T>();
            for (int i = 0; i < count; i++) {
                var poolCom = Instantiate(com);
                if (null == poolCom)
                    break;
                temp.Add(poolCom);
            }
            for (int i = 0; i < temp.Count; i++)
                Destory(temp[i]);
        }
        #endregion

    }
}