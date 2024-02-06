using System;

namespace AKIRA.UIFramework {
    /// <summary>
    /// UI Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class WinAttribute : System.Attribute {
        public WinData Data { get; private set; }

        public WinAttribute(WinEnum self, WinEnum parent, string path, WinType @type) =>
            // 做一个顺序判断，正常子物体肯定在父物体后，如果顺序不对还是设置成 WinEnum.None 
            this.Data = new(self, parent > self ? WinEnum.None : parent, path, @type);
        
        public WinAttribute(WinEnum self, string path, WinType @type) =>
            this.Data = new(self, WinEnum.None, path, @type);
    }
}
