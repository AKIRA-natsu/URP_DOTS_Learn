using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AKIRA.Manager;
using UnityEngine;

namespace AKIRA {
    /// <summary>
    /// systems & controllers & entities & component in game world
    /// </summary>
    public static class World {
        #region init
        // init by gamemanager
        public static void Init() {
            systems.Clear();
            var types = new List<Type>();
            types.AddRange(typeof(ISystem).FullName.GetConfigTypeByInterface(GameData.DLL.Default));
            types.AddRange(typeof(ISystem).FullName.GetConfigTypeByInterface(GameData.DLL.AKIRA_Runtime));

            var flag = BindingFlags.Static | BindingFlags.NonPublic;
            foreach (var t in types) {
                if (t.BaseType.GetField("instance", flag)?.GetValue(null) is not ISystem value)
                    continue;
                systems.Add(value);
            }
        }
        #endregion

        #region systems
        private static List<ISystem> systems = new();

        public static T GetWorldSystem<T>() where T : ISystem => (T)GetWorldSystem(typeof(T));
        public static ISystem GetWorldSystem(Type type) {
            var system = systems.SingleOrDefault(s => s.GetType() == type);
            if (system != null)
                return system;
            // not found systems, try to get all system again
            Init();
            system = systems.SingleOrDefault(s => s.GetType() == type);
            
            return system;
        }
        #endregion

        #region controllers
        public static T GetOrCreateWorldSystem<T>() where T : ISystem {
            var system = GetWorldSystem<T>();

            if (system == null) {
                var type = typeof(T).BaseType;
                if (type.Name.Contains("Mono")) {
                    system = (T)type.GetMethod("GetOrCreateDefaultInstance", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
                    systems.Add(system);
                } else {
                    system = (T)type.GetProperty("Instance").GetValue(null);
                    systems.Add(system);
                }
                system?.Initialize();
            }

            return system;
        }

        public static T GetWorldController<T>() where T : IController => (T)GetWorldController(typeof(T));
        public static IController GetWorldController(Type type) {
            foreach (var system in systems) {
                var method = system.GetType().GetMethod("GetController");
                method = method.MakeGenericMethod(type);
                var res = (IController)method.Invoke(system, null);
                if (res != null)
                    return res;
            }
            return default;
        }
        #endregion

        #region entities
        private static Dictionary<Type, List<EntityBase>> entities = new();
        
        public static T CreateEntity<T>(string path) where T : EntityBase
            => CreateEntity<T>(path, Vector3.zero, Quaternion.identity, null);

        public static T CreateEntity<T>(string path, Transform parent) where T : EntityBase
            => CreateEntity<T>(path, Vector3.zero, Quaternion.identity, parent);
        
        public static T CreateEntity<T>(string path, Vector3 position) where T : EntityBase
            => CreateEntity<T>(path, position, Quaternion.identity, null);

        public static T CreateEntity<T>(string path, Vector3 position, Quaternion rotation) where T : EntityBase
            => CreateEntity<T>(path, position, rotation, null);

        public static T CreateEntity<T>(string path, Vector3 position, Quaternion rotation, Transform parent) where T : EntityBase {
            var entity = ObjectPool.Instance.Instantiate<T>(path, position, rotation, parent);
            var key = typeof(T);
            if (entities.ContainsKey(key))
                entities[key].Add(entity);
            else
                entities[key] = new List<EntityBase>() { entity };
            return entity as T;
        }

        public static void DestoryEntity<T>(T entity) where T: EntityBase {
            var type = typeof(T);
            if (!entities.ContainsKey(type))
                return;
            entities[type].Remove(entity);

            if (componentDatas.ContainsKey(entity))
                componentDatas.Remove(entity);

            ObjectPool.Instance.Destory(entity);
        }

        public static void DestoryEntity<T>() where T : EntityBase {
            var type = typeof(T);
            if (!entities.ContainsKey(type))
                return;
            var count = entities[type].Count;
            for (int i = 0; i < count; i++)            
                DestoryEntity(entities[type][0] as T);
            entities.Remove(type);
        }

        public static IEnumerable<T> GetEntities<T>() where T : EntityBase {
            var key = typeof(T);
            if (entities.ContainsKey(key))
                return entities[key].Cast<T>();
            return default;
        }

        public static U GetSingleEntity<T, U>() where T : EntityBase where U : T {
            var type = typeof(U);
            return (U)(GetEntities<T>()?.SingleOrDefault(entity => entity.GetType().Equals(type)));
        }
        #endregion

        #region entities ienumerable
        public static IEnumerable<T> Query<U, T>(this IEnumerable<U> ienumerable) where T : U {
            return ienumerable?.Where(element => element is T)?.Cast<T>() ?? default;
        }
        #endregion
    
        #region components
        private static Dictionary<EntityBase, List<IComponentData>> componentDatas = new();

        public static void AddComponentData(this EntityBase entity, IComponentData component) {
            if (componentDatas.ContainsKey(entity))
                componentDatas[entity].Add(component);
            else
                componentDatas[entity] = new() { component };
        }

        public static void AddComponentData(this EntityBase entity, params IComponentData[] components) {
            if (componentDatas.ContainsKey(entity))
                componentDatas[entity].AddRange(components);
            else
                componentDatas[entity] = new(components);
        }

        public static T GetComponentData<T>(this EntityBase entity) where T : IComponentData {
            var type = typeof(T);
            if (componentDatas.ContainsKey(entity))
                return (T)componentDatas[entity].SingleOrDefault(data => data.GetType().Equals(type));
            else
                return default;
        }

        public static void RemoveComponentData<T>(this EntityBase entity, T component) where T : IComponentData {
            if (!componentDatas.ContainsKey(entity))
                return;
            componentDatas[entity].Remove(component);
            component.Dispose();
        }

        public static void RemoveComponentData<T>(this EntityBase entity) where T : IComponentData {
            if (!componentDatas.ContainsKey(entity))
                return;
            var type = typeof(T);
            var components = componentDatas[entity];
            for (int i = 0; i < components.Count; i++) {
                var component = components[i];
                if (!component.GetType().Equals(type))
                    continue;
                components.RemoveAt(i--);
                component.Dispose();
            }
        }

        public static void RemoveComponentData<T>(T component) where T : IComponentData {
            foreach (var kvp in componentDatas) {
                if (!kvp.Value.Contains(component))
                    continue;
                kvp.Value.Remove(component);
                component.Dispose();
            }
        }

        public static IEnumerable<T> Query<T>() where T : IComponentData {
            var type = typeof(T);
            List<IComponentData> res = new();
            componentDatas.Values.Foreach(components => res.AddRange(components.Where(component => component.GetType().Equals(type))));
            return res.Cast<T>();
        }

        public static IEnumerable<KeyValuePair<EntityBase, T>> QueryWithEntity<T>() where T : IComponentData {
            var type = typeof(T);
            List<KeyValuePair<EntityBase, T>> res = new();
            for (int i = 0; i < componentDatas.Count; i++) {
                var kvp = componentDatas.ElementAt(i);
                var component = kvp.Value.SingleOrDefault(data => data.GetType().Equals(type));
                if (component == null)
                    continue;
                res.Add(new KeyValuePair<EntityBase, T>(kvp.Key, (T)component));
            }
            return res;
        }
        #endregion
    }
}
