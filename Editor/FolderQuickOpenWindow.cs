using UnityEditor;
using UnityEngine;

public sealed class FolderQuickOpenWindow : EditorWindow
{
    private const string WINDOW_TITLE = "Favorites Window";

    private PidorSaves _data;
    private Vector2 _scroll;

    public static void OpenExternal(PidorSaves data)
    {
        var w = GetWindow<FolderQuickOpenWindow>(WINDOW_TITLE);
        w._data = data;
    }

    private void OnEnable() => _data ??= PidorSaves.Load();

    private void OnGUI()
    {
        DrawDropArea();
        DrawObjectList();
    }

    private void DrawDropArea()
    {
        var rect = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
        GUI.Box(rect, "Drop here", EditorStyles.helpBox);

        var e = Event.current;
        if ((e.type != EventType.DragUpdated && e.type != EventType.DragPerform) || !rect.Contains(e.mousePosition))
            return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (e.type != EventType.DragPerform) return;

        DragAndDrop.AcceptDrag();
        foreach (var o in DragAndDrop.objectReferences)
            Add(o);

        e.Use();
    }

    private void DrawObjectList()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        for (int i = 0; i < _data.Count; i++)
        {
            var path = _data.GetPath(i);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

            if (obj == null)
            {
                _data.RemoveAt(i--);
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            var rect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
            EditorGUI.LabelField(rect, new GUIContent(obj.name, AssetPreview.GetMiniThumbnail(obj)));

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
                EditorApplication.ExecuteMenuItem("Window/General/Project");
                Event.current.Use();
            }

            if (GUILayout.Button("X", GUILayout.Width(18)))
                _data.RemoveAt(i--);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void Add(UnityEngine.Object obj)
    {
        var path = AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path)) return;
        _data.Add(path);
    }
}