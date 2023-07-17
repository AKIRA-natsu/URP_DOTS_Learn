using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Jobs;

public class PoolObject : MonoBehaviour
{
    static readonly ProfilerMarker<int> profilerMarker = new ProfilerMarker<int>();

    private TransformAccessArray transformAccessArray;
}
