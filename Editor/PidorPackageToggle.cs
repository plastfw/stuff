using UnityEditor;

public static class PidorPackageToggle
{
    private const string MENU_ROOT = "PIDOR/";
    private const string ENABLE_KEY = "PIDOR_PACKAGE_ENABLED";

    [MenuItem(MENU_ROOT + "Open Favorites")]
    private static void OpenFavorites()
    {
        if (!IsEnabled()) return;
        FolderQuickOpenWindow.OpenExternal();
    }

    [MenuItem(MENU_ROOT + "Enable Package")]
    private static void Toggle()
    {
        EditorPrefs.SetBool(ENABLE_KEY, !IsEnabled());
        Menu.SetChecked(MENU_ROOT + "Enable Package", IsEnabled());
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.RepaintProjectWindow();
    }

    [MenuItem(MENU_ROOT + "Enable Package", true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MENU_ROOT + "Enable Package", IsEnabled());
        return true;
    }

    public static bool IsEnabled() => EditorPrefs.GetBool(ENABLE_KEY, true);
}