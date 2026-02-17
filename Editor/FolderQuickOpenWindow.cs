using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public sealed class FolderQuickOpenWindow : EditorWindow
{
    private const string WINDOW_TITLE = "Favorites Window";
    private const string PREF_KEY = "FolderQuickOpenWindow_Assets";

    private readonly List<Object> _objects = new();
    private Vector2 _scroll;

    [MenuItem("Tools/Open Favorites Window")]
    private static void Open() => GetWindow<FolderQuickOpenWindow>(WINDOW_TITLE);

    private void OnEnable() => LoadObjects();
    private void OnDisable() => SaveObjects();

    private void DrawDropArea()
    {
        var rect = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
        GUI.Box(rect, "Drop here", EditorStyles.helpBox);

        var evt = Event.current;
        if ((evt.type != EventType.DragUpdated && evt.type != EventType.DragPerform) ||
            !rect.Contains(evt.mousePosition))
            return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            foreach (var obj in DragAndDrop.objectReferences)
                if (!_objects.Contains(obj))
                    _objects.Add(obj);
            evt.Use();
        }
    }
    
    public static void OpenExternal()
    {
        if (!PidorPackageToggle.IsEnabled()) return;
        GetWindow<FolderQuickOpenWindow>(WINDOW_TITLE);
    }


    private void DrawObjectList()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        for (int i = 0; i < _objects.Count; i++)
        {
            var obj = _objects[i];
            var icon = AssetPreview.GetMiniThumbnail(obj);
            EditorGUILayout.BeginHorizontal();

            var rect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
            EditorGUI.LabelField(rect, new GUIContent(obj.name, icon), EditorStyles.label);

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
                EditorApplication.ExecuteMenuItem("Window/General/Project");
                Event.current.Use();
            }

            if (GUILayout.Button("X", GUILayout.Width(18)))
            {
                _objects.RemoveAt(i--);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void OnGUI()
    {
        DrawDropArea();
        DrawObjectList();
    }

    private void SaveObjects()
    {
        var paths = _objects
            .Where(o => o != null)
            .Select(AssetDatabase.GetAssetPath)
            .ToArray();

        EditorPrefs.SetString(PREF_KEY, string.Join(";", paths));
    }

    private void LoadObjects()
    {
        _objects.Clear();
        var saved = EditorPrefs.GetString(PREF_KEY, "");
        if (string.IsNullOrEmpty(saved))
            return;

        foreach (var path in saved.Split(';'))
        {
            if (string.IsNullOrEmpty(path))
                continue;
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj != null)
                _objects.Add(obj);
        }
    }
}