using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public static class HierarchyHighlighter
{
    private static readonly Dictionary<int, Color> _highlighted = new();

    private static readonly Color[] _palette =
    {
        new(1f, .3f, .3f, .9f), new(.3f, 1f, .3f, .9f), new(.3f, .3f, 1f, .9f),
        new(1f, 1f, .3f, .9f), new(.3f, 1f, 1f, .9f), new(1f, .3f, 1f, .9f),
        new(.7f, .7f, .7f, .9f), new(1f, 1f, 1f, .9f)
    };

    private const float BUTTON_SIZE = 33f;
    private const float WINDOW_HEIGHT = 35f;

    static HierarchyHighlighter() => EditorApplication.hierarchyWindowItemOnGUI += OnGUI;

    private static void OnGUI(int id, Rect rect)
    {
        if (!PidorPackageToggle.IsEnabled()) return;
        
        var go = EditorUtility.InstanceIDToObject(id) as GameObject;
        if (go == null) return;

        if (_highlighted.TryGetValue(id, out var c))
            EditorGUI.DrawRect(rect, new Color(c.r, c.g, c.b, .5f));

        if (Event.current.type == EventType.MouseDown && Event.current.alt &&
            rect.Contains(Event.current.mousePosition))
        {
            ColorPickerPopup.Show(
                _palette,
                BUTTON_SIZE,
                WINDOW_HEIGHT,
                color => Set(id, color),
                () => Remove(id)
            );
            Event.current.Use();
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A)
            if (rect.Contains(Event.current.mousePosition))
            {
                go.SetActive(!go.activeSelf);
                Event.current.Use();
            }
    }

    private static void Set(int id, Color c)
    {
        _highlighted[id] = c;
        EditorApplication.RepaintHierarchyWindow();
    }

    private static void Remove(int id)
    {
        if (_highlighted.ContainsKey(id)) _highlighted.Remove(id);
        EditorApplication.RepaintHierarchyWindow();
    }
}