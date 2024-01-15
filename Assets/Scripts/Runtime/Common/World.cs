using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AKIRA.Manager;
using UnityEngine;

namespace AKIRA {
    /// <summary>
    /// systems & controllers & entities in game world
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
        
        public static T CreateEntity<T>(string path, object data = null) where T : EntityBase
            => CreateEntity<T>(path, Vector3.zero, Quaternion.identity, data);
        
        public static T CreateEntity<T>(string path, Vector3 position, object data = null) where T : EntityBase
            => CreateEntity<T>(path, position, Quaternion.identity, data);

        public static T CreateEntity<T>(string path, Vector3 position, Quaternion rotation, object data = null) where T : EntityBase {
            EntityBase entity;
            if (typeof(T).IsSubclassOf(typeof(PoolEntityBase)))
                entity = ObjectPool.Instance.Instantiate<PoolEntityBase>(path, position, rotation, data);
            else
                entity = AssetSystem.Instance.LoadObject<T>(path).Instantiate(position, rotation);
            var key = typeof(T);
            if (entities.ContainsKey(key))
                entities[key].Add(entity);
            else
                entities[key] = new List<EntityBase>() { entity };
            return entity as T;
        }

        public static void DestoryEntity<T>(T com, object data = null) where T: EntityBase {
            var type = typeof(T);
            if (!entities.ContainsKey(type))
                return;
            entities[type].Remove(com);

            if (type.IsSubclassOf(typeof(PoolEntityBase)))
                ObjectPool.Instance.Destory(com as PoolEntityBase, data);
            else
                GameObject.Destroy(com.gameObject);
        }

        public static void DestoryEntity<T>(object data = null) where T : EntityBase {
            var type = typeof(T);
            if (!entities.ContainsKey(type))
                return;
            foreach (var entity in entities[type])
                DestoryEntity(entity, data);
            entities.Remove(type);
        }

        public static IEnumerable<T> GetEntities<T>() where T : EntityBase {
            var key = typeof(T);
            if (entities.ContainsKey(key))
                return entities[key].Cast<T>();
            return default;
        }

        public static T GetEntity<T>() where T : EntityBase
            => GetEntities<T>()?.First();
        #endregion

        #region entities ienumerable
        #endregion
    }
}
