using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SAS.Utilities.Editor
{
    public class SearchablePopup : PopupWindowContent
    {
        private const float RowHeight = 16.0f;
        private const float RowIndent = 8.0f;
        private const string SearchControlName = "SearchText";
        
        public static void Show(Rect activatorRect, string[] options, int current, Action<int> onSelectionMade)
        {
            SearchablePopup window = new SearchablePopup(options, current, onSelectionMade);
            PopupWindow.Show(activatorRect, window);
        }

        private static void Repaint()
        { 
            EditorWindow.focusedWindow.Repaint(); 
        }
 
        private static void DrawBox(Rect rect, Color tint)
        {
            Color c = GUI.color;
            GUI.color = tint;
            GUI.Box(rect, "", Selection);
            GUI.color = c;
        }
        
        private class FilteredList
        {
            public struct Entry
            {
                public int Index;
                public string Text;
            }
            
            private string[] allItems { get; }

            public FilteredList(string[] items)
            {
                allItems = items;
                Entries = new List<Entry>();
                UpdateFilter("");
            }
         
            public string Filter { get; private set; }
            public List<Entry> Entries { get; private set; }
            public int MaxLength => allItems.Length;

            public bool UpdateFilter(string filter)
            {
                if (Filter == filter)
                    return false;
                
                Filter = filter;
                Entries.Clear();
                
                for (int i = 0; i < allItems.Length; i++)
                {
                    if (string.IsNullOrEmpty(Filter) || allItems[i].ToLower().Contains(Filter.ToLower()))
                    {
                        Entry entry = new Entry
                        {
                            Index = i,
                            Text = allItems[i]
                        };
                        if (string.Equals(allItems[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                            Entries.Insert(0, entry);
                        else
                            Entries.Add(entry);
                    }
                }
                return true;
            }
        }
        
        private readonly Action<int> onSelectionMade;
        private readonly FilteredList list;
        private int CurrentIndex { get; }
        
        private Vector2 scroll;
        private int hoverIndex;
        private int scrollToIndex;
        private float scrollOffset;
        
        private static GUIStyle SearchBox => "ToolbarSeachTextField";
        private static GUIStyle CancelButton => "ToolbarSeachCancelButton";
        private static GUIStyle DisabledCancelButton => "ToolbarSeachCancelButtonEmpty";
        private static GUIStyle Selection => "SelectionRect";
        
        private SearchablePopup(string[] names, int currentIndex, Action<int> onSelectionMade)
        {
            list = new FilteredList(names);
            this.CurrentIndex = currentIndex;
            this.onSelectionMade = onSelectionMade;
            
            hoverIndex = currentIndex;
            scrollToIndex = currentIndex;
            scrollOffset = GetWindowSize().y - RowHeight * 2;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            // Force a repaint every frame to be responsive to mouse hover.
            EditorApplication.update += Repaint;
        }

        public override void OnClose()
        {
            base.OnClose();
            EditorApplication.update -= Repaint;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(base.GetWindowSize().x,
                Mathf.Min(600, list.MaxLength * RowHeight + 
                EditorStyles.toolbar.fixedHeight));
        }
        
        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }
      
        private void DrawSearch(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
                EditorStyles.toolbar.Draw(rect, false, false, false, false);
            
            Rect searchRect = new Rect(rect);
            searchRect.xMin += 6;
            searchRect.xMax -= 6;
            searchRect.y += 2;
            searchRect.width -= CancelButton.fixedWidth;
            
            GUI.FocusControl(SearchControlName);
            GUI.SetNextControlName(SearchControlName);
            string newText = GUI.TextField(searchRect, list.Filter, SearchBox);

            if (list.UpdateFilter(newText))
            {
                hoverIndex = 0;
                scroll = Vector2.zero;
            }

            searchRect.x = searchRect.xMax;
            searchRect.width = CancelButton.fixedWidth;
            
            if (string.IsNullOrEmpty(list.Filter))
                GUI.Box(searchRect, GUIContent.none, DisabledCancelButton);
            else if (GUI.Button(searchRect, "x", CancelButton))
            {
                list.UpdateFilter("");
                scroll = Vector2.zero;
            }
        }
        
        private void DrawSelectionArea(Rect scrollRect)
        {
            Rect contentRect = new Rect(0, 0,
                scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                list.Entries.Count * RowHeight);

            scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

            Rect rowRect = new Rect(0, 0, scrollRect.width, RowHeight);

            for (int i = 0; i < list.Entries.Count; i++)
            {
                if (scrollToIndex == i &&
                    (Event.current.type == EventType.Repaint
                     || Event.current.type == EventType.Layout))
                {
                    Rect r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    scroll.x = 0;
                }

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseMove ||
                        Event.current.type == EventType.ScrollWheel)
                        hoverIndex = i;
                    if (Event.current.type == EventType.MouseDown)
                    {
                        onSelectionMade(list.Entries[i].Index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                DrawRow(rowRect, i);
                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }

        private void DrawRow(Rect rowRect, int i)
        {
            if (list.Entries[i].Index == CurrentIndex)
                DrawBox(rowRect, Color.cyan);
            else if (i == hoverIndex)
                DrawBox(rowRect, Color.white);

            Rect labelRect = new Rect(rowRect);
            labelRect.xMin += RowIndent;

            GUI.Label(labelRect, list.Entries[i].Text);
        }

        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    hoverIndex = Mathf.Min(list.Entries.Count - 1, hoverIndex + 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = RowHeight;
                }

                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    Event.current.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = -RowHeight;
                }

                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (hoverIndex >= 0 && hoverIndex < list.Entries.Count)
                    {
                        onSelectionMade(list.Entries[hoverIndex].Index);
                        EditorWindow.focusedWindow.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                    EditorWindow.focusedWindow.Close();
            }
        }
    }
}