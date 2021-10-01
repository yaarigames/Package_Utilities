using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using EditorUtility = SAS.Utilities.Editor.EditorUtility;

namespace SAS.TagSystem.Editor
{
    [CustomEditor(typeof(Tagger), true)]
    public class TaggerEditor : UnityEditor.Editor
    {
        private ReorderableList _componentTagList;
        private string[] Tags => TagList.GetList();
        private static TagList Instance => TagList.Instance();

        private void OnEnable()
        {
            _componentTagList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Tags"), true, true, false, true);
            _componentTagList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Tagged Component List");
            };

            _componentTagList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var componentTag = serializedObject.FindProperty("m_Tags");
                if (componentTag.arraySize > 0)
                {
                    var component = componentTag.GetArrayElementAtIndex(index).FindPropertyRelative("m_Component");
                    var tag = componentTag.GetArrayElementAtIndex(index).FindPropertyRelative("m_Value");
                    var oldValue = tag.stringValue;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.ObjectField(new Rect(rect.x + 5, rect.y, rect.width / 2, rect.height), component.objectReferenceValue, typeof(Component), false);

                    EditorGUI.EndDisabledGroup();

                    Rect pos = new Rect(rect.width / 2 + 60, rect.y, rect.width / 2 - 20, rect.height);
                    int id = GUIUtility.GetControlID("SearchableStringDrawer".GetHashCode(), FocusType.Keyboard, pos);

                    EditorUtility.DropDown(id, pos, Tags, Array.IndexOf(Tags, tag.stringValue), selectedIndex => OnTagSelected(index, selectedIndex), ShowTagList);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            _componentTagList.DoLayoutList();
        }

        private void OnTagSelected(int componentIndex, int index)
        {
            var tagList = serializedObject.FindProperty("m_Tags");
            var tagProperty = tagList.GetArrayElementAtIndex(componentIndex);
            tagProperty.FindPropertyRelative("m_Value").stringValue = index != -1 ? Tags[index] : string.Empty;
            serializedObject.ApplyModifiedProperties();
            UnityEditor.EditorUtility.SetDirty(target);
        }

        private void ShowTagList()
        {
            Selection.activeObject = Instance;
        }
    }
}
