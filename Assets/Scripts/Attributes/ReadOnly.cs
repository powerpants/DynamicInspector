using System;

namespace DynamicInspector.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnly : Attribute
    {
    }
}