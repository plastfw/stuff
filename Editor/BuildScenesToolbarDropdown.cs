using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public static class MainToolbarScenesDropdown
{
    private const string kElementPath = "PIDOR/Scenes Dropdown";

    [MainToolbarElement(kElementPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement CreateDropdown()
    {
        var content = new MainToolbarContent("Scene", null, "Select scene");

        return new MainToolbarDropdown(content, rect =>
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled)
                .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            var menu = new GenericMenu();
            foreach (var scene in scenes)
            {
                menu.AddItem(new GUIContent(scene), SceneManager.GetActiveScene().name == scene, () =>
                {
                    var path = EditorBuildSettings.scenes
                        .First(s => System.IO.Path.GetFileNameWithoutExtension(s.path) == scene).path;
                    if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                    EditorSceneManager.OpenScene(path);
                    MainToolbar.Refresh(kElementPath);
                });
            }

            menu.DropDown(rect);
        });
    }
}