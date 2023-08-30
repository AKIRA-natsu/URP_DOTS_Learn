using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;

/// <summary>
/// Excel
/// </summary>
public static class ExcelHelp {
    /// <summary>
    /// Excel读取并转换 <paramref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static List<T> ExcelConvertToClass<T>(this string path, string dllName, int sheetIndex = 0) where T : class {
        return ReadExcel(path).ConvertToClass<T>(dllName, sheetIndex);
    }

    /// <summary>
    /// Excel读取并转换 <paramref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static List<T> ExcelConvertToStruct<T>(this string path, int sheetIndex = 0) where T : new() {
        return ReadExcel(path).ConvertToStruct<T>(sheetIndex);
    }

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
    /// XML转Struct
    /// </summary>
    /// <param name="table"></param>
    /// <param name="sheetIndex"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static List<T> ConvertToStruct<T>(this DataTableCollection table, int sheetIndex = 0) where T : new() {
        DataTable sheet;
        try {
            sheet = table[sheetIndex];
        } catch {
            return null;
        }

        // 没有数据
        if (sheet.Rows.Count <= 1)
            return null;

        // 返回数组
        List<T> result = new List<T>();
        // 获得字段
        List<string> fieldNames = new List<string>();
        for (int i = 0; i < sheet.Columns.Count; i++) {
            fieldNames.Add(sheet.Rows[0][i].ToString().ToLower());
        }

        for (int i = 1; i < sheet.Rows.Count; i++) {
            T data = new T();
            for (int j = 0; j < sheet.Columns.Count; j++) {
                var field = data.GetType().GetField(fieldNames[j]);
                // Be aware that __makeref is an undocumented keyword. It could as well not work on future versions of C#.
                TypedReference reference = __makeref(data);
                field.SetValueDirect(reference, sheet.Rows[i][j].ConvertTo(field.FieldType));
            }
            result.Add(data);
        }
        return result;
    }

    /// <summary>
    /// XML转Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static List<T> ConvertToClass<T>(this DataTableCollection table, string dllName, int sheetIndex = 0) where T : class {
        DataTable sheet;
        try {
            sheet = table[sheetIndex];
        } catch {
            return null;
        }

        // 没有数据
        if (sheet.Rows.Count <= 1)
            return null;

        // 返回数组
        List<T> result = new List<T>();
        // 获得字段
        List<string> fieldNames = new List<string>();
        for (int i = 0; i < sheet.Columns.Count; i++) {
            fieldNames.Add(sheet.Rows[0][i].ToString());
        }
        
        for (int i = 1; i < sheet.Rows.Count; i++) {
            var data = typeof(T).CreateInstance<T>(dllName);
            for (int j = 0; j < sheet.Columns.Count; j++) {
                var field = data.GetType().GetField(fieldNames[j]);
                field.SetValue(data, sheet.Rows[i][j].ConvertTo(field.FieldType));
            }
            result.Add(data);
        }
        return result;
    }
}