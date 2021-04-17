using System;

namespace DynamicInspector.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DynamicHidden : Attribute
    {
        public string fieldName;
        public object show;
        public bool readOnly;

        public DynamicHidden(string fieldName, object show, bool readOnly = false) {
            this.fieldName = fieldName;
            this.show = show;
            this.readOnly = readOnly;
        }
    }
}