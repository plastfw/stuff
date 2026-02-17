using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public static class FolderColorizer
{
    private static readonly Dictionary<string, Color> _colors = new();

    private static readonly Color[] _palette =
    {
        new(1f, .3f, .3f, .9f), new(.3f, 1f, .3f, .9f), new(.3f, .3f, 1f, .9f),
        new(1f, 1f, .3f, .9f), new(.3f, 1f, 1f, .9f), new(1f, .3f, 1f, .9f),
        new(.7f, .7f, .7f, .9f), new(1f, 1f, 1f, .9f)
    };

    private const float BUTTON_SIZE = 33f;
    private const float WINDOW_HEIGHT = 35f;

    static FolderColorizer() => EditorApplication.projectWindowItemOnGUI += OnGUI;

    private static void OnGUI(string guid, Rect rect)
    {
        if (!PidorPackageToggle.IsEnabled()) return;

        var path = AssetDatabase.GUIDToAssetPath(guid);
        if (!AssetDatabase.IsValidFolder(path)) return;

        if (_colors.TryGetValue(path, out var c))
            EditorGUI.DrawRect(rect, new Color(c.r, c.g, c.b, .5f));

        if (Event.current.type == EventType.MouseDown && Event.current.alt &&
            rect.Contains(Event.current.mousePosition))
        {
            ColorPickerPopup.Show(
                _palette,
                BUTTON_SIZE,
                WINDOW_HEIGHT,
                color =>
                {
                    _colors[path] = color;
                    EditorApplication.RepaintProjectWindow();
                },
                () =>
                {
                    if (_colors.ContainsKey(path)) _colors.Remove(path);
                    EditorApplication.RepaintProjectWindow();
                }
            );
            Event.current.Use();
        }
    }
}