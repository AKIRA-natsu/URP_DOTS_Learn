using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using AKIRA;
using UnityEngine;
using UnityEngine.Networking;

public class XML {
    /// <summary>
    /// 路径
    /// </summary>
    /// <value></value>
    private string path;
    /// <summary>
    /// xml 文件
    /// </summary>
    /// <returns></returns>
    private XmlDocument xml = new XmlDocument();
    /// <summary>
    /// <para>只读</para>
    /// <para>true下安卓平台修改路径</para>
    /// </summary>
    private bool @readonly;

    /// <summary>
    /// 默认存储StreamingAssets
    /// </summary>
    /// <param name="path"></param>
    /// <param name="readonly">true下 Android | IOS 平台修改路径</param>
    public XML(string path, bool @readonly = true) {
        this.@readonly = @readonly;

        if (!@readonly && !Application.isEditor) {
            this.path = Path.Combine(Application.persistentDataPath, path);
        } else {
            this.path = Path.Combine(Application.streamingAssetsPath, path);
        }
    }

    /// <summary>
    /// 此构造函数不需要判断是否存在，判断TextAsset即可
    /// </summary>
    /// <param name="asset"></param>
    public XML(TextAsset asset) {
        xml.LoadXml(asset.text);
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="create"></param>
    public void Create(Action<XmlDocument> create) {
        // 设置编码
        XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "UTF-8", "");
        
        // 具体内容
        create.Invoke(xml);
        
        xml.Save(path);
        $"XML: 创建{path}成功".Log(GameData.Log.Success);
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="read"></param>
    public void Read(Action<XmlDocument> read) {
        read.Invoke(xml);
        $"XML: 读取{path}成功".Log(GameData.Log.Success);
    }

    /// <summary>
    /// 添加/更新/删除
    /// </summary>
    /// <param name="update"></param>
    public void Update(Action<XmlDocument> update) {
        update.Invoke(xml);

        xml.Save(path);
        $"XML: 更新{path}成功".Log(GameData.Log.Success);
    }

    /// <summary>
    /// 删除xml文件
    /// </summary>
    public void Delete() {
        if (!Exist())
            return;
        
        File.Delete(path);
        xml = new XmlDocument();
    }

    /// <summary>
    /// <para>检查是否存在文件</para>
    /// <para>Android | IOS 使用UnityWebRequest</para>
    /// <para>来源：https://answers.unity.com/questions/1247609/android-streamingasset-problems-with-xml-files.html</para>
    /// </summary>
    /// <returns></returns>
    public bool Exist() {
#if !UNITY_EDITOR
        if (@readonly) {
            UnityWebRequest request = UnityWebRequest.Get(path);
            var operation = request.SendWebRequest();
            while (!operation.isDone) {}
            if (request.result == (UnityWebRequest.Result.ConnectionError | UnityWebRequest.Result.ProtocolError))
                return false;
            
            xml.LoadXml(request.downloadHandler.text);
        } else {
#endif
        if (!File.Exists(path))
            return false;

        xml.Load(path);
#if !UNITY_EDITOR
        }
#endif
        return true;
    }

    /// <summary>
    /// 类转XML，序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string XmlSerialize<T>(T obj) where T : class {
        try {
            using (StringWriter sw = new StringWriter()) {
                Type type = typeof(T);
                XmlSerializer serializer = new XmlSerializer(type);
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        } catch (Exception e) {
            e.Error();
        }
        return default;
    }

    /// <summary>
    /// XML转类，反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static T DeSerialize<T>(string serialize) where T : class {
        try {
            using (StringReader sr = new StringReader(serialize)) {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(sr) as T;
            }
        } catch (Exception e) {
            e.Error();
        }
        return default;
    }
}