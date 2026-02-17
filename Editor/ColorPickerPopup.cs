using UnityEditor;
using UnityEngine;

public sealed class ColorPickerPopup : EditorWindow
{
    private Color[] _colors;
    private System.Action<Color> _onSelect;
    private System.Action _onRemove;
    private float _buttonSize;
    private float _height;

    public static void Show(
        Color[] colors,
        float buttonSize,
        float height,
        System.Action<Color> onSelect,
        System.Action onRemove)
    {
        var popup = CreateInstance<ColorPickerPopup>();
        popup._colors = colors;
        popup._buttonSize = buttonSize;
        popup._height = height;
        popup._onSelect = c =>
        {
            onSelect?.Invoke(c);
            popup.Close();
        };
        popup._onRemove = () =>
        {
            onRemove?.Invoke();
            popup.Close();
        };

        var mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        var width = colors.Length * (buttonSize + 2) + buttonSize;
        popup.position = new Rect(mouse.x, mouse.y, width, height);
        popup.ShowPopup();
    }

    private void OnLostFocus() => Close();

    private void OnGUI()
    {
        if (!PidorPackageToggle.IsEnabled()) return;
        
        var width = _colors.Length * (_buttonSize + 2) + _buttonSize;
        minSize = maxSize = new Vector2(width, _buttonSize);

        for (int i = 0; i < _colors.Length; i++)
        {
            var old = GUI.backgroundColor;
            GUI.backgroundColor = _colors[i];
            if (GUI.Button(new Rect(i * (_buttonSize + 2), 0, _buttonSize, _buttonSize), ""))
                _onSelect?.Invoke(_colors[i]);
            GUI.backgroundColor = old;
        }

        if (GUI.Button(
                new Rect(_colors.Length * (_buttonSize + 2), 0, _buttonSize, _buttonSize), "X"))
            _onRemove?.Invoke();
    }
}