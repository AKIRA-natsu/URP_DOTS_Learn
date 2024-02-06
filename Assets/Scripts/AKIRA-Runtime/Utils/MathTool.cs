using System;
using UnityEngine;

/// <summary>
/// 数学方法
/// </summary>
public static class MathTool {
    /// <summary>
    /// <para>Lerp</para>
    /// <para>来源：https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/</para>
    /// <para>TODO: 测试</para>
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="lambda"></param>
    public static float Damp(float value1, float value2, float lambda) {
        return Mathf.Lerp(value1, value2, 1 - Mathf.Exp(-lambda * Time.deltaTime));
    }

    /// <summary>
    /// <para>Lerp</para>
    /// <para>来源：https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/</para>
    /// <para>TODO: 测试</para>
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="lambda"></param>
    /// <param name="time"></param>
    public static Vector3 Damp(Vector3 value1, Vector3 value2, float lambda) {
        return Vector3.Lerp(value1, value2, 1 - Mathf.Exp(-lambda * Time.deltaTime));
    }

    /// <summary>
    /// 二次贝塞尔曲线
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="t">[0, 1]</param>
    /// <returns></returns>
    public static Vector3 GetBezier(Vector3 p0, Vector3 p2, Vector3 p3, float t) {
        return (1 - t) * (1 - t) * p0 + 2 * t * (1 - t) * p2 + t * t * p3;
    }

    /// <summary>
    /// 2进制
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ToBinary(this int value) {
        return Convert.ToInt64(Convert.ToString(value, 2));
    }

    /// <summary>
    /// 8进制
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ToOctal(this int value) {
        return Convert.ToInt64(Convert.ToString(value, 8));
    }

    /// <summary>
    /// 10进制
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ToDecimal(this int value) {
        return Convert.ToInt32(Convert.ToString(value, 10));
    }

    /// <summary>
    /// 16进制
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToHexadecimal(this int value) {
        return Convert.ToString(value, 16);
    }

    // 常用转换添加
    public static short ToInt16(this object value) => Convert.ToInt16(value);
    public static int ToInt32(this object value) => Convert.ToInt32(value);
    public static long ToInt64(this object value) => Convert.ToInt64(value);
    public static float ToSingle(this object value) => Convert.ToSingle(value);
    public static bool ToBoolean(this object value) => Convert.ToBoolean(value);
}