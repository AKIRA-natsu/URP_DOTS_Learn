using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AKIRA.Behaviour.Camera;
using System.Linq;

public class TestLaunch : MonoBehaviour
{
    public CameraFollowAuthoring following;
    public CameraFollowAuthoring[] intervals;

    public float intervalInvokeTime = 1f;

    private void Awake() {
        CameraSystem.Instance.Regist();
    }

    private IEnumerator Start() {
        var delay = new WaitForSeconds(intervalInvokeTime);

        yield return delay;
        following.enabled = true;

        for (int i = 0; i < intervals.Length; i++) {
            yield return delay;
            intervals[i].enabled = true;
        }
    }
}
