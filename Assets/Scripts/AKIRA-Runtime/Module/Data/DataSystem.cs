using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using AKIRA.Security;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public interface IInfo {}

namespace AKIRA.Manager {
    /// <summary>
    /// 数据管理器，存储，读表
    /// </summary>
    public partial class DataSystem : Singleton<DataSystem> {
        protected DataSystem() { }

        public override async Task Initialize() {
            // 配合Excel表读取
            var config = GameConfig.Instance.GetConfig<ExcelDataConfig>();
            if (config != null) {
                $"Start Load Tables".Log();
                if (config.output.Contains("Resources")) {
                    await LoadByResources(config);
                } else {
                    await LoadByStreamingAssets(config);
                }
            } else {
                $"Skip Load Tables".Log();
            }
            
            $"Start Load Presisents".Log();
            await LoadInfos();
        }

        #region Table
        // type key -- json string
        private Dictionary<string, string> tableMap = new();

        private async Task LoadByResources(ExcelDataConfig config) {
            var extense = config.encrypt ? ".bytes" : ".json";
            foreach (var file in config.paths) {
                var asset = file.Replace(extense, "").Load<TextAsset>();
                await Task.Yield();
                $"Read Data File => {file}".Log();
                if (config.encrypt)
                    Read(asset.bytes, config.encryptKey, GetDecryptLv(file));
                else
                    Read(asset.text, GetDecryptLv(file));
                await Task.Yield();
            }
        }

        private async Task LoadByStreamingAssets(ExcelDataConfig config) {
            foreach (var file in config.paths) {
                UnityWebRequest request = UnityWebRequest.Get(Path.Combine(AssetSystem.Instance.PreformPath, file));
                request.SendWebRequest();
                while (!request.isDone) {
                    if (request.result == (UnityWebRequest.Result.ConnectionError | UnityWebRequest.Result.ProtocolError))
                        break;
                }
                    
                $"Read Data File => {file}".Log();
                if (config.encrypt)
                    Read(request.downloadHandler.data, config.encryptKey, GetDecryptLv(file));
                else
                    Read(request.downloadHandler.text, GetDecryptLv(file));
                await Task.Yield();
            }
        }

        /// <summary>
        /// 获得类名
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string GetDecryptLv(string file) {
            var startIndex = file.LastIndexOf('\\');
            var endIndex = file.LastIndexOf('.');
            return file.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        /// <summary>
        /// 读取（加密）
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="key"></param>
        /// <param name="lv"></param>
        private void Read(byte[] bytes, string key, string lv) {
            using var memory = new MemoryStream(SecurityTool.Decrypt(bytes, key, lv));
            var formatter = new BinaryFormatter();
            tableMap.Add(lv, formatter.Deserialize(memory).ToString());
            memory.Close();
        }

        /// <summary>
        /// 读取（非加密）
        /// </summary>
        /// <param name="json"></param>
        /// <param name="lv"></param>
        private void Read(string json, string lv) {
            tableMap.Add(lv, json);
        }

        /// <summary>
        /// 获得表数据，返回表数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetTable<T>() {
            var key = typeof(T).Name;
            if (tableMap.ContainsKey(key)) {
                return JsonConvert.DeserializeObject<List<T>>(tableMap[key]);
            } else {
                return default;
            }
        }
        #endregion
    

        #region presistent
        private string localPath;

        private Dictionary<string, IInfo> infos = new();

        private async Task LoadInfos() {
            localPath = Path.Combine(Application.persistentDataPath, Application.productName);
            var types = typeof(IInfo).Name.GetConfigTypeByInterface();
            foreach (var type in types) {
                var json = type.Name.GetString();
                IInfo info;
                if (string.IsNullOrEmpty(json))
                    info = type.CreateInstance<IInfo>();
                else
                    try {
                        info = JsonConvert.DeserializeObject(json, type) as IInfo;
                    } catch {
                        throw new JsonException($"error deserialize with {type}");
                    }
                infos.Add(type.Name, info);
                await Task.Yield();
            }
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetInfo<T>() where T : class, IInfo {
            infos.TryGetValue(typeof(T).Name, out IInfo value);
            return value as T;
        }

        /// <summary>
        /// 存储系统存档
        /// </summary>
        /// <param name="info"></param>
        public void SaveInfo(IInfo info) {
            info.GetType().Name.Save(JsonConvert.SerializeObject(info));
        }
        #endregion

    }
}