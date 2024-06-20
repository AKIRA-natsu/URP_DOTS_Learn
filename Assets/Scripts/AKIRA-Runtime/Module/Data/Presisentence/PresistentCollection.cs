using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// save as UnityEngine.PlayerPrefs
/// </summary>
public interface ISystemInfo : AKIRA.Manager.IInfo {}

/// <summary>
/// save as FileStream, because of multi-save, only have one IGameInfo Global
/// </summary>
public interface IGameInfo : AKIRA.Manager.IInfo {
    string GUID { get; set; }              // file name guid
    bool IsAutoSave { get; set; }
    long LastSaveTime { get; set; }        // save in dic
    long LastUseTime { get; set; }
}

namespace AKIRA.Manager {
    public interface IInfo {
        Dictionary<string, string> GetSaveDic();
        void SetSaveDic(Dictionary<string, string> dic);
    }

    public class PresistentCollection {
        private string localPath;

        /// <summary>
        /// typeof(ISystemInfo).Name -- value
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, ISystemInfo> systemInfos = new();

        /// <summary>
        /// Global infos
        /// </summary>
        private IGameInfo gameInfo;

        /// <summary>
        /// record for gameinfo file name
        /// first is auto save
        /// </summary>
        /// <returns></returns>
        private List<string> guids = new();

        public async Task<PresistentCollection> Initialize() {
            $"Start Load Presistent".Log();
            localPath = Path.Combine(Application.persistentDataPath, "SaveData");
            if (!Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);

            // load system info
            var types = typeof(ISystemInfo).Name.GetConfigTypeByInterface();
            foreach (var type in types) {
                var dicJson = type.Name.GetString();
                if (!string.IsNullOrEmpty(dicJson)) {
                    try {
                        var info = type.CreateInstance() as ISystemInfo;
                        info.SetSaveDic(JsonConvert.DeserializeObject<Dictionary<string, string>>(dicJson));
                        systemInfos.Add(type.Name, info);
                    } catch {
                        throw new JsonException($"error deserialize with {type}");
                    }
                }
                await Task.Yield();
            }

            // load game info
            // get guid first, also is the path of gameinfo file
            var guidJson = nameof(IGameInfo).GetString();
            if (string.IsNullOrEmpty(guidJson))
                guids.Add(GetUniqueGUID());
            else
                guids = JsonConvert.DeserializeObject<List<string>>(guidJson);
            return this;
        }

        /// <summary>
        /// 是否存在存档
        /// </summary>
        /// <returns></returns>
        public bool IsEmptyInfo() => systemInfos.Count == 0 && guids.Count == 1;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetUniqueGUID() {
            string guid = default;
            while (string.IsNullOrEmpty(guid) || guids.Contains(guid))
                guid = Guid.NewGuid().ToString();
            return guid;
        }

        public T ReadSystemInfo<T>() where T : class, ISystemInfo, new() {
            var key = typeof(T).Name;
            if (!systemInfos.TryGetValue(key, out ISystemInfo value)) {
                systemInfos.Add(key, value = new T());
            }
            return value as T;
        }

        /// <summary>
        /// 读取存档，如果不存在就 new 一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T ReadGameInfo<T>() where T : class, IGameInfo, new() {
            gameInfo ??= new T() { GUID = GetUniqueGUID() };
            return gameInfo as T;
        }

        /// <summary>
        /// 读取全部存档，异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T[]> ReadGameInfos<T>() where T : IGameInfo, new() {
            List<T> res = new();
            for (int i = 0; i < guids.Count; i++) {
                var path = Path.Combine(localPath, guids[i]);
                // check is exist
                if (!File.Exists(path)) {
                    if (i != 0)
                        guids.Remove(guids[i--]);
                    continue;
                }
                // load file
                var json = await File.ReadAllTextAsync(path);
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var value = new T() { GUID = guids[i], IsAutoSave = i == 0, LastSaveTime = dic[nameof(IGameInfo.LastSaveTime)].ToInt64() };
                value.SetSaveDic(dic);
                res.Add(value);
            }
            return res.ToArray();
        }

        /// <summary>
        /// 选择存档
        /// </summary>
        /// <param name="info"></param>
        public void SetGameInfo(IGameInfo info) {
            gameInfo = info;
            gameInfo.LastUseTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// 保存系统存档
        /// </summary>
        /// <param name="info"></param>
        public void Save(ISystemInfo info) {
            var json = JsonConvert.SerializeObject(info.GetSaveDic());
            info.GetType().Name.Save(json);
        }

        /// <summary>
        /// 保存游戏存档
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async void Save(IGameInfo info, bool isAuto) {
            string guid = info.GUID;
            if (isAuto)
                guid = guids[0];
            else
                if (string.IsNullOrEmpty(guid))
                    guid = info.GUID = GetUniqueGUID();

            if (!guids.Contains(guid)) {
                guids.Add(guid);
                nameof(IGameInfo).Save(JsonConvert.SerializeObject(guids));
            }

            var dic = info.GetSaveDic();
            dic.Add(nameof(IGameInfo.LastSaveTime), DateTime.Now.Ticks.ToString());
            var json = JsonConvert.SerializeObject(dic, Formatting.Indented);
            using var stream = File.Open(Path.Combine(localPath, guid), FileMode.OpenOrCreate);
            await stream.WriteAsync(Encoding.UTF8.GetBytes(json));
            stream.Close();
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        /// <param name="info"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async void Delete<T>(T info) where T : IInfo {
            if (typeof(T) is ISystemInfo) {
                var key = info.GetType().Name;
                PlayerPrefs.DeleteKey(key);
                await Task.Yield();
                systemInfos.Remove(key);
            } else {
                var guid = (info as IGameInfo).GUID;
                if (!guids.Contains(guid))
                    return;

                var path = Path.Combine(localPath, guid);
                File.Delete(path);
                while (File.Exists(path))
                    await Task.Yield();
                guids.Remove(guid);
                nameof(IGameInfo).Save(JsonConvert.SerializeObject(guids));
            }
        }
    }
}