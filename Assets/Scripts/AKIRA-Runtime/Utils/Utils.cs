using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using AKIRA.UIFramework;
using System.Linq;

public static partial class Utils {
    #region GameObject
    /// <summary>
    /// GameObject Dont Destory
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static GameObject DontDestory(this GameObject go) {
        GameObject.DontDestroyOnLoad(go);
        return go;
    }

    /// <summary>
    /// 加载 T
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Load<T>(this string path) where T : UnityEngine.Object {
        return Resources.Load<T>(path);
    }

#if UNITY_EDITOR
    /// <summary>
    /// <para>编辑器模式下，加载T</para>
    /// <para> AssetDatabase.LoadAssetAtPath </para>
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadAssetAtPath<T>(this string path) where T : UnityEngine.Object {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    /// <summary>
    /// <para>编辑器模式下，加载T[]</para>
    /// <para> AssetDatabase.LoadAllAssetsAtPath </para>
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> LoadAllAssetsAtPath<T>(this string path) where T : UnityEngine.Object {
        return AssetDatabase.LoadAllAssetsAtPath(path).OfType<T>();
    }
#endif

    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Instantiate<T>(this T com) where T : Component {
        return GameObject.Instantiate<T>(com);
    }

    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="com"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Instantiate<T>(this T com, Vector3 position, Quaternion rotation) where T : Component {
        return GameObject.Instantiate<T>(com, position, rotation);
    }
    
    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="com"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Instantiate<T>(this T com, Vector3 position, Quaternion rotation, Transform parent) where T : Component {
        return GameObject.Instantiate<T>(com, position, rotation, parent);
    }

    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static GameObject Instantiate(this GameObject obj) {
        var go = GameObject.Instantiate(obj);
        go.name = obj.name;
        return go;
    }

    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject Instantiate(this GameObject obj, Vector3 position, Quaternion rotation) {
        var go = GameObject.Instantiate(obj, position, rotation);
        go.name = obj.name;
        return go;
    }

    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject Instantiate(this GameObject obj, Vector3 position, Quaternion rotation, Transform parent) {
        var go = GameObject.Instantiate(obj, position, rotation, parent);
        go.name = obj.name;
        return go;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="time">销毁等待时间</param>
    public static void Destory(this GameObject obj, float time = 0) {
        GameObject.Destroy(obj, time);
    }

    /// <summary>
    /// 销毁
    /// </summary>
    /// <param name="com"></param>
    /// <param name="time">销毁等待时间</param>
    /// <typeparam name="T"></typeparam>
    public static void Destory<T>(this T com, float time = 0) where T : Component {
        GameObject.Destroy(com, time);
    }

    /// <summary>
    /// 设置父节点
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    /// <param name="isUI">
    ///     <para>是否是 UI</para>
    ///     <para>UI情况下防止父物体对子物体的影响</para>
    ///     <para>参考： https://blog.csdn.net/qq_42672770/article/details/109180796</para>
    /// </param>
    /// <returns></returns>
    public static GameObject SetParent(this GameObject child, GameObject parent, bool isUI = false) {
        child.transform.SetParent(parent.transform, !isUI);
        return child;
    }

    /// <summary>
    /// 设置父节点
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    /// <param name="isUI">
    ///     <para>是否是 UI</para>
    ///     <para>UI情况下防止父物体对子物体的影响</para>
    ///     <para>参考： https://blog.csdn.net/qq_42672770/article/details/109180796</para>
    /// </param>
    /// <returns></returns>
    public static T SetParent<T>(this T child, Component parent, bool isUI = false) where T : Component {
        child.transform.SetParent(parent.transform, !isUI);
        return child;
    }

    /// <summary>
    /// 获得 或 添加 组件
    /// </summary>
    /// <param name="go"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
        if (!go.TryGetComponent(out T value))
            value = go.AddComponent<T>();
        return value;
        // return go.GetComponent<T>() ?? go.AddComponent<T>();
    }

    /// <summary>
    /// 获得 或 添加组件
    /// </summary>
    /// <param name="component"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this Component component) where T : Component
        => GetOrAddComponent<T>(component.gameObject);

    /// <summary>
    /// 获得子物体组件
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetComponent<T>(this GameObject parent, string path) {
        var child = parent.transform.Find(path);
        return child.GetComponent<T>();
    }

    /// <summary>
    /// 获得子物体组件
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetComponent<T>(this Component parent, string path) {
        var child = parent.transform.Find(path);
        return child.GetComponent<T>();
    }

    /// <summary>
    /// UI 获得组件
    /// </summary>
    /// <param name="com"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetComponent<T>(this UIComponent com) {
        return com.transform.GetComponent<T>();
    }

    /// <summary>
    /// 获得Props数组
    /// </summary>
    /// <param name="parent"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] GetComponentProps<T>(this UIComponent com, Transform parent) where T : UIComponentProp, new() {
        var count = parent.childCount;
        List<T> result = new();
        var typeName = typeof(T).Name;
        for (int i = 0; i < count; i++) {
            var child = parent.GetChild(i);
            // 判断是否重命名
            if (child.TryGetComponent(out UIPropRenameComponent rename)) {
                if (rename.propName.Equals(typeName)) {
                    var component = new T();
                    component.Awake(child);
                    result.Add(component);
                    continue;                    
                }
            }
            // 去掉复制预制体名称带(1)/(2)的影响
            if (!typeName.Contains(child.name.Split("(")[0].Trim())) {
                result.AddRange(com.GetComponentProps<T>(child));        
            } else {
                var component = new T();
                component.Awake(child);
                result.Add(component);
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// 获得物体大小
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetFilterSize(this GameObject obj) {
        return GetFilterSize(obj.transform);
    }

    /// <summary>
    /// 获得物体大小
    /// </summary>
    /// <param name="component">从自身或子物体找第一个MeshFilter</param>
    /// <returns></returns>
    public static Vector3 GetFilterSize(this Component component) {
        var filter = component.GetComponentInChildren<MeshFilter>();
        var length = filter.sharedMesh.bounds.size;
        Vector3 size;
        size.x = length.x * filter.transform.lossyScale.x;
        size.y = length.y * filter.transform.lossyScale.y;
        size.z = length.z * filter.transform.lossyScale.z;
        return size;
    }

    /// <summary>
    /// 获得场景路径
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetPath(this GameObject obj) {
        return GetPath(obj.transform);
    }

    /// <summary>
    /// 获得场景路径
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static string GetPath(this Transform transform) {
        Stack<string> names = new Stack<string>();
        names.Push(transform.name);
        GetPath(transform, ref names);
        StringBuilder builder = new StringBuilder();
        while (names.Count != 0)
            builder.Append($"/{names.Pop()}");
        // 移除第一个 "/"
        builder.Remove(0, 1);
        return builder.ToString();
    }

    /// <summary>
    /// 获得名称节点
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="names"></param>
    private static void GetPath(this Transform transform, ref Stack<string> names) {
        if (transform.parent == null)
            return;
        transform = transform.parent;
        names.Push(transform.name);
        GetPath(transform, ref names);
    }

    /// <summary>
    /// <para>获得 <paramref name="component" /> ID（路径）</para>
    /// <para>遍历父节点</para>
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public static string GetComponentID(this Component component) {
        StringBuilder name = new StringBuilder(component.name);
        string lastName = "";
        Transform nextTrans = component.transform;
        while (nextTrans.parent != null) {
            name.Append(nextTrans.name + "_");
            lastName = nextTrans.name;
            nextTrans = nextTrans.parent;
        }

        name.Remove(name.Length - 1, 1);
        // name.ToString().Log();
        return name.ToString();
    }
    #endregion

    #region Color/String
    /// <summary>
    /// 富文本
    /// </summary>
    /// <param name="str"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string Colorful(this string str, string color) {
        return $"<color={color}>{str}</color>";
    }

    /// <summary>
    /// 富文本
    /// </summary>
    /// <param name="str"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string Colorful(this string str, Color color) {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
    }

    /// <summary>
    /// 富文本
    /// </summary>
    /// <param name="str"></param>
    /// <param name="color">System.Drawing.Color</param>
    /// <returns></returns>
    public static string Colorful(this string str, System.Drawing.Color color) {
        // 转16进制输出
        return Colorful(str, String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));
    }

    /// <summary>
    /// 富文本
    /// </summary>
    /// <param name="str"></param>
    /// <param name="size">value or percent with '%'</param>
    /// <returns></returns>
    public static string Size(this string str, string size) {
        return $"<size={size}>{str}</size>";
    }

    /// <summary>
    /// 富文本，鏈接
    /// </summary>
    /// <param name="url"></param>
    /// <param name="webName"></param>
    /// <returns></returns>
    public static string Link(this string url, string webName) {
        if (String.IsNullOrEmpty(webName))
            webName = url;
        return $"<a href=\"{url}\">{webName}</a>";
    }

    /// <summary>
    /// Color的简单转换（System.Drawing.Color => UnityEngine.Color）
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ToUnityColor(this System.Drawing.Color color) {
        return new Color(color.R / 255f, color.G / 255f, color.B / 255f);
    }

    /// <summary>
    /// Color的简单转换（System.Drawing.Color => UnityEngine.Color）
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ToUnityColor(this System.Drawing.Color color, float alpha) {
        return new Color(color.R / 255f, color.G / 255f, color.B / 255f, Mathf.Clamp(alpha, 0, 1));
    }

    /// <summary>
    /// <para>获得一个粉嫩的颜色</para>
    /// <para>来源: https://blog.csdn.net/weixin_43994445/article/details/98728416</para>
    /// </summary>
    /// <returns></returns>
    public static Color GetBeautifulColor() {
        float r = 0f, g = 0f, b = 0f;
        //定义3个颜色备用
        float c1 = 1f;
        float c2 = 150 / 255f;
        float c3 = UnityEngine.Random.Range(150 / 255f, 1f);
        //将3个颜色随机分配给R,G,B
        int choose = UnityEngine.Random.Range(0, 6);
        switch (choose)
        {
            case 0:
                r = c1; g = c2; b = c3; break;
            case 1:
                r = c1; g = c3; b = c2; break;
            case 2:
                r = c2; g = c1; b = c3; break;
            case 3:
                r = c2; g = c3; b = c1; break;
            case 4:
                r = c3; g = c1; b = c2; break;
            case 5:
                r = c3; g = c2; b = c1; break;
        }
        return new Color(r, g, b);
    }
    #endregion

    #region UI
    /// <summary>
    /// 屏幕坐标转 UGUI 坐标
    /// </summary>
    /// <param name="screenpos"></param>
    /// <returns></returns>
    public static Vector2 ScreenToUGUI(this Vector3 screenpos) {
        Vector2 screenpos2 = new Vector2(screenpos.x - (Screen.width / 2), screenpos.y - (Screen.height / 2));
        var UISize = UI.Rect.sizeDelta;
        Vector2 uipos;
        uipos.x = (screenpos2.x / Screen.width) * UISize.x;
        uipos.y = (screenpos2.y / Screen.height) * UISize.y;
        return uipos;
    }

    /// <summary>
    /// 屏幕坐标转UGUI坐标
    /// </summary>
    /// <param name="screenpos"></param>
    /// <returns></returns>
    public static Vector2 ScreenToUGUI(this Vector2 screenpos) {
        Vector2 screenpos2 = new Vector2(screenpos.x - (Screen.width / 2), screenpos.y - (Screen.height / 2));
        var UISize = UI.Rect.sizeDelta;
        Vector2 uipos;
        uipos.x = (screenpos2.x / Screen.width) * UISize.x;
        uipos.y = (screenpos2.y / Screen.height) * UISize.y;
        return uipos;
    }

    /// <summary>
    /// 屏幕坐标转 UGUI 坐标
    /// UI静态类没有初始化的时候添加Canvas.RectTransform使用
    /// </summary>
    /// <param name="screenpos"></param>
    /// <param name="canvasRect">Canvas.RectTransform</param>
    /// <returns></returns>
    public static Vector2 ScreenToUGUI(this Vector3 screenpos, RectTransform canvasRect) {
        Vector2 screenpos2 = new Vector2(screenpos.x - (Screen.width / 2), screenpos.y - (Screen.height / 2));
        var UISize = canvasRect.sizeDelta;
        Vector2 uipos;
        uipos.x = (screenpos2.x / Screen.width) * UISize.x;
        uipos.y = (screenpos2.y / Screen.height) * UISize.y;
        return uipos;
    }

    /// <summary>
    /// 世界坐标转 UGUI 坐标
    /// </summary>
    public static Vector2 WorldToUGUI(this Vector3 position) {
        Vector2 ScreenPoint = Camera.main.WorldToScreenPoint(position);
        Vector2 ScreenSize = new Vector2(Screen.width, Screen.height);
        ScreenPoint -= ScreenSize / 2;//将屏幕坐标变换为以屏幕中心为原点
        Vector2 anchorPos = ScreenPoint / ScreenSize * UI.Rect.sizeDelta;//缩放得到UGUI坐标
        return anchorPos;
    }

    /// <summary>
    /// 世界坐标转Canvas坐标
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector2 WorldToCanvas(this Vector3 position) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UI.Rect, UI.UICamera.WorldToScreenPoint(position), UI.UICamera, out Vector2 canvasPosition);
        return canvasPosition;
    }

    /// <summary>
    /// <para>运行模式下Texture转换成Texture2D</para>
    /// <para>来源：https://blog.csdn.net/s15100007883/article/details/80411638</para>
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public static Texture2D TextureToTexture2D(this Texture texture) {
        if (texture == null)
            return default;
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

    /// <summary>
    /// 移动端判断点击的位置是否有UI
    /// </summary>
    /// <param name="screenPosition">Input.mousePosition / Mouse.current.position.ReadValue()</param>
    /// <returns></returns>
    public static bool IsPointerOverUiObject(this Vector3 screenPosition) {
        if (!EventSystem.current.Equals(null)) {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        return true;
    }

    /// <summary>
    /// 移动端判断点击的位置是否有含 <see cref="type" /> 的UI
    /// </summary>
    /// <param name="screenPosition">Input.mousePosition / Mouse.current.position.ReadValue()</param>
    /// <param name="type">组件类型</param>
    /// <returns></returns>
    public static bool IsPointerOverUiObject(this Vector3 screenPosition, Type type) {
        if (!EventSystem.current.Equals(null)) {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            foreach (var result in results) {
                if (result.gameObject.GetComponent(type) != null)
                    return true;
            }

            return false;
        }

        return true;
    }
    #endregion

    #region Layer
    /// <summary>
    /// <para><paramref name="layer" /> 是否包含在 <paramref name="mask" /> 内</para>
    /// <para>来源：https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html</para>
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static bool IsLayerEqualMask(this int layer, LayerMask mask) {
        return mask == (mask | (1 << layer));
    }
    #endregion

    #region IEnumerable
    public static void Foreach<T>(this IEnumerable<T> values, Action<T> callback) {
        if (values == default)
            return;

        foreach (var value in values)
            callback.Invoke(value);
    }

    public static void Foreach<T>(this IEnumerable<T> values, Action<int, T> callback) {
        if (values == default)
            return;
        
        for (int i = 0; i < values.Count(); i++)
            callback?.Invoke(i, values.ElementAt(i));
    }
    #endregion

    #region 随机值
    /// <summary>
    /// 获得枚举随机值
    /// </summary>
    /// <param name="excepts">筛选集合</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandomValue<T>(params T[] excepts) where T : Enum {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        excepts.Foreach(except => values.Remove(except));
        return values[UnityEngine.Random.Range(0, values.Count)];
    }

    /// <summary>
    /// 根据权重获得随机对象
    /// </summary>
    /// <param name="values"></param>
    /// <param name="weight"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandomValue<T>(this IEnumerable<T> values, Func<T, float> weight) {
        if (values == default)
            return default;

        var map = new (T key, float value)[values.Count()];
        for (int i = 0; i < map.Length; i++) {
            var value = values.ElementAt(i);
            map[i] = (value, weight.Invoke(value) + (i == 0 ? 0 : map[i - 1].value));
        }
        var randomWeight = UnityEngine.Random.Range(0, map.Last().value);
        return map.First(m => m.value > randomWeight).key;

        // var totalWeight = values.Sum(value => weight.Invoke(value));
        // var randomWeight = UnityEngine.Random.Range(0, totalWeight);
        // return values.First(value => (randomWeight -= weight.Invoke(value)) <= 0);
    }
    #endregion

}