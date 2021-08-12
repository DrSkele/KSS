using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BindableActivator))]
public class BindableActivatorEditor : Editor
{
    BindableActivator activator;

    private void OnEnable()
    {
        activator = (BindableActivator)target;
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        serializedObject.Update();

        bool userNameAsKey = EditorGUILayout.Toggle(new GUIContent("UseNameAsKey", "Use attached gameobject's name as key"), activator.useNameAsKey);
        string key = userNameAsKey ? "" : EditorGUILayout.TextField("Key", activator.key);
        
        EditorGUILayout.HelpBox("True : activates \nFalse : deactivates", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
        
        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            Undo.RegisterCompleteObjectUndo(activator, nameof(BindableActivator) + " undo");
            activator.useNameAsKey = userNameAsKey;
            activator.key = key;
        }
    }
}
