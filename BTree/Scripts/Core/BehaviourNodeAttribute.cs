using System;

namespace BTree.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BehaviourNodeAttribute : Attribute
    {
#if UNITY_EDITOR
        public readonly string Name, Category;

        public BehaviourNodeAttribute(string category, string name)
        {
            Name = name;
            Category = category;
        }
#else
        public BehaviourNodeAttribute(param object[] params)
        { }
#endif
    }
}
