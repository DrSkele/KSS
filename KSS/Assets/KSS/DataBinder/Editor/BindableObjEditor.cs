using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[CustomEditor(typeof(BindableObj))]
public class BindableObjEditor : Editor
{
    BindableObj obj = null;

    private void OnEnable()
    {
        obj = (BindableObj)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        {
            serializedObject.Update();

            var supportedComponents = obj.GetComponents<UIBehaviour>().Where(x => MatchTypes(x));

            obj.index = EditorGUILayout.Popup("Component", obj.index, supportedComponents.Select(comp => $"({comp.GetType()})").ToArray());

            switch (supportedComponents.ToArray()[obj.index])
            {
                case Text txt:
                case TMP_Text txtPro:
                    EditorGUILayout.HelpBox("To insert keys in Text, wrap a key with brackets {}, i.e. {key}", MessageType.Info);
                    break;
                default:
                    obj.key = EditorGUILayout.TextField("Key", obj.key);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }
    }

    private bool MatchTypes(UIBehaviour obj)
    {
        switch (obj)
        {
            case Text txt:
            case TMP_Text txtPro:
            case Image img:
            case RawImage imgRaw:
            case Toggle toggle:
            case Slider slider:
            case Dropdown dropdown:
                return true;
            default:
                return false;
        }
    }
}
