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
    /// <para>内部不需要new()，只需要对成员进行随机就好，DataSystem会进行实例化调用后返回</para>
    /// </summary>
    void GetRandomValue();
}

/// <summary>
/// 数据管理器
/// </summary>
[SystemLauncher(-10)]
public partial class DataSystem : Singleton<DataSystem> {
    protected DataSystem() { }

    public override async Task Initialize() {
        // 配合Excel表读取
        // 通过调用 var list = GetController<TableCollectionContorller>().GetTableData<T>(); 获得list列表
        await CreateController<TableCollectionContorller>(GameData.DLL.AKIRA_Runtime);

    }

    /// <summary>
    /// create random data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T CreateData<T>() where T : IData {
        T data = (T)typeof(T).CreateInstance();
        data.GetRandomValue();
        return data;
    }

    /// <summary>
    /// create random data list
    /// </summary>
    /// <param name="count"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T[] CreateData<T>(int count) where T : IData {
        if (count <= 0)
            return default;

        T[] res = new T[count];
        for (int i = 0; i < count; i++)
            res[i] = CreateData<T>();
        return res;
    }
}