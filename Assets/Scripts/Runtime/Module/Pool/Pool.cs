#define UseAB
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Manager {
    #region Base
    public abstract class PoolBase {
        // 对象池根节点下父节点
        protected Transform poolParent;
        // 池子最大包含数量
        protected int maxCount = 0;

        /// <summary>
        /// 释放池子（销毁）
        /// </summary>
        public abstract void Free();
    }
    #endregion
    
    /// <summary>
    /// 池
    /// </summary>
    /// <typeparam name="T">可挂载脚本对象</typeparam>
    public class Pool<T> : PoolBase where T : Component, IPool {
        // 先进先出
        private Queue<T> pool;
        // 使用中
        private List<T> onUse;

        /// <summary>
        /// 池子初始化
        /// </summary>
        /// <param name="root">对象池根节点</param>
        /// <param name="name">名称</param>
        /// <param name="maxCount">池子最大数量</param>
        /// <returns></returns>
        public Pool<T> Init(Transform root, string name, int maxCount = 2000) {
            pool = new Queue<T>();
            onUse = new List<T>();
            poolParent = new GameObject(name).transform;
            poolParent.SetParent(root);
            this.maxCount = maxCount;
            return this;
        }

        /// <summary>
        /// 对象池取出对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T Instantiate(string path, object data = null) {
            // 池子超出限制
            if (!TryGetFree(data, out T com)) return null;
            // 空闲对象存在
            if (com != null) return com;
            // 池子中没有空闲对象，加载实例化
#if UseAB
            com = AssetSystem.Instance.LoadObject<T>(path);
#else
            com = path.Load<T>();
#endif
            if (com == null) {
                $"{path} 下 {typeof(T).Name} 错误！".Log(GameData.Log.Warn);
                return default;
            }
            com = com.Instantiate();
            com.name = poolParent.name;
            com.Wake(data);
            onUse.Add(com);
            return com;
        }

        /// <summary>
        /// 对象池取出对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T Instantiate(T target, object data = null) {
            // 池子超出限制
            if (!TryGetFree(data, out T com)) return null;
            // 空闲对象存在
            if (com != null) return com;
            // 池子中没有空闲对象，加载实例化
            com = target.Instantiate();
            com.name = poolParent.name;
            com.Wake(data);
            onUse.Add(com);
            return com;
        }

        /// <summary>
        /// 获得空闲对象
        /// </summary>
        /// <returns></returns>
        private bool TryGetFree(object data, out T com) {
            com = null;
            // 循环池子拿出空闲对象
            if (pool.Count != 0) {
                com = pool.Dequeue();
                com.gameObject.SetActive(true);
                com.Wake(data);
                onUse.Add(com);
                return true;
            }
            // 判断超过个数
            if (pool.Count + onUse.Count >= maxCount) {
                $"{this}超过最大个数".Log(GameData.Log.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="com"></param>
        public void Destory(T com, object data = null) {
            com.Recycle(data);
            com.SetParent(poolParent);
            com.gameObject.SetActive(false);
            pool.Enqueue(com);
            onUse.Remove(com);
        }

        public override void Free() {
            while (pool.Count != 0)
                pool.Dequeue().gameObject.Destory();
            for (int i = 0; i < onUse.Count; i++)
                onUse[i].gameObject.Destory();
            poolParent.gameObject.Destory();
            onUse.Clear();
            maxCount = 0;
        }
    }

    /// <summary>
    /// GameObject池
    /// </summary>
    public class Pool : PoolBase {
        private Queue<GameObject> pool;
        private List<GameObject> onUse;

        /// <summary>
        /// 池子初始化
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parentName">池子名称</param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public Pool Init(Transform root, string parentName, int maxCount = 2000) {
            pool = new Queue<GameObject>();
            onUse = new List<GameObject>();
            poolParent = new GameObject(parentName).transform;
            poolParent.SetParent(root);
            this.maxCount = maxCount;
            return this;
        }

        /// <summary>
        /// 对象池取出对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GameObject Instantiate(string path) {
            // 池子超出限制
            if (!TryGetFree(out GameObject go)) return null;
            // 空闲对象存在
            if (go != null) return go;
            // 池子中没有空闲对象，加载实例化
#if UseAB
            go = AssetSystem.Instance.LoadObject<GameObject>(path);
#else
            go = path.Load<GameObject>();
#endif
            if (go == null) {
                $"{path} 下 {poolParent.name} 错误！".Log(GameData.Log.Warn);
                return default;
            }
            go = go.Instantiate();
            go.name = poolParent.name;
            onUse.Add(go);
            return go;
        }

        /// <summary>
        /// 对象池取出对象
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public GameObject Instantiate(GameObject target) {
            // 池子超出限制
            if (!TryGetFree(out GameObject go)) return null;
            // 空闲对象存在
            if (go != null) return go;
            // 池子中没有空闲对象，加载实例化
            go = target.Instantiate();
            go.name = poolParent.name;
            onUse.Add(go);
            return go;
        }

        /// <summary>
        /// 获得空闲对象
        /// </summary>
        /// <returns></returns>
        private bool TryGetFree(out GameObject go) {
            go = null;
            // 循环池子拿出空闲对象
            if (pool.Count != 0) {
                go = pool.Dequeue();
                go.gameObject.SetActive(true);
                onUse.Add(go);
                return true;
            }
            // 判断超过个数
            if (pool.Count + onUse.Count >= maxCount) {
                $"{this}超过最大个数".Log(GameData.Log.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="go"></param>
        public void Destory(GameObject go) {
            go.transform.SetParent(poolParent);
            go.gameObject.SetActive(false);
            pool.Enqueue(go);
            onUse.Remove(go);
        }

        public override void Free() {
            while (pool.Count != 0)
                pool.Dequeue().Destory();
            for (int i = 0; i < onUse.Count; i++)
                onUse[i].Destory();
            poolParent.gameObject.Destory();
            onUse.Clear();
            maxCount = 0;
        }
    }

    /// <summary>
    /// 引用池
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public class RPool<K> : PoolBase where K : class, IPool, new() {
        private Queue<K> rpool;
        private List<K> onUse;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public RPool<K> Init(int maxCount = 2000) {
            rpool = new Queue<K>();
            onUse = new List<K>();
            this.maxCount = maxCount;
            return this;
        }

        /// <summary>
        /// 获得对象
        /// </summary>
        /// <returns></returns>
        public K Instantiate(object data = null) {
            if (!TryGetFree(data, out K @class)) return default;
            // 获得空闲对象
            if (@class != null) return @class;
            // new
            @class = new K();
            @class.Wake(data);
            onUse.Add(@class);
            return @class;
        }

        /// <summary>
        /// 尝试获得空闲对象
        /// </summary>
        private bool TryGetFree(object data, out K @class) {
            @class = default;
            if (rpool.Count != 0) {
                @class = rpool.Dequeue();
                @class.Wake(data);
                onUse.Add(@class);
                return true;
            }
            if (rpool.Count + onUse.Count >= maxCount) {
                $"对象 {typeof(K)} 已经new最大数量".Log(GameData.Log.Warn);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 对象回收
        /// </summary>
        public void Destroy(K @class, object data = null) {
            @class.Recycle(data);
            rpool.Enqueue(@class);
            onUse.Remove(@class);
        }

        public override void Free() {
            while (rpool.Count != 0) {
                var c = rpool.Dequeue();
                c = default;
            }
            for (int i = 0; i < onUse.Count; i++)
                onUse[i] = default;
            onUse.Clear();
            maxCount = 0;
        }
    }

}