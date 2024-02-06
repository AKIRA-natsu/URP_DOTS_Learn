using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using AKIRA.Security;

// ------------------------------------
// ##param     param1 param2 param3
// ##type       float  int   string
// ##summary    注释1  注释2   注释3
// ##          注释1.1
// ##          注释1.1
//               1.1    1      "1"
// ------------------------------------

/// <summary>
/// Excel
/// </summary>
public static class ExcelHelp {
    private const string ParamName = "##param";
    private const string TypeName = "##type";
    private const string SummaryName = "##summary";

    /// <summary>
    /// 读取Excel
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="columnNum"></param>
    /// <param name="rowNum"></param>
    /// <returns></returns>
    private static DataTableCollection ReadExcel(this string filePath) {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var table = reader.AsDataSet().Tables;
                reader.Dispose();
                stream.Dispose();
                return table;
            }
        }
    }

    /// <summary>
    /// 创建CS文件
    /// </summary>
    /// <param name="excel"></param>
    /// <param name="cs"></param>
    /// <param name="sheetIndex"></param>
    public static void CreateExcelDataScript(string excel, string cs, int sheetIndex = 0) {
        var table = excel.ReadExcel();
        DataTable sheet;
        try {
            sheet = table[sheetIndex];
        } catch {
            $"{excel} 读取错误".Error();
            return;
        }

        // 没有数据
        if (sheet.Rows.Count <= 1) {
            $"{excel} 不存在数据".Error();
            return;
        }

        // 判断是否存在文件夹
        var directoryName = Path.GetDirectoryName(cs);
        if (!Directory.Exists(directoryName)) {
            Directory.CreateDirectory(directoryName);
        }

#region cs
        var scriptType = Path.GetFileNameWithoutExtension(cs);
        var content = 
@$"/// <summary>
/// ------------------------------------------------------
/// <para>Auto Generate By {typeof(ExcelHelp)}</para>
/// <para>At {DateTime.Now}</para>
/// ------------------------------------------------------
/// </summary>
[System.Serializable]
public partial class {scriptType} {{";

        // params
        List<string> @types = new();
        List<string> @params = new();
        List<string> @summaries = new();
        for (int i = 0; i < sheet.Rows.Count; i++) {
            // check first column
            var header = sheet.Rows[i][0].ToString();
            if (string.IsNullOrEmpty(header))
                continue;
            if (!header.Contains("##"))
                continue;
            if (header.ToLower().Equals(ParamName)) {
                for (int j = 1; j < sheet.Columns.Count; j++) {
                    var @param = sheet.Rows[i][j].ToString();
                    if (string.IsNullOrEmpty(@param))
                        break;
                    // replace xx-oo to xx_oo
                    @params.Add(@param.Replace("-", "_"));
                }
            }

            if (header.ToLower().Equals(TypeName)) {
                for (int j = 1; j < sheet.Columns.Count; j++) {
                    var @type = sheet.Rows[i][j].ToString();
                    if (string.IsNullOrEmpty(@type))
                        break;
                    @types.Add(@type);
                }
            }

            if (header.ToLower().Equals(SummaryName)) {
                for (int j = 1; j < sheet.Columns.Count; j++)
                    @summaries.Add(sheet.Rows[i][j].ToString());
            }
        }
        
        if (@types.Count != @params.Count) {
            $"字段与类型数量不一致".Error();
            return;
        }

        for (int i = 0; i < @types.Count; i++) {
            content += @$"
        /// <summary>
        /// {@summaries.ElementAtOrDefault(i)}
        /// </summary>
        public {@types[i]} {@params[i]};";
        }

        content += @$"
}}
";

        File.WriteAllText(cs, content);
#endregion
    }

    /// <summary>
    /// 创建Json文件
    /// </summary>
    /// <param name="excel"></param>
    /// <param name="json"></param>
    /// <param name="sheetIndex"></param>
    public static void CreateExcelJsonScript(string excel, string json, int sheetIndex = 0) {
        var text = GetExcelJsonData(excel, json, sheetIndex);
        if (string.IsNullOrEmpty(text))
            return;
        File.WriteAllText(json, text);
    }

    /// <summary>
    /// 创建Bytes二进制文件 
    /// </summary>
    /// <param name="excel"></param>
    /// <param name="byte"></param>
    /// <param name="key"></param>
    /// <param name="lv"></param>
    /// <param name="sheetIndex"></param>
    public static void CreateExcelByteScript(string excel, string @byte, string key, string lv, int sheetIndex = 0) {
        var text = GetExcelJsonData(excel, @byte, sheetIndex);
        if (string.IsNullOrEmpty(text))
            return;
        using var memory = new MemoryStream();
        var formatter = new BinaryFormatter();
        formatter.Serialize(memory, text);
        var bytes = SecurityTool.Encrypt(memory.ToArray(), key, lv);
        using var file = new FileStream(@byte, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        file.Write(bytes);
    }

    /// <summary>
    /// 获得Json文件
    /// </summary>
    /// <param name="excel"></param>
    /// <param name="json"></param>
    /// <param name="sheetIndex"></param>
    /// <returns></returns>
    public static string GetExcelJsonData(string excel, string json, int sheetIndex = 0) {
        var table = excel.ReadExcel();
        DataTable sheet;
        try {
            sheet = table[sheetIndex];
        } catch {
            $"{excel} 读取错误".Error();
            return default;
        }

        // 没有数据
        if (sheet.Rows.Count <= 1) {
            $"{excel} 不存在数据".Error();
            return default;
        }

        // 判断是否存在文件夹
        var directoryName = Path.GetDirectoryName(json);
        if (!Directory.Exists(directoryName)) {
            Directory.CreateDirectory(directoryName);
        }

        var scriptType = Path.GetFileNameWithoutExtension(json).GetConfigTypeByAssembley();
        var fields = scriptType.GetFields();
#region Json
        List<object> instances = new();
        DataRow row = default;
        for (int i = 0; i < sheet.Rows.Count; i++) {
            // check first column
            var header = sheet.Rows[i][0].ToString();
            // find param row in rows
            if (header.Equals(ParamName))
                row = sheet.Rows[i];
            if (!string.IsNullOrEmpty(header))
                continue;
            var data = ReflectionHelp.CreateInstance(scriptType);
            for (int j = 1; j < sheet.Columns.Count; j++) {
                var fieldName = row[j].ToString().Replace("-", "_");
                // 长度不太对劲，需要更多测试
                if (string.IsNullOrEmpty(fieldName)) continue;
                // 防止分布类多出字段导致值塞入错误，按照字段排名称查找字段
                var field = fields.Single(field => field.Name.Equals(fieldName));
                field.SetValue(data, sheet.Rows[i][j].ConvertTo(field.FieldType));
            }
            instances.Add(data);
        }

#endregion
        return JsonConvert.SerializeObject(instances, Formatting.Indented);
    }

    // /// <summary>
    // /// XML转Struct
    // /// </summary>
    // /// <param name="table"></param>
    // /// <param name="sheetIndex"></param>
    // /// <typeparam name="T"></typeparam>
    // /// <returns></returns>
    // private static List<T> ConvertToStruct<T>(this DataTableCollection table, int sheetIndex = 0) where T : new() {
    //     DataTable sheet;
    //     try {
    //         sheet = table[sheetIndex];
    //     } catch {
    //         return null;
    //     }

    //     // 没有数据
    //     if (sheet.Rows.Count <= 1)
    //         return null;

    //     // 返回数组
    //     List<T> result = new List<T>();
    //     // 获得字段
    //     List<string> fieldNames = new List<string>();
    //     for (int i = 0; i < sheet.Columns.Count; i++) {
    //         fieldNames.Add(sheet.Rows[0][i].ToString().ToLower());
    //     }

    //     for (int i = 1; i < sheet.Rows.Count; i++) {
    //         T data = new T();
    //         for (int j = 0; j < sheet.Columns.Count; j++) {
    //             var field = data.GetType().GetField(fieldNames[j]);
    //             // Be aware that __makeref is an undocumented keyword. It could as well not work on future versions of C#.
    //             TypedReference reference = __makeref(data);
    //             field.SetValueDirect(reference, sheet.Rows[i][j].ConvertTo(field.FieldType));
    //         }
    //         result.Add(data);
    //     }
    //     return result;
    // }
}