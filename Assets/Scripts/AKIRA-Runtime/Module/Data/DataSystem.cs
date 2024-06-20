using System.Threading.Tasks;

namespace AKIRA.Manager {
    /// <summary>
    /// 数据管理器，存储，读表
    /// </summary>
    public class DataSystem : Singleton<DataSystem> {
        // 是否读取完成
        public bool IsCompleted { get; private set; } = false;

        // excel table
        public static TableCollection Table { get; private set; }

        // presisentence
        public static PresistentCollection Presistent { get; private set; }

        protected DataSystem() {}

        public override async Task Initialize() {
            Table = await new TableCollection().Initialize();
            Presistent = await new PresistentCollection().Initialize();
            IsCompleted = true;
        }

    }
}