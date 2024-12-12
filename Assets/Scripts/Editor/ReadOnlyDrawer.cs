using UnityEditor;
using UnityEngine;

// https://www.youtube.com/watch?v=04pTWi_-1_M
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Diable the field
        EditorGUI.PropertyField(position, property, label, true); // 最后一个bool什么意思？
        GUI.enabled = true; // Re-enable the GUI for the rest of the fields
    }
}
