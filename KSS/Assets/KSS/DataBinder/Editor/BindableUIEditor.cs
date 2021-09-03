using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine;

[CustomEditor(typeof(BindableUI))]
public class BindableUIEditor : Editor
{
    Component[] supportedComponents;
    int index;
    Component component;
    string key = "";
    string txtString = "";
    bool doUpdateOnValueChanged = false;

    BindableUI obj = null;

    private void OnEnable()
    {
        obj = (BindableUI)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        serializedObject.Update();

        supportedComponents = BindableUI.GetSupportedComponents(obj.gameObject).ToArray();

        index = EditorGUILayout.Popup("Component", obj.index, supportedComponents.Select(comp => $"({comp.GetType()})").ToArray());
        component = supportedComponents.ToArray()[index];

        DropDownBindingOption bindingOption = DropDownBindingOption.dropdown_options;

        switch (supportedComponents[obj.index])
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
                doUpdateOnValueChanged = (bindingOption == DropDownBindingOption.dropdown_options) ? false : EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                if (bindingOption != DropDownBindingOption.dropdown_options)
                    EditorGUILayout.HelpBox("TMP_Dropdown can't handle setting Index(or Name) and Dropdown options at the same time. " +
                        "Recommended : 1.Add options manually in Editor. 2. Set Index(or Name) later at different timing(ex. options at Awake, index at Start).", MessageType.Warning);
                break;
            default:
                key = EditorGUILayout.TextField("Key", obj.key);
                break;
        }
        
        serializedObject.ApplyModifiedProperties();

        if ((component != null && obj.component == null) || EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(obj);
            Undo.RegisterCompleteObjectUndo(obj, nameof(BindableUI) + " undo");
            obj.component = component;
            obj.index = index;
            obj.key = key;
            obj.txtString = txtString;
            obj.doUpdateOnValueChanged = doUpdateOnValueChanged;
            obj.bindingOption = bindingOption;
        }
    }
}
