using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIButton))]
public class UIButtonEditor : Editor
{
    UIButton targetUI;
    SerializedProperty uiSendTarget;

    private void OnEnable()
    {
        targetUI = (UIButton)target;
        uiSendTarget = serializedObject.FindProperty(nameof(targetUI.sendTarget));
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();

        EditorGUILayout.PropertyField(uiSendTarget, new GUIContent("target"));
        var selected = targetUI.index;
        if (targetUI.sendTarget)
            selected = EditorGUILayout.Popup(new GUIContent("key"), targetUI.index, targetUI.sendTarget?.GetEnumArray());

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            Undo.RegisterCompleteObjectUndo(targetUI, nameof(UIButton) + " undo");
            targetUI.index = selected;
            targetUI.key = targetUI.sendTarget?.GetEnumArray()[selected];
            if(targetUI.sendTarget)
                targetUI.sendTarget.AddSender(targetUI);
        }
    }
}
