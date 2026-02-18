using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyToggleActiveHotkey
{
    private const KeyCode TOGGLE_KEY = KeyCode.A;

    static HierarchyToggleActiveHotkey() =>
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;

    private static void OnHierarchyGUI(int instanceId, Rect rect)
    {
        var e = Event.current;
        if (e.type != EventType.KeyDown)
            return;

        if (e.keyCode != TOGGLE_KEY)
            return;

        if (!rect.Contains(e.mousePosition))
            return;

        var obj = EditorUtility.EntityIdToObject(instanceId) as GameObject;
        if (obj == null)
            return;

        Undo.RecordObject(obj, "Toggle Active");
        obj.SetActive(!obj.activeSelf);
        EditorUtility.SetDirty(obj);

        e.Use();
    }
}