using System;

namespace MenuBuilder {
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MenuAttribute : Attribute {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public MenuAttribute(string id) {
            ID = id.ToLower();
        }
    }
}
