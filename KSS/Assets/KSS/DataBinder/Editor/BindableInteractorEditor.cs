using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(BindableInteractor))]
public class BindableInteractorEditor : Editor
{
    BindableInteractor interactor;

    private void OnEnable()
    {
        interactor = (BindableInteractor)target;
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();

        bool userNameAsKey = EditorGUILayout.Toggle(new GUIContent("UseNameAsKey", "Use attached gameobject's name as key"), interactor.useNameAsKey);
        string key = userNameAsKey ? "" : EditorGUILayout.TextField("Key", interactor.key);

        EditorGUILayout.HelpBox("True : interactable \nFalse : not interactable", MessageType.Info);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            Undo.RegisterCompleteObjectUndo(interactor, nameof(BindableInteractor) + " undo");
            interactor.useNameAsKey = userNameAsKey;
            interactor.key = key;
        }
    }
}
