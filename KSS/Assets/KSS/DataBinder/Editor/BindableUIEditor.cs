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
    DropDownBindingOption dropdownOption = DropDownBindingOption.dropdown_options;
    ImageBindingOption imageOption = ImageBindingOption.sprite;

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


        switch (supportedComponents[obj.index])
        {
            case Text txt:
            case TMP_Text txtPro:
                txtString = EditorGUILayout.TextArea(obj.txtString, GUILayout.Height(50));
                EditorGUILayout.HelpBox("To insert keys in Text, wrap a key with brackets {}, i.e. {key}", MessageType.Info);
                break;
            case Image image:
                imageOption = (ImageBindingOption)EditorGUILayout.EnumPopup("Binding Option", obj.imageOption);
                key = EditorGUILayout.TextField("Key", obj.key);
                break;
            case Toggle toggle:
            case Slider slider:
            case InputField input:
            case TMP_InputField inputPro:
                key = EditorGUILayout.TextField("Key", obj.key);
                doUpdateOnValueChanged = EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                break;
            case Dropdown dropDown:
                dropdownOption = (DropDownBindingOption)EditorGUILayout.EnumPopup("Binding Option", obj.dropdownOption);
                key = EditorGUILayout.TextField("Key", obj.key);
                doUpdateOnValueChanged = (dropdownOption == DropDownBindingOption.dropdown_options) ? false : EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
                break;
            case TMP_Dropdown dropdownPro:
                dropdownOption = (DropDownBindingOption)EditorGUILayout.EnumPopup("Binding Option", obj.dropdownOption);
                key = EditorGUILayout.TextField("Key", obj.key);
                doUpdateOnValueChanged = (dropdownOption == DropDownBindingOption.dropdown_options) ? false : EditorGUILayout.Toggle(new GUIContent("Update On ValueChanged", "Check if you want user input to change binded value"), obj.doUpdateOnValueChanged);
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
            obj.dropdownOption = dropdownOption;
            obj.imageOption = imageOption;
        }
    }
}
