using System.Threading.Tasks;
using AKIRA;
using AKIRA.Attribute;
using AKIRA.Manager;

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
}