using System;

namespace DynamicInspector.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CustomInspectorGUI : Attribute
    {
    }
}