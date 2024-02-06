#define USE_ASSETBUNDLE     // 是否使用AB包，不使用的话读取Resource
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Manager {
    #region Base
    internal interface IPoolBase { }

    internal abstract class PoolBase<T> : IPoolBase {
        // 池子最大包含数量
        protected int maxCount = 0;

        protected Queue<T> pool;
        protected List<T> onUse;

        public PoolBase(int maxCount = 128) {
            this.maxCount = maxCount;
            pool = new();
            onUse = new();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="value"></param>
        public abstract void Destroy(T value, object data = null);
        /// <summary>
        /// 释放池子（销毁）
        /// </summary>
        public abstract void Free();

        /// <summary>
        /// 尝试获得空闲对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool TryGetFree(out T value) {
            value = default;
            // 循环池子拿出空闲对象
            if (pool.Count != 0) {
                value = pool.Dequeue();
                onUse.Add(value);
                return true;
            }
            // 判断超过个数
            if (pool.Count + onUse.Count >= maxCount) {
                maxCount *= 2;
                $"{this}超过最大个数，扩容到 {maxCount}".Log(GameData.Log.Warn);
            }
            return false;
        }
    }
    #endregion
    
    #region Object Pool [OPool]
    /// <summary>
    /// 池
    /// </summary>
    /// <typeparam name="T">可挂载脚本对象</typeparam>
    internal class OPool<T> : PoolBase<T> where T : Object {
        private Transform parent;

        public OPool(Transform root, string parentName) : base() {
            parent = new GameObject(parentName).transform;
            parent.SetParent(root);
        }

        /// <summary>
        /// 对象池取出对象，本地坐标
        /// </summary>
        /// <returns></returns>
        public T Instantiate(string path, Vector3 position, Quaternion rotation, Transform parent) {
            parent = parent != null ? parent : this.parent;
            if (!TryGetFree(out T value)) {
                // 池子中没有空闲对象，加载实例化
#if USE_ASSETBUNDLE
                value = AssetSystem.Instance.LoadObject<T>(path);
#else
                value = path.Load<T>();
#endif
                // 如果空，说明路径错误
                if (value == null) {
                    $"{path} 下 {typeof(T).Name} 错误！".Log(GameData.Log.Warn);
                    return default;
                }
                
                value = CreateNewInstance(value);
            }
            SetTransformData(value, position, rotation, parent);
            return value;
        }

        /// <summary>
        /// 对象池取出对象，本地坐标
        /// </summary>
        /// <returns></returns>
        public T Instantiate(T origin, Vector3 position, Quaternion rotation, Transform parent) {
            parent = parent != null ? parent : this.parent;
            if (!TryGetFree(out T value))
                value = CreateNewInstance(origin);
            SetTransformData(value, position, rotation, parent);
            return value;
        }

        /// <summary>
        /// 创建一个新的实例
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        private T CreateNewInstance(T origin) {
            var value = Object.Instantiate(origin);
            value.name = this.parent.name;
            onUse.Add(value);
            return value;
        }

        /// <summary>
        /// 设置 <see cref="value" /> 的 Transform 数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        protected void SetTransformData(T value, Vector3 position, Quaternion rotation, Transform parent) {
            Transform trans;
            if (value is Component com)
                trans = com.transform;
            else if (value is GameObject go)
                trans = go.transform;
            else
                return;
            trans.SetParent(parent);
            trans.SetLocalPositionAndRotation(position, rotation);
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="com"></param>
        public override void Destroy(T value, object data = null) {
            if (value is Component com) {
                com.gameObject.SetActive(false);
                com.SetParent(parent);
            } else if (value is GameObject go) {
                go.SetActive(false);
                go.transform.SetParent(parent);
            }
            onUse.Remove(value);
            pool.Enqueue(value);
        }

        public override void Free() {
            // 把使用中回收
            for (int i = 0; i < onUse.Count; i++)
                pool.Enqueue(onUse[i]);
            onUse.Clear();
            // 把一整个删掉
            parent.gameObject.Destory();
            maxCount = 0;
        }
    }
    #endregion

}