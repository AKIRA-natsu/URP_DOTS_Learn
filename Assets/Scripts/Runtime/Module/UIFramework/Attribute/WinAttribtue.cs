using System;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class WinAttribute : System.Attribute {
        public WinData Data { get; private set; }

        public WinAttribute(WinEnum self, WinEnum parent, string path, WinType @type) => 
            this.Data = new(self, parent, path, @type);
        
        public WinAttribute(WinEnum self, string path, WinType @type) =>
            this.Data = new(self, WinEnum.None, path, @type);
    }
}
