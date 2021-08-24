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
            case InputField input:
            case TMP_InputField inputPro:
                key = EditorGUILayout.TextField("Key", obj.key);
                doUpdateOnValueChanged = EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                break;
            case Dropdown dropDown:
                bindingOption = (DropDownBindingOption)EditorGUILayout.EnumPopup("Binding Option", obj.bindingOption);
                key = EditorGUILayout.TextField("Key", obj.key);
                doUpdateOnValueChanged = (bindingOption == DropDownBindingOption.dropdown_options) ? false : EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                break;
            case TMP_Dropdown dropdownPro:
                bindingOption = (DropDownBindingOption)EditorGUILayout.EnumPopup("Binding Option", obj.bindingOption);
                key = EditorGUILayout.TextField("Key", obj.key);
                doUpdateOnValueChanged = (bindingOption == DropDownBindingOption.dropdown_options)? false : EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                if (bindingOption != DropDownBindingOption.dropdown_options)
                    EditorGUILayout.HelpBox("TMP_Dropdown can't handle setting Index(or Name) and Dropdown options at the same time. " +
                        "Recommended : 1.Add options manually in Editor. 2. Set Index(or Name) later at different timing(ex. options at Awake, index at Start).", MessageType.Warning);
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
            case TMP_Dropdown dropdownPro:
            case InputField input:
            case TMP_InputField inputPro:
                return true;
            default:
                return false;
        }
    }
}
