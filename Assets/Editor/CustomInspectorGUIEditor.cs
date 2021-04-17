using System;
using System.Collections.Generic;
using System.Reflection;
using DynamicInspector.Attributes;
using UnityEditor;
using UnityEngine;

namespace DynamicInspector.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public sealed class CustomInspectorGUIEditor : UnityEditor.Editor
    {
        private bool customInspectorGUI;
        private SerializedObject obj;
        private List<string> excludeProperties = new List<string>();
        private Type type;

        private void OnEnable() {
            type = target.GetType();

            var classAttrs = type.GetCustomAttribute(typeof(CustomInspectorGUI), true);
            if (classAttrs is CustomInspectorGUI) customInspectorGUI = true;
            else return;

            obj = new SerializedObject(target);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (var field in fields) {
                var attr = field.GetCustomAttribute(typeof(DynamicHidden), true);
                if (attr == null) continue;
                InitDynamicHiddenAttribute(field, attr as DynamicHidden);
            }
        }

        private Dictionary<string, KeyValuePair<FieldInfo, List<KeyValuePair<FieldInfo, Attribute>>>>
            fieldDict =
                new Dictionary<string,
                    KeyValuePair<FieldInfo, List<KeyValuePair<FieldInfo, Attribute>>>>();

        private void InitDynamicHiddenAttribute(FieldInfo field, DynamicHidden attr) {
            var switchField = type.GetField(attr.fieldName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            if (switchField == null) return;

            if (!fieldDict.ContainsKey(switchField.Name)) {
                excludeProperties.Add(switchField.Name);
                fieldDict.Add(switchField.Name,
                    new KeyValuePair<FieldInfo, List<KeyValuePair<FieldInfo, Attribute>>>(
                        switchField, new List<KeyValuePair<FieldInfo, Attribute>>()));
            }

            excludeProperties.Add(field.Name);
            fieldDict[switchField.Name].Value.Add(new KeyValuePair<FieldInfo, Attribute>(field, attr));
        }

        public override void OnInspectorGUI() {
            if (!customInspectorGUI) {
                base.OnInspectorGUI();
                return;
            }

            DrawPropertiesExcluding(obj, excludeProperties.ToArray());
            ProcessDynamicHiddenAttribute();
        }

        private object GetSwitchValue(FieldInfo field, SerializedProperty sField) {
            object switchValue = default;

            if (field.FieldType == typeof(bool)) {
                switchValue = sField.boolValue;
            } else if (field.FieldType == typeof(string)) {
                switchValue = sField.stringValue;
            } else if (field.FieldType == typeof(int)) {
                switchValue = sField.intValue;
            } else if (field.FieldType == typeof(long)) {
                switchValue = sField.longValue;
            } else if (field.FieldType == typeof(float)) {
                switchValue = sField.floatValue;
            } else if (field.FieldType == typeof(double)) {
                switchValue = sField.doubleValue;
            }

            return switchValue;
        }

        private void ProcessDynamicHiddenAttribute() {
            foreach (var pair in fieldDict) {
                var switchField = pair.Value.Key;
                var sSwitchField = obj.FindProperty(switchField.Name);
                if (sSwitchField == null) continue;
                EditorGUILayout.PropertyField(sSwitchField);

                var switchValue = GetSwitchValue(switchField, sSwitchField);
                var fieldAttrPairsList = pair.Value.Value;

                foreach (var fieldAttrPair in fieldAttrPairsList) {
                    var field = fieldAttrPair.Key;
                    var attr = (DynamicHidden) fieldAttrPair.Value;

                    if (attr.show.Equals(switchValue)) {
                        using (var sField = obj.FindProperty(field.Name)) {
                            if (sField != null) EditorGUILayout.PropertyField(sField, true);
                        }
                    } else if (attr.readOnly) {
                        using (var sField = obj.FindProperty(field.Name)) {
                            if (sField != null) {
                                GUI.enabled = false;
                                EditorGUILayout.PropertyField(sField, true);
                                GUI.enabled = true;
                            }
                        }
                    }
                }
            }
        }
    }
}