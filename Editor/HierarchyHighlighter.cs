using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
public static class HierarchyHighlighter
{
    private const string PREF_KEY = "PIDOR_HierarchyHighlighter";
    private static readonly Dictionary<string, Color> _highlighted = new();

    static HierarchyHighlighter()
    {
        Load();
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        EditorSceneManager.sceneOpened += (_, __) => EditorApplication.RepaintHierarchyWindow();
    }

    private static void OnGUI(int id, Rect rect)
    {
        if (!PidorPackageToggle.IsEnabled()) return;

        var go = EditorUtility.EntityIdToObject(id) as GameObject;
        if (go == null) return;

        var key = $"{go.scene.path}|{go.transform.GetHierarchyPath()}";

        if (_highlighted.TryGetValue(key, out var c))
            EditorGUI.DrawRect(rect, new Color(c.r, c.g, c.b, .5f));

        if (Event.current.type == EventType.MouseDown && Event.current.alt &&
            rect.Contains(Event.current.mousePosition))
        {
            ColorPickerPopup.Show(
                Palette,
                33f,
                35f,
                color => Set(key, color),
                () => Remove(key)
            );
            Event.current.Use();
        }
    }

    private static void Set(string key, Color c)
    {
        _highlighted[key] = c;
        Save();
        EditorApplication.RepaintHierarchyWindow();
    }

    private static void Remove(string key)
    {
        if (_highlighted.Remove(key)) Save();
        EditorApplication.RepaintHierarchyWindow();
    }

    private static void Save()
    {
        var w = new Wrapper();
        foreach (var kv in _highlighted)
            w.items.Add(new Entry { key = kv.Key, color = kv.Value });

        EditorPrefs.SetString(PREF_KEY, JsonUtility.ToJson(w, false));
    }

    private static void Load()
    {
        _highlighted.Clear();
        if (!EditorPrefs.HasKey(PREF_KEY)) return;

        var w = JsonUtility.FromJson<Wrapper>(EditorPrefs.GetString(PREF_KEY));
        if (w?.items == null) return;

        foreach (var e in w.items)
            _highlighted[e.key] = e.color;
    }

    private static readonly Color[] Palette =
    {
        new(1f, .3f, .3f, .9f), new(.3f, 1f, .3f, .9f), new(.3f, .3f, 1f, .9f),
        new(1f, 1f, .3f, .9f), new(.3f, 1f, 1f, .9f), new(1f, .3f, 1f, .9f),
        new(.7f, .7f, .7f, .9f), new(1f, 1f, 1f, .9f)
    };

    [System.Serializable]
    private sealed class Wrapper
    {
        public List<Entry> items = new();
    }


    [System.Serializable]
    private struct Entry
    {
        public string key;
        public Color color;
    }
}