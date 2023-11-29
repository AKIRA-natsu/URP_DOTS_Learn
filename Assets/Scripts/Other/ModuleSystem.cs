using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AKIRA;
using AKIRA.Attribute;
using AKIRA.Manager;

/// <summary>
/// 数据基类
/// </summary>
public interface IData {
    /// <summary>
    /// <para>获得随机值</para>
    /// <para>内部不需要new()，只需要对成员进行随机就好，ModuleSystem会进行实例化调用后返回</para>
    /// </summary>
    void GetRandomValue();
}

/// <summary>
/// <para>模块系统</para>
/// <para>尝试获得模块数据（非程序集内的模块，正常拿程序集内的东西也不需要这个）</para>
/// </summary>
[SystemLauncher(-1)]
public class ModuleSystem : Singleton<ModuleSystem> {
    // 系统单例数组
    private Type[] systems;

    protected ModuleSystem() {}

    public override async Task Initialize() {
        // 非程序集内的单例
        systems = Assembly.Load(GameData.DLL.Default).GetTypes()
            .Where(type => type.GetInterface("ISystem") != null && !type.Name.Contains("Singleton"))
            .ToArray();
        await Task.Yield();
    }

    /// <summary>
    /// 获得模块
    /// </summary>
    /// <param name="module"></param>
    /// <returns></returns>
    private Type GetModule(string module) {
        return systems.SingleOrDefault(system => system.Name.ToLower().Contains(module));
    }

    /// <summary>
    /// 尝试获得模块字段值
    /// </summary>
    /// <param name="module">模块名称：查询所以已存在的ISystem中是否包含模块</param>
    /// <param name="controllerName">
    ///     <para>管理器名称：查询模块中的Controllers列表是否存在管理器</para>
    ///     <para>可空，跳过管理器直接拿模块中的字段</para>
    /// </param>
    /// <param name="fieldName">字段名称，必须一致</param>
    /// <returns></returns>
    public object TryGetModuleFieldValue(string module, string controllerName, string fieldName) {
        var system = GetModule(module);
        if (system == null) {
            $"不存在模块 {module}".Log(GameData.Log.Test);
            return default;
        }

        // 获得单例和管理器列表
        var instance = system.BaseType.GetProperty("Instance").GetValue(system.BaseType);
        var controllers = system.BaseType.GetField("controllers", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (!string.IsNullOrWhiteSpace(controllerName)) {
            var controller = (controllers.GetValue(instance) as List<IController>)
                .SingleOrDefault(controller => controller.GetType().Name.ToLower().Contains(controllerName.ToLower()));
            if (controller == null) {
                $"不存在管理器 {controllerName}".Log(GameData.Log.Test);
                return default;
            }
            var field = controller.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return field?.GetValue(controller) ?? default;
        } else {
            var field = system.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return field?.GetValue(instance) ?? default;
        }
    }

    /// <summary>
    /// 尝试获得模块属性值
    /// </summary>
    /// <param name="module">模块名称：查询所以已存在的ISystem中是否包含模块</param>
    /// <param name="controllerName">
    ///     <para>管理器名称：查询模块中的Controllers列表是否存在管理器</para>
    ///     <para>可空，跳过管理器直接拿模块中的属性</para>
    /// </param>
    /// <param name="propertyName">属性名称，必须一致</param>
    /// <returns></returns>
    public object TryGetModulePropertyValue(string module, string controllerName, string propertyName) {
        var system = GetModule(module);
        if (system == null) {
            $"不存在模块 {module}".Log(GameData.Log.Test);
            return default;
        }

        // 获得单例和管理器列表
        var instance = system.BaseType.GetProperty("Instance").GetValue(system.BaseType);
        var controllers = system.BaseType.GetField("controllers", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (!string.IsNullOrWhiteSpace(controllerName)) {
            var controller = (controllers.GetValue(instance) as List<IController>)
                .SingleOrDefault(controller => controller.GetType().Name.ToLower().Contains(controllerName.ToLower()));
            if (controller == null) {
                $"不存在管理器 {controllerName}".Log(GameData.Log.Test);
                return default;
            }
            var property = controller.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(controller) ?? default;
        } else {
            var property = system.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(instance) ?? default;
        }
    }

    /// <summary>
    /// 尝试获得模块方法返回值
    /// </summary>
    /// <param name="module">模块名称：查询所以已存在的ISystem中是否包含模块</param>
    /// <param name="controllerName">
    ///     <para>管理器名称：查询模块中的Controllers列表是否存在管理器</para>
    ///     <para>可空，跳过管理器直接拿模块中的方法</para>
    /// </param>
    /// <param name="methodName">方法名称，必须一致</param>
    /// <param name="datas">方法参数</param>
    /// <returns></returns>
    public object TryGetModuleMethodValue(string module, string controllerName, string methodName, params object[] datas) {
        var system = GetModule(module);
        if (system == null) {
            $"不存在模块 {module}".Log(GameData.Log.Test);
            return default;
        }

        // 获得单例和管理器列表
        var instance = system.BaseType.GetProperty("Instance").GetValue(system.BaseType);
        var controllers = system.BaseType.GetField("controllers", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (!string.IsNullOrWhiteSpace(controllerName)) {
            var controller = (controllers.GetValue(instance) as List<IController>)
                .SingleOrDefault(controller => controller.GetType().Name.ToLower().Contains(controllerName.ToLower()));
            if (controller == null) {
                $"不存在管理器 {controllerName}".Log(GameData.Log.Test);
                return default;
            }
            var method = controller.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return method?.Invoke(controller, datas) ?? default;
        } else {
            var method = system.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return method?.Invoke(instance, datas) ?? default;
        }
    }
    
    /// <summary>
    /// <para>尝试获得模块字段值</para>
    /// <para>如果找不到会返回T.GetRandomValue</para>
    /// </summary>
    /// <param name="module">模块名称：查询所以已存在的ISystem中是否包含模块</param>
    /// <param name="controllerName">
    ///     <para>管理器名称：查询模块中的Controllers列表是否存在管理器</para>
    ///     <para>可空，跳过管理器直接拿模块中的字段</para>
    /// </param>
    /// <param name="fieldName">字段名称，必须一致</param>
    /// <returns></returns>
    public T TryGetModuleFieldValue<T>(string module, string controllerName, string fieldName) where T : IData {
        var system = GetModule(module);
        if (system == null) {
            $"不存在模块 {module}".Log(GameData.Log.Test);
            return default;
        }

        // 获得单例和管理器列表
        var instance = system.BaseType.GetProperty("Instance").GetValue(system.BaseType);
        var controllers = system.BaseType.GetField("controllers", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (!string.IsNullOrWhiteSpace(controllerName)) {
            var controller = (controllers.GetValue(instance) as List<IController>)
                .SingleOrDefault(controller => controller.GetType().Name.ToLower().Contains(controllerName.ToLower()));
            if (controller == null) {
                $"不存在管理器 {controllerName}".Log(GameData.Log.Test);
                return default;
            }
            var field = controller.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)field?.GetValue(controller) ?? GetDataRandomValue<T>();
        } else {
            var field = system.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)field?.GetValue(instance) ?? GetDataRandomValue<T>();
        }
    }

    /// <summary>
    /// <para>尝试获得模块属性值</para>
    /// <para>如果找不到会返回T.GetRandomValue</para>
    /// </summary>
    /// <param name="module">模块名称：查询所以已存在的ISystem中是否包含模块</param>
    /// <param name="controllerName">
    ///     <para>管理器名称：查询模块中的Controllers列表是否存在管理器</para>
    ///     <para>可空，跳过管理器直接拿模块中的属性</para>
    /// </param>
    /// <param name="propertyName">属性名称，必须一致</param>
    /// <returns></returns>
    public T TryGetModulePropertyValue<T>(string module, string controllerName, string propertyName) where T : IData {
        var system = GetModule(module);
        if (system == null) {
            $"不存在模块 {module}".Log(GameData.Log.Test);
            return default;
        }

        // 获得单例和管理器列表
        var instance = system.BaseType.GetProperty("Instance").GetValue(system.BaseType);
        var controllers = system.BaseType.GetField("controllers", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (!string.IsNullOrWhiteSpace(controllerName)) {
            var controller = (controllers.GetValue(instance) as List<IController>)
                .SingleOrDefault(controller => controller.GetType().Name.ToLower().Contains(controllerName.ToLower()));
            if (controller == null) {
                $"不存在管理器 {controllerName}".Log(GameData.Log.Test);
                return default;
            }
            var property = controller.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)property?.GetValue(controller) ?? GetDataRandomValue<T>();
        } else {
            var property = system.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)property?.GetValue(instance) ?? GetDataRandomValue<T>();
        }
    }

    /// <summary>
    /// <para>尝试获得模块方法返回值</para>
    /// <para>如果找不到会返回T.GetRandomValue</para>
    /// </summary>
    /// <param name="module">模块名称：查询所以已存在的ISystem中是否包含模块</param>
    /// <param name="controllerName">
    ///     <para>管理器名称：查询模块中的Controllers列表是否存在管理器</para>
    ///     <para>可空，跳过管理器直接拿模块中的方法</para>
    /// </param>
    /// <param name="methodName">方法名称，必须一致</param>
    /// <param name="datas">方法参数</param>
    /// <returns></returns>
    public T TryGetModuleMethodValue<T>(string module, string controllerName, string methodName, params object[] datas) where T : IData {
        var system = GetModule(module);
        if (system == null) {
            $"不存在模块 {module}".Log(GameData.Log.Test);
            return default;
        }

        // 获得单例和管理器列表
        var instance = system.BaseType.GetProperty("Instance").GetValue(system.BaseType);
        var controllers = system.BaseType.GetField("controllers", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (!string.IsNullOrWhiteSpace(controllerName)) {
            var controller = (controllers.GetValue(instance) as List<IController>)
                .SingleOrDefault(controller => controller.GetType().Name.ToLower().Contains(controllerName.ToLower()));
            if (controller == null) {
                $"不存在管理器 {controllerName}".Log(GameData.Log.Test);
                return default;
            }
            var method = controller.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)method?.Invoke(controller, datas) ?? GetDataRandomValue<T>();
        } else {
            var method = system.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T)method?.Invoke(instance, datas) ?? GetDataRandomValue<T>();
        }
    }

    /// <summary>
    /// <para>尝试获得模块方法返回值</para>
    /// <para>争对数组对象</para>
    /// <para>如果找不到会返回T.GetRandomValue</para>
    /// </summary>
    /// <param name="module">模块名称：查询所以已存在的ISystem中是否包含模块</param>
    /// <param name="controllerName">
    ///     <para>管理器名称：查询模块中的Controllers列表是否存在管理器</para>
    ///     <para>可空，跳过管理器直接拿模块中的方法</para>
    /// </param>
    /// <param name="methodName">方法名称，必须一致</param>
    /// <param name="count">如果最终找不到，返回随机生成的一组数量，count > 0</param>
    /// <param name="datas">方法参数</param>
    /// <returns></returns>
    public T[] TryGetModuleMethodValue<T>(string module, string controllerName, string methodName, int count, params object[] datas) where T : IData {
        var system = GetModule(module);
        if (system == null) {
            $"不存在模块 {module}".Log(GameData.Log.Test);
            return default;
        }

        // 获得单例和管理器列表
        var instance = system.BaseType.GetProperty("Instance").GetValue(system.BaseType);
        var controllers = system.BaseType.GetField("controllers", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (!string.IsNullOrWhiteSpace(controllerName)) {
            var controller = (controllers.GetValue(instance) as List<IController>)
                .SingleOrDefault(controller => controller.GetType().Name.ToLower().Contains(controllerName.ToLower()));
            if (controller == null) {
                $"不存在管理器 {controllerName}".Log(GameData.Log.Test);
                return default;
            }
            var method = controller.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T[])method?.Invoke(controller, datas) ?? GetDataRandomValue<T>(count);
        } else {
            var method = system.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return (T[])method?.Invoke(instance, datas) ?? GetDataRandomValue<T>(count);
        }
    }

    /// <summary>
    /// 获得IData一个随机值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetDataRandomValue<T>() where T : IData {
        var data = (T)typeof(T).CreateInstance<T>();
        data.GetRandomValue();
        return data;
    }

    /// <summary>
    /// 获得IData一个数组的随机值
    /// </summary>
    /// <param name="count">数组数量</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] GetDataRandomValue<T>(int count) where T : IData {
        if (count <= 0)
            return default;

        T[] res = new T[count];
        for (int i = 0; i < count; i++)
            res[i] = GetDataRandomValue<T>();
        return res;
    }
}