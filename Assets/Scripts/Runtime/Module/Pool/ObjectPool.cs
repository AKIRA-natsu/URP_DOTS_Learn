using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace AKIRA.Manager {
    /// <summary>
    /// <para>对象池</para>
    /// </summary>
    public class ObjectPool : Singleton<ObjectPool> {
        // 泛型池
        private Dictionary<string, PoolBase> ObjectPoolMap = new Dictionary<string, PoolBase>();
        // 纯GameObject
        private Dictionary<string, Pool> GameObjectMap = new Dictionary<string, Pool>();
        // 父节点
        private Transform root;
        
        private ObjectPool() {
            // 构造函数，创建父节点
            root = new GameObject("[ObjectPool]").DontDestory().transform;
        }

        #region 泛型
        #region path 获得对象
        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="space">默认世界坐标</param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, Vector3 position, Quaternion rotation, Transform parent, Space space = Space.World, object data = null) where T : Component, IPool {
            T com = Instantiate<T>(path, parent, data);
            if (space == Space.World) {
                com.transform.position = position;
                com.transform.rotation = rotation;
            } else {
                com.transform.localPosition = position;
                com.transform.localRotation = rotation;
            }
            return com;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, Vector3 position, Quaternion rotation, object data = null) where T : Component, IPool {
            T com = Instantiate<T>(path, data);
            com.transform.position = position;
            com.transform.rotation = rotation;
            return com;
        }

        /// <summary>
        /// 对象池获得对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, Transform parent, object data = null) where T : Component, IPool {
            T com = Instantiate<T>(path, data);
            com.SetParent(parent);
            return com;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(string path, object data = null) where T : Component, IPool {
            var names = path.Split('/');
            var name = names[names.Length - 1];
            if (ObjectPoolMap.ContainsKey(name)) {
                var pool = ObjectPoolMap[name] as Pool<T>;
                return pool.Instantiate(path, data);
            } else {
                var pool = new Pool<T>().Init(root, name);
                ObjectPoolMap.Add(name, pool);
                return pool.Instantiate(path, data);
            }
        }
        #endregion

        #region T 获得对象
        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="space">默认世界坐标</param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T target, Vector3 position, Quaternion rotation, Transform parent, Space space = Space.World, object data = null) where T : Component, IPool {
            T com = Instantiate(target, parent, data);
            if (space == Space.World) {
                com.transform.position = position;
                com.transform.rotation = rotation;
            } else {
                com.transform.localPosition = position;
                com.transform.localRotation = rotation;
            }
            return com;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T target, Vector3 position, Quaternion rotation, object data = null) where T : Component, IPool {
            T com = Instantiate(target, data);
            com.transform.position = position;
            com.transform.rotation = rotation;
            return com;
        }

        /// <summary>
        /// 从对象池中获得对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T target, Transform parent, object data = null) where T : Component, IPool {
            T com = Instantiate(target, data);
            com.SetParent(parent);
            return com;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="com"></param>
        /// <param name="data">Pool 唤醒参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>(T com, object data = null) where T : Component, IPool {
            var name = com.name;
            if (ObjectPoolMap.ContainsKey(name)) {
                var pool = ObjectPoolMap[name] as Pool<T>;
                return pool.Instantiate(com, data);
            } else {
                var pool = new Pool<T>().Init(root, name);
                ObjectPoolMap.Add(name, pool);
                return pool.Instantiate(com, data);
            }
        }
        #endregion

        /// <summary>
        /// 对象池销毁池对象
        /// </summary>
        /// <param name="com"></param>
        /// <param name="data">Pool 回收参数</param>
        /// <typeparam name="T"></typeparam>
        public void Destory<T>(T com, object data = null) where T : Component, IPool {
            var name = com.name;
            if (ObjectPoolMap.ContainsKey(name)) {
                // FIXME: 继承类是空
                var pool = ObjectPoolMap[name] as Pool<T>;
                pool.Destory(com);
            } else {
                var pool = new Pool<T>().Init(root, name);
                ObjectPoolMap.Add(name, pool);
                pool.Destory(com);
            }
        }

        /// <summary>
        /// 对象池释放池
        /// </summary>
        /// <param name="com"></param>
        /// <typeparam name="T"></typeparam>
        public void Free<T>(T com) where T : Component, IPool {
            // var name = typeof(T).Name;
            var name = com.name;
            if (!ObjectPoolMap.ContainsKey(name)) {
                $"{name} 池子不存在！".Log(GameData.Log.Error);
                return;
            }
            var pool = ObjectPoolMap[name] as Pool<T>;
            pool.Free();
            ObjectPoolMap.Remove(name);
        }

        /// <summary>
        /// 预加载
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public void PreLoad<T>(string path, int count) where T : Component, IPool {
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
        public void PreLoad<T>(T com, int count) where T : Component, IPool {
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
    
        #region 纯GameObject
        #region path 获得对象
        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="space">默认世界坐标</param>
        /// <returns></returns>
        public GameObject Instantiate(string path, Vector3 position, Quaternion rotation, Transform parent, Space space = Space.World) {
            GameObject go = Instantiate(path, parent);
            if (space == Space.World) {
                go.transform.position = position;
                go.transform.rotation = rotation;
            } else {
                go.transform.localPosition = position;
                go.transform.localRotation = rotation;
            }
            return go;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject Instantiate(string path, Vector3 position, Quaternion rotation) {
            GameObject go = Instantiate(path);
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject Instantiate(string path, Transform parent) {
            GameObject go = Instantiate(path);
            go.transform.SetParent(parent);
            return go;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GameObject Instantiate(string path) {
            var names = path.Split('/');
            var name = names[names.Length - 1];
            if (GameObjectMap.ContainsKey(name)) {
                return GameObjectMap[name].Instantiate(path);
            } else {
                var pool = new Pool().Init(root, name);
                GameObjectMap.Add(name, pool);
                return pool.Instantiate(path);
            }
        }
        #endregion

        #region GameObject 获得对象
        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="space">默认世界坐标</param>
        /// <returns></returns>
        public GameObject Instantiate(GameObject target, Vector3 position, Quaternion rotation, Transform parent, Space space = Space.World) {
            GameObject go = Instantiate(target, parent);
            if (space == Space.World) {
                go.transform.position = position;
                go.transform.rotation = rotation;
            } else {
                go.transform.localPosition = position;
                go.transform.localRotation = rotation;
            }
            return go;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject Instantiate(GameObject target, Vector3 position, Quaternion rotation) {
            GameObject go = Instantiate(target);
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject Instantiate(GameObject target, Transform parent) {
            GameObject go = Instantiate(target);
            go.transform.SetParent(parent);
            return go;
        }

        /// <summary>
        /// 对象池获得池对象
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public GameObject Instantiate(GameObject target) {
            var name = target.name;
            if (GameObjectMap.ContainsKey(name)) {
                return GameObjectMap[name].Instantiate(target);
            } else {
                var pool = new Pool().Init(root, name);
                GameObjectMap.Add(name, pool);
                return pool.Instantiate(target);
            }
        }
        #endregion

        /// <summary>
        /// 对象池销毁池对象
        /// </summary>
        /// <param name="go"></param>
        /// <typeparam name="T"></typeparam>
        public void Destory(GameObject go) {
            var name = go.name;
            if (GameObjectMap.ContainsKey(name)) {
                GameObjectMap[name].Destory(go);
            } else {
                var pool = new Pool().Init(root, go.name);
                GameObjectMap.Add(name, pool);
                pool.Destory(go);
            }
        }

        /// <summary>
        /// 对象池释放池(一整个GameObject相关)
        /// </summary>
        /// <param name="go"></param>
        public void Free(GameObject go) {
            var name = go.name;
            if (!GameObjectMap.ContainsKey(name)) {
                $"{name} 池子不存在！".Log(GameData.Log.Error);
                return;
            }
            GameObjectMap[name].Free();
            GameObjectMap.Remove(name);
        }

        /// <summary>
        /// 预加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        public void PreLoad(string path, int count) {
            var temp = new List<GameObject>();
            for (int i = 0; i < count; i++) {
                var go = Instantiate(path);
                if (null == go)
                    break;
                temp.Add(go);
            }
            for (int i = 0; i < temp.Count; i++)
                Destory(temp[i]);
        }

        /// <summary>
        /// 预加载
        /// </summary>
        /// <param name="go"></param>
        /// <param name="count"></param>
        public void PreLoad(GameObject go, int count) {
            var temp = new List<GameObject>();
            for (int i = 0; i < count; i++) {
                var poolGo = Instantiate(go);
                if (null == poolGo)
                    break;
                temp.Add(poolGo);
            }
            for (int i = 0; i < temp.Count; i++)
                Destory(temp[i]);
        }
        #endregion
    
    }
}