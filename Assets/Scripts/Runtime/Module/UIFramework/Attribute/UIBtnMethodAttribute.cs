using System;

namespace AKIRA.UIFramework {
    /// <summary>
    /// Method will show in RectTransform Inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class UIBtnMethodAttribute : System.Attribute {
        public string name { get; private set; }

        /// <summary>
        /// use method name
        /// </summary>
        public UIBtnMethodAttribute() {
            this.name = default;
        }

        /// <summary>
        /// use <see cref="name" /> name
        /// </summary>
        /// <param name="name"></param>
        public UIBtnMethodAttribute(string name) {
            this.name = name;
        }
                
    }
}