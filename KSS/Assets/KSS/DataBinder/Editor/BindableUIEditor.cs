using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine;

[CustomEditor(typeof(BindableUI))]
public class BindableUIEditor : Editor
{
    BindableUI obj = null;

    private void OnEnable()
    {
        obj = (BindableUI)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        serializedObject.Update();

        var supportedComponents = obj.GetComponents<Component>().Where(x => MatchTypes(x));

        obj.index = EditorGUILayout.Popup("Component", obj.index, supportedComponents.Select(comp => $"({comp.GetType()})").ToArray());

        string key = "";
        string txtString = "";
        bool doUpdateOnValueChanged = false;
        DropDownBindingOption bindingOption = DropDownBindingOption.dropdown_options;

        switch (supportedComponents.ToArray()[obj.index])
        {
            case Text txt:
            case TMP_Text txtPro:
                txtString = EditorGUILayout.TextArea(obj.txtString, GUILayout.Height(50));
                EditorGUILayout.HelpBox("To insert keys in Text, wrap a key with brackets {}, i.e. {key}", MessageType.Info);
                break;
            case Toggle toggle:
            case Slider slider:
                key = EditorGUILayout.TextField("Key", obj.key);
                doUpdateOnValueChanged = EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                break;
            case Dropdown dropDown:
                key = EditorGUILayout.TextField("Key", obj.key);
                bindingOption = (DropDownBindingOption)EditorGUILayout.EnumPopup("Binding Option", obj.bindingOption);
                doUpdateOnValueChanged = EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                break;
            default:
                key = EditorGUILayout.TextField("Key", obj.key);
                break;
        }

        serializedObject.ApplyModifiedProperties();
        
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            Undo.RegisterCompleteObjectUndo(obj, nameof(BindableUI) + " undo");
            obj.key = key;
            obj.txtString = txtString;
            obj.doUpdateOnValueChanged = doUpdateOnValueChanged;
            obj.bindingOption = bindingOption;
        }
    }
    /// <summary>
    /// Supported types of component
    /// </summary>
    private bool MatchTypes(Component obj)
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
