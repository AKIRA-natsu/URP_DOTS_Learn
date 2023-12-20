using UnityEngine;

/// <summary>
/// 跳跃枚举
/// </summary>
public enum NextPlayerMovement {
    /// <summary>
    /// 正常跳跃
    /// </summary>
    Jump,
    /// <summary>
    /// 翻越
    /// </summary>
    ClimbLow,
    /// <summary>
    /// 攀爬
    /// </summary>
    ClimbHeight,
    /// <summary>
    /// 
    /// </summary>
    vault,
}

/// <summary>
/// 移动环境检测表现
/// </summary>
public class MoveEnvironmentBehaviour : MonoBehaviour {
    /// <summary>
    /// 跳跃方式
    /// </summary>
    [ReadOnly]
    [SerializeField]
    private NextPlayerMovement nextMovement = NextPlayerMovement.Jump;

    /// <summary>
    /// 翻越高度检测
    /// </summary>
    [SerializeField, Min(0f)]
    private float lowClimbHeight = 0.5f;
    /// <summary>
    /// 前方检测
    /// </summary>
    [SerializeField, Min(0.1f)]
    private float checkDistance = 1f;
    /// <summary>
    /// 攀爬角度
    /// </summary>
    private float climbAngle;
    /// <summary>
    /// 攀爬向量
    /// </summary>
    private Vector3 climbHitNormal;
    /// <summary>
    /// 攀爬距离
    /// </summary>
    private float climbDistance;
    /// <summary>
    /// 身体高度
    /// </summary>
    [SerializeField, Min(0.1f)]
    private float bodyHeight = 1f;
    /// <summary>
    /// 攀爬的最高高度
    /// /// </summary>
    [SerializeField]
    private float maxClimbHeight = 3.5f;

    /// <summary>
    /// 射线检测数量
    /// </summary>
    [SerializeField, Min(1)]
    private int rayCount = 1;
    /// <summary>
    /// 高度差值
    /// </summary>
    private float offset;

    /// <summary>
    /// 顶部边缘信息
    /// </summary>
    private Vector3 ledge;

    private void Awake() {
        climbDistance = Mathf.Cos(climbAngle) * checkDistance;
        offset = (maxClimbHeight - bodyHeight) / rayCount;
    }

    private void Update() {
        ClimbDirect(this.transform.forward).Log();
    }

    /// <summary>
    /// 获得跳跃的动画方式
    /// </summary>
    /// <param name="inputDirection"></param>
    /// <returns></returns>
    public NextPlayerMovement ClimbDirect(Vector3 inputDirection) {
        if (Physics.Raycast(this.transform.position + Vector3.up * lowClimbHeight, this.transform.forward, out RaycastHit obsHit, checkDistance)) {
            climbHitNormal = obsHit.normal;
            // 不能攀爬的条件
            if (Vector3.Angle(-climbHitNormal, this.transform.forward) > climbAngle ||
                Vector3.Angle(-climbHitNormal, inputDirection) > climbAngle) {
                    return NextPlayerMovement.Jump;
                }
            
            for (int i = 0; i < rayCount; i++) {
                if (Physics.Raycast(this.transform.position + Vector3.up * (lowClimbHeight + offset * i), -climbHitNormal, out RaycastHit wallHit, climbDistance)) {
                    // 循环到了最后，超过最高高度
                    if (i == rayCount - 1)
                        return NextPlayerMovement.Jump;
                } else {
                    // 向下检测攀爬高度
                    if (Physics.Raycast(wallHit.point + Vector3.up * bodyHeight, Vector3.down, out RaycastHit result, bodyHeight)) {
                        ledge = result.point;
                        // TODO: 测试实际判断
                        return i == 0 ? NextPlayerMovement.ClimbLow : NextPlayerMovement.ClimbHeight;
                    }
                }
            }
        }

        return NextPlayerMovement.Jump;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        var start = this.transform.position + Vector3.up * lowClimbHeight;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, this.transform.position + this.transform.forward * checkDistance);
        offset = (maxClimbHeight - bodyHeight) / rayCount;
        Gizmos.color = Color.green;
        for (int i = 0; i < rayCount; i++) {
            Gizmos.DrawLine(start + Vector3.up * i * offset, this.transform.position - climbHitNormal * checkDistance);
        }
    }
    #endif
}