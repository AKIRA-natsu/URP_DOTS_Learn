using System;
using UnityEngine;

/// <summary>
/// 只读
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class CNNameAttribute : ReadOnly {
    public string name { get; private set; }

    public CNNameAttribute(string name, bool readOnly = false) {
        this.name = name;
        this.readOnly = readOnly;
    }
}