using System;
using UnityEditor;
using UnityEngine;

namespace SAS.Utilities.Editor
{
    public static class EditorUtility
    {
        public static void DropDown(int id, Rect position, string[] options, int selectedIndex, Action<int> onSelect)
        {
            int controlID = GUIUtility.GetControlID(id, FocusType.Keyboard, position);
            int result = selectedIndex;

            if (DropdownButton(controlID, position, new GUIContent(selectedIndex ==-1 ? "None" : options[selectedIndex])))
            {
                SearchablePopup.Show(position, options, selectedIndex, onSelect);
            }
        }

        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }
    }
}
