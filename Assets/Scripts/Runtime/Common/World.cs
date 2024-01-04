using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AKIRA.Manager;

namespace AKIRA {
    /// <summary>
    /// systems & controllers in game world
    /// </summary>
    public static class World {
        private static List<ISystem> systems = new();

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
        
    }
}