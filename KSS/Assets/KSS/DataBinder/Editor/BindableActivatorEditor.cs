using UnityEditor;
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
        {
            serializedObject.Update();

            activator.key = EditorGUILayout.TextField("Key", activator.key);
            EditorGUILayout.HelpBox("True : activates \nFalse : deactivates", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }
    }
}
