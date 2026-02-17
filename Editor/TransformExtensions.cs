using UnityEngine;

public static class TransformExtensions
{
    public static string GetHierarchyPath(this Transform t)
    {
        if (t.parent == null) return t.name;
        return t.parent.GetHierarchyPath() + "/" + t.name;
    }
}