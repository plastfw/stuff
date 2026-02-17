using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public sealed class PidorSaves
{
    private const string KEY = "pidor_favorites_v3";

    [SerializeField] private List<string> _paths = new();

    public int Count => _paths.Count;

    public static PidorSaves Load()
    {
        if (!EditorPrefs.HasKey(KEY))
            return new PidorSaves();

        var json = EditorPrefs.GetString(KEY);
        var data = JsonUtility.FromJson<PidorSaves>(json);
        return data == null || data._paths == null ? new PidorSaves() : data;
    }

    private void Save() => EditorPrefs.SetString(KEY, JsonUtility.ToJson(this, false));

    public string GetPath(int index) => _paths[index];

    public void Add(string path)
    {
        if (_paths.Contains(path)) return;
        _paths.Add(path);
        Save();
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_paths.Count) return;
        _paths.RemoveAt(index);
        Save();
    }
}