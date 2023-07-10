using System.Linq;
using AKIRA.Attribute;
using UnityEngine;

namespace AKIRA.Manager {
    [CreateAssetMenu(fileName = "LogConfig", menuName = "Framework/LogConfig", order = 0)]
    public class LogConfig : ScriptableObject {
        /// <summary>
        /// 日志数据
        /// </summary>
        [System.Serializable]
        private class LogData {
            [SelectionPop(typeof(GameData.Log))]
            public string name;
            public Color color = Color.white;
            public bool logable = true;
        }

        /// <summary>
        /// 详细日志
        /// </summary>
        public bool logfully = true;

        /// <summary>
        /// 数据
        /// </summary>
        [SerializeField]
        private LogData[] datas;

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="key"></param>
        public (Color color, bool logable) GetData(string key) {
            var data = datas.SingleOrDefault(data => data.name.Equals(key));
            return data == null ? (Color.white, true) : (data.color, data.logable);
        }
    }
}