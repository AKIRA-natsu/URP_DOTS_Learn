using System.Threading.Tasks;
using AKIRA;
using AKIRA.Attribute;
using AKIRA.Manager;

/// <summary>
/// 其他
/// </summary>
[SystemLauncher(5)]
public class MetaSystem : Singleton<MetaSystem> {
    protected MetaSystem() {}

    public override async Task Initialize() {
        await CreateController<FXController>();
    }
}