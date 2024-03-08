using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using AKIRA.Security;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AKIRA.Manager {
    /// <summary>
    /// 数据表处理管理器
    /// </summary>
    public class TableCollectionContorller : IController {
        private Dictionary<string, string> tableMap = new();

        public async Task Initialize() {
            var config = GameConfig.Instance.GetConfig<ExcelDataConfig>();
            await Task.Yield();
            if (config.output.Contains("Resources")) {
                await LoadByResources(config);
            } else {
                await LoadByStreamingAssets(config);
            }
        }

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
        public List<T> GetTableData<T>() {
            var key = typeof(T).Name;
            if (tableMap.ContainsKey(key)) {
                return JsonConvert.DeserializeObject<List<T>>(tableMap[key]);
            } else {
                return default;
            }
        }
    }
}