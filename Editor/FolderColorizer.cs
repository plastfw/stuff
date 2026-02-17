using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
public static class FolderColorizer
{
    private const string PREF_KEY = "PIDOR_FolderColorizer";
    private static readonly Dictionary<string, Color> _colors = new();

    static FolderColorizer()
    {
        Load();
        EditorApplication.projectWindowItemOnGUI += OnGUI;
    }

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
                Palette,
                33f,
                35f,
                color => Set(path, color),
                () => Remove(path)
            );
            Event.current.Use();
        }
    }

    private static void Set(string path, Color c)
    {
        _colors[path] = c;
        Save();
        EditorApplication.RepaintProjectWindow();
    }

    private static void Remove(string path)
    {
        if (_colors.Remove(path)) Save();
        EditorApplication.RepaintProjectWindow();
    }

    private static void Save()
    {
        var w = new Wrapper();
        foreach (var kv in _colors)
            w.items.Add(new Entry { key = kv.Key, color = kv.Value });

        EditorPrefs.SetString(PREF_KEY, JsonUtility.ToJson(w, false));
    }

    private static void Load()
    {
        _colors.Clear();
        if (!EditorPrefs.HasKey(PREF_KEY)) return;

        var w = JsonUtility.FromJson<Wrapper>(EditorPrefs.GetString(PREF_KEY));
        if (w?.items == null) return;

        foreach (var e in w.items)
            _colors[e.key] = e.color;
    }

    private static readonly Color[] Palette =
    {
        new(1f, .3f, .3f, .9f), new(.3f, 1f, .3f, .9f), new(.3f, .3f, 1f, .9f),
        new(1f, 1f, .3f, .9f), new(.3f, 1f, 1f, .9f), new(1f, .3f, 1f, .9f),
        new(.7f, .7f, .7f, .9f), new(1f, 1f, 1f, .9f)
    };

    [System.Serializable]
    private struct Entry
    {
        public string key;
        public Color color;
    }

    [System.Serializable]
    private sealed class Wrapper
    {
        public List<Entry> items = new();
    }
}