using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;
using System.Linq;
using UnityEditor;
using Newtonsoft.Json;

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
                    @params.Add(@param);
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
        AssetDatabase.Refresh();
#endregion
    }

    /// <summary>
    /// 创建Json文件
    /// </summary>
    /// <param name="excel"></param>
    /// <param name="json"></param>
    /// <param name="sheetIndex"></param>
    public static void CreateExcelJsonScript(string excel, string json, int sheetIndex = 0) {
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
        var directoryName = Path.GetDirectoryName(json);
        if (!Directory.Exists(directoryName)) {
            Directory.CreateDirectory(directoryName);
        }

        var scriptType = Path.GetFileNameWithoutExtension(json).GetConfigTypeByAssembley();
        var fields = scriptType.GetFields();
#region Json
        List<object> instances = new();
        for (int i = 0; i < sheet.Rows.Count; i++) {
            // check first column
            var header = sheet.Rows[i][0].ToString();
            if (!string.IsNullOrEmpty(header))
                continue;
            var data = ReflectionHelp.CreateInstance(scriptType);
            for (int j = 1; j < fields.Length + 1; j++) {
                var field = fields[j - 1];
                field.SetValue(data, sheet.Rows[i][j].ConvertTo(field.FieldType));
            }
            instances.Add(data);
        }

        File.WriteAllText(json, JsonConvert.SerializeObject(instances, Formatting.Indented));
        AssetDatabase.Refresh();
#endregion
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

    // /// <summary>
    // /// XML转Class
    // /// </summary>
    // /// <typeparam name="T"></typeparam>
    // private static List<T> ConvertToClass<T>(this DataTableCollection table, string dllName, int sheetIndex = 0) where T : class {
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
    //         fieldNames.Add(sheet.Rows[0][i].ToString());
    //     }
        
    //     for (int i = 1; i < sheet.Rows.Count; i++) {
    //         var data = typeof(T).CreateInstance<T>(dllName);
    //         for (int j = 0; j < sheet.Columns.Count; j++) {
    //             var field = data.GetType().GetField(fieldNames[j]);
    //             field.SetValue(data, sheet.Rows[i][j].ConvertTo(field.FieldType));
    //         }
    //         result.Add(data);
    //     }
    //     return result;
    // }
}