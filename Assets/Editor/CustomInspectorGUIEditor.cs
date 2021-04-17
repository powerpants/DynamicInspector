using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DynamicInspector.Attributes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public sealed class CustomInspectorGUIEditor : UnityEditor.Editor
    {
        private bool customInspectorGUI;
        private SerializedObject obj;
        private HashSet<string> excludeProperties = new HashSet<string>();
        private HashSet<string> readOnlyProperties = new HashSet<string>();
        private Type type;

        private Dictionary<string, KeyValuePair<FieldInfo, List<KeyValuePair<FieldInfo, Attribute>>>>
            fieldDict =
                new Dictionary<string,
                    KeyValuePair<FieldInfo, List<KeyValuePair<FieldInfo, Attribute>>>>();


        private void OnEnable() {
            type = target.GetType();

            var classAttrs = type.GetCustomAttribute(typeof(CustomInspectorGUI), true);
            if (classAttrs is CustomInspectorGUI) customInspectorGUI = true;
            else return;

            obj = new SerializedObject(target);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (var field in fields) {
                var dynamicHiddenAttr = field.GetCustomAttribute(typeof(DynamicHidden), true);
                if (dynamicHiddenAttr == null) {
                    var readOnlyAttr = field.GetCustomAttribute(typeof(ReadOnly), true);
                    if (readOnlyAttr == null) continue;
                    InitReadOnlyAttribute(field);
                } else InitDynamicHiddenAttribute(field, dynamicHiddenAttr as DynamicHidden);
            }
        }

        private void InitReadOnlyAttribute(FieldInfo field) {
            excludeProperties.Add(field.Name);
            readOnlyProperties.Add(field.Name);
        }

        private void InitDynamicHiddenAttribute(FieldInfo field, DynamicHidden attr) {
            string fieldName = attr.fieldName;
            FieldInfo switchField = null;
            if (fieldName != "") {
                switchField = type.GetField(attr.fieldName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                if (switchField == null) return;
            }

            if (!fieldDict.ContainsKey(fieldName)) {
                excludeProperties.Add(fieldName);
                fieldDict.Add(fieldName,
                    new KeyValuePair<FieldInfo, List<KeyValuePair<FieldInfo, Attribute>>>(
                        switchField, new List<KeyValuePair<FieldInfo, Attribute>>()));
            }

            excludeProperties.Add(field.Name);
            fieldDict[fieldName].Value.Add(new KeyValuePair<FieldInfo, Attribute>(field, attr));
        }

        public override void OnInspectorGUI() {
            if (!customInspectorGUI) {
                base.OnInspectorGUI();
                return;
            }

            DrawPropertiesExcluding(obj, excludeProperties.ToArray());
            ProcessDynamicHiddenProperties();
            ProcessReadOnlyProperties();
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

        private void ProcessDynamicHiddenProperties() {
            foreach (var pair in fieldDict) {
                string fieldName = pair.Key;

                object switchValue = null;
                if (fieldName != "") {
                    var switchField = pair.Value.Key;
                    var sSwitchField = obj.FindProperty(switchField.Name);
                    if (sSwitchField == null) continue;
                    EditorGUILayout.PropertyField(sSwitchField);
                    switchValue = GetSwitchValue(switchField, sSwitchField);
                }

                var fieldAttrPairsList = pair.Value.Value;

                foreach (var fieldAttrPair in fieldAttrPairsList) {
                    var field = fieldAttrPair.Key;
                    var attr = (DynamicHidden) fieldAttrPair.Value;

                    var readOnly = field.GetCustomAttribute(typeof(ReadOnly), true) != null;
                    using (var sField = obj.FindProperty(field.Name)) {
                        if (fieldName == "" ||
                            fieldName != "" && switchValue != null && attr.show != null &&
                            attr.show.Equals(switchValue)) {
                            DrawPropertyField(sField, fieldName == "" || attr.readOnly || readOnly);
                        }
                    }
                }
            }
        }

        private void DrawPropertyField(SerializedProperty sField, bool readOnly = false) {
            if (sField == null) return;
            if (readOnly) {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(sField, true);
                GUI.enabled = true;
            } else EditorGUILayout.PropertyField(sField, true);
        }

        private void ProcessReadOnlyProperties() {
            GUI.enabled = false;
            foreach (var readOnlyField in readOnlyProperties) {
                using (var sField = obj.FindProperty(readOnlyField))
                    if (sField != null)
                        EditorGUILayout.PropertyField(sField, true);
            }

            GUI.enabled = true;
        }
    }
}