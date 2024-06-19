using System.Threading.Tasks;
using AKIRA.Attribute;
using AKIRA.Manager;
using Cysharp.Threading.Tasks;

/// <summary>
/// 其他
/// </summary>
[SystemLauncher(5)]
public class MetaSystem : Singleton<MetaSystem> {
    public FXController FX { get; private set; } = new();

    protected MetaSystem() {}

    public override async Task Initialize() {
        await UniTask.Yield();
    }
}