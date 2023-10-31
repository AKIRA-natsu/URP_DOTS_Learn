using UnityEngine;

/// <summary>
/// 在屏幕边缘指示怪物/敌人当前所处的方位
/// 参考链接：https://blog.csdn.net/xiao_shixiong/article/details/125725344?spm=1001.2101.3001.6650.3&utm_medium=distribute.pc_relevant.none-task-blog-2%7Edefault%7EESLANDING%7Edefault-3-125725344-blog-98366918.235%5Ev27%5Epc_relevant_landingrelevant&depth_1-utm_source=distribute.pc_relevant.none-task-blog-2%7Edefault%7EESLANDING%7Edefault-3-125725344-blog-98366918.235%5Ev27%5Epc_relevant_landingrelevant&utm_relevant_index=6
/// </summary>
public static class OutScreenMark {
    /// <summary>
    /// 屏幕左下角
    /// </summary>
    public static Vector2 ScreenLeftBottom = new Vector2(0, 0);
    /// <summary>
    /// 屏幕右下角
    /// </summary>
    public static Vector2 ScreenRightBottom = new Vector2(Screen.width, 0);
    /// <summary>
    /// 屏幕左上角
    /// </summary>
    public static Vector2 ScreenLeftTop = new Vector2(0, Screen.height);
    /// <summary>
    /// 屏幕右上角
    /// </summary>
    public static Vector2 ScreenRightTop = new Vector2(Screen.width, Screen.height);
    /// <summary>
    /// 屏幕中心
    /// </summary>
    public static Vector2 ScreenCenter = new Vector2(Screen.width, Screen.height) / 2;

    private static float widthOffset;
    /// <summary>
    /// 宽度偏移量
    /// </summary>
    public static float WidthOffset {
        get => widthOffset;
        set {
            widthOffset = value;
            ResetScreenSize();
        }
    }

    private static float heightOffset;
    /// <summary>
    /// 高度偏移量
    /// </summary>
    /// <value></value>
    public static float HeightOffset {
        get => heightOffset;
        set {
            heightOffset = value;
            ResetScreenSize();
        }
    }

    /// <summary>
    /// 重置屏幕范围
    /// </summary>
    private static void ResetScreenSize() {
        ScreenLeftBottom = new Vector2(WidthOffset, HeightOffset);
        ScreenRightBottom = new Vector2(Screen.width - WidthOffset, HeightOffset);
        ScreenLeftTop = new Vector2(WidthOffset, Screen.height - HeightOffset);
        ScreenRightTop = new Vector2(Screen.width - WidthOffset, Screen.height - HeightOffset);
    }

    /// <summary>
    /// 是否在屏幕内
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    private static bool InScreen(this Vector3 screenPosition) {
        return screenPosition.x >= ScreenLeftBottom.x &&
            screenPosition.x <= ScreenRightTop.x &&
            screenPosition.y >= ScreenLeftBottom.y &&
            screenPosition.y <= ScreenRightTop.y;
    }

    /// <summary>
    /// 尝试获得交点
    /// </summary>
    /// <param name="isCross"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static (bool isCross, Vector3 crossPoint) TryGetIntersectPoint(this Vector3 position) {
        var screenPosition = Camera.main.WorldToScreenPointProjected(position);
        if (InScreen(screenPosition))
            return default;

        var isCross = OnLinearAlgebra(ScreenCenter, screenPosition, out Vector3 crossPosition);
        return (isCross, crossPosition);
    }

    // /// <summary>
    // /// 尝试获得交点
    // /// </summary>
    // /// <param name="isCross"></param>
    // /// <param name="a"></param>
    // /// <param name="b"></param>
    // /// <param name="c"></param>
    // /// <param name="d"></param>
    // /// <returns></returns>
    // public static (bool isCross, Unity.Mathematics.float3 crossPoint) TryGetIntersectPoint(this Unity.Mathematics.float3 position) {
    //     var screenPosition = CameraExtend.MainCamera.WorldToScreenPoint(position);
    //     if (InScreen(screenPosition))
    //         return default;

    //     var isCross = OnLinearAlgebra(ScreenCenter, screenPosition, out Vector3 crossPosition);
    //     return (isCross, crossPosition);
    // }

    /// <summary>
    /// 世界坐标转屏幕坐标（Unity自带的转换可能存在一点问题）
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static Vector2 WorldToScreenPointProjected(this Camera camera, Vector3 worldPos) {
        Vector3 camNormal = camera.transform.forward;
        Vector3 vectorFromCam = worldPos - camera.transform.position;
        float camNormDot = Vector3.Dot( camNormal, vectorFromCam );
        if ( camNormDot <= 0 )
        {
            // we are behind the camera forward facing plane, project the position in front of the plane
            Vector3 proj = ( camNormal * camNormDot * 1.01f );
            worldPos = camera.transform.position + ( vectorFromCam - proj );
        }
 
        return RectTransformUtility.WorldToScreenPoint( camera, worldPos );
    }

    /// <summary>
    /// 拿到交点
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="pos3"></param>
    /// <param name="pos4"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    [System.Obsolete("有点问题, 可能闪烁", true)]
    private static bool GetPoint(Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4, out Vector3 target)
    {
        target = default;

        float a = pos2.y - pos1.y;
        float b = pos1.x - pos2.x;
        float c = pos2.x * pos1.y - pos1.x * pos2.y;
        float d = pos4.y - pos3.y;
        float e = pos3.x - pos4.x;
        float f = pos4.x * pos3.y - pos3.x * pos4.y;
        float x = (f * b - c * e) / (a * e - d * b);
        float y = (c * d - f * a) / (a * e - d * b);
        // if (a * e == d * b)  //A1*B2==A2*B1  平行
        //     return;
        if (x < 0 || y < 0 || x > Screen.width || y > Screen.height) return false;
        if (!GetOnLine(pos1, pos2, new Vector2(x, y))) return false;
        target = new Vector3(x, y);
        return true;
    }

    /// <summary>
    /// 求交点
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <param name="IntrPos"></param>
    /// <returns></returns>
    public static bool SegmentsInterPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 IntrPos)
    {
        IntrPos = default;
 
        //以线段ab为准，是否c，d在同一侧
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        float abXac = Cross(ab, ac);
 
        Vector3 ad = d - a;
        float abXad = Cross(ab, ad);
 
        if (abXac * abXad >= 0)
        {
            return false;
        }
 
        //以线段cd为准，是否ab在同一侧
        Vector3 cd = d - c;
        Vector3 ca = a - c;
        Vector3 cb = b - c;
 
        float cdXca = Cross(cd, ca);
        float cdXcb = Cross(cd, cb);
        if (cdXca * cdXcb >= 0)
        {
            return false;
        }
        //计算交点坐标  
        float t = Cross(a - c, d - c) / Cross(d - c, b - a);
        float dx = t * (b.x - a.x);
        float dy = t * (b.y - a.y);
 
        IntrPos = new Vector3() { x = a.x + dx, y = a.y + dy };
        return true;
    }

    /// <summary>
    /// 叉乘
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Cross(Vector3 a, Vector3 b)
    {
        return a.x * b.y - b.x * a.y;
    }

    /// <summary>
    /// 线代解
    /// </summary>
    private static bool OnLinearAlgebra(Vector3 pos1, Vector3 pos2, out Vector3 target)
    {
        target = default;
        //AB线段的abc   //a=y2-y1  b=x1-x2  c=x2y1-x1y2
        if (SegmentsInterPoint(pos1, pos2, ScreenLeftBottom, ScreenLeftTop, out target)) return true;
        else if (SegmentsInterPoint(pos1, pos2, ScreenRightBottom, ScreenRightTop, out target)) return true;
        else if (SegmentsInterPoint(pos1, pos2, ScreenLeftTop, ScreenRightTop, out target)) return true;
        else if (SegmentsInterPoint(pos1, pos2, ScreenLeftBottom, ScreenRightBottom, out target)) return true;
        return false;

    }
  
    /// <summary>
    /// 判断在哪根线
    /// </summary>
    /// <param name="pos1">a</param>
    /// <param name="pos2">b</param>
    /// <param name="pos3">p</param>
    private static bool GetOnLine(Vector2 pos1, Vector2 pos2, Vector2 pos3)
    {
        float EPS = 1e-3f; //误差
        float d1 = Vector2.Distance(pos1, pos3);
        float d2 = Vector2.Distance(pos2, pos3);
        float d3 = Vector2.Distance(pos1, pos2);
        if (Mathf.Abs(d1 + d2 - d3) <= EPS)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 这个方法是让箭头指向处于屏幕中间的玩家坐标与箭头坐标向量的方向
    /// </summary>
    public static void UILookAt(Transform ctrlObj, Vector3 dir, Vector3 lookAxis) {
        Quaternion quaternion = Quaternion.identity;
        quaternion.SetFromToRotation(lookAxis, dir);
        ctrlObj.eulerAngles = Vector3.Lerp(ctrlObj.eulerAngles, new Vector3(quaternion.eulerAngles.x, 0, quaternion.eulerAngles.z), Time.deltaTime * 10f);
        // ctrlObj.eulerAngles = new Vector3(quaternion.eulerAngles.x, 0, quaternion.eulerAngles.z);
    }
}