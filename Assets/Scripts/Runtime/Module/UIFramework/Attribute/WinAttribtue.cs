using System;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class WinAttribute : System.Attribute {
        public WinData Data { get; private set; }

        public WinAttribute(WinEnum @enum, string path, WinType @type) => 
            this.Data = new WinData(@enum, path, @type);
    }
}
