using System.Threading.Tasks;
using AKIRA.Attribute;

namespace AKIRA.Manager {
    /// <summary>
    /// <para>设置系统</para>
    /// <para>设置界面的一堆配置</para>
    /// </summary>
    [SystemLauncher(-1)]
    public class SettingSystem : Singleton<SettingSystem> {
        protected SettingSystem() {}

        public override async Task Initialize() {
#if !UNITY_ANDROID
            await CreateController<ReportController>(GameData.DLL.AKIRA_Runtime);
#endif
        }
    }
}