using System;
using UnityEngine;

/// <summary>
/// 只读
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ReadOnly : PropertyAttribute {
    public bool readOnly { get; protected set; }

    public ReadOnly(bool readOnly = true) => this.readOnly = readOnly;
}