using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public enum DropDownBindingOption { dropdown_options, index, name }
/// <summary>
/// DataBinding helper for <see cref="UIBehaviour"/> components.
/// Currently supports Text, Image, Toggle, Slider, and Dropdown.
/// </summary>
public class BindableUI : BindableObj
{
    [HideInInspector] public string key;
    /// <summary>
    /// Binded UI component.
    /// </summary>
    /*[HideInInspector]*/[SerializeField] public Component component;
    /// <summary>
    /// Invoked when value changed in <see cref="DataBinder"/>
    /// </summary>
    private DataBindUpdater updater;
    public delegate void DataBindUpdater(DataBinder binder);
    private DataBinder binderSource;

    public string txtString;
    public string[] txtKeys;

    /// <summary>
    /// When true, user input changes binded value.
    /// </summary>
    public bool doUpdateOnValueChanged;
    public DropDownBindingOption bindingOption;
    /// <summary>
    /// index of binded component.
    /// </summary>
    [HideInInspector]
    public int index;

    private void Awake()
    {
        if(component == null)
        {
            component = GetSupportedComponents(gameObject)[index];
        }

        if (component is Text || component is TMP_Text)
        {
            string pattern = @"\{[^}]*}"; //ex. "This is {key}" => "{key}";
            var matches = Regex.Matches(txtString, pattern, RegexOptions.None, TimeSpan.FromSeconds(0.25f));
            txtKeys = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                txtKeys[i] = matches[i].Value.Trim('{', '}');
            }
        }

        if (doUpdateOnValueChanged)
        {
            switch (component)
            {
                case Toggle toggle:
                    toggle.onValueChanged.AddListener(ToggleValueChanged);
                    break;
                case Slider slider:
                    slider.onValueChanged.AddListener(SliderValueChanged);
                    break;
                case Dropdown dropdown:
                    dropdown.onValueChanged.AddListener(DropdownValueChanged);
                    break;
                case TMP_Dropdown dropPro:
                    dropPro.onValueChanged.AddListener(DropdownValueChanged);
                    break;
                case InputField input:
                    input.onEndEdit.AddListener(InputFieldValueChanged);
                    break;
                case TMP_InputField inputPro:
                    inputPro.onEndEdit.AddListener(InputFieldValueChanged);
                    break;
            }
        }
    }
    private void OnDestroy()
    {
        switch (component)
        {
            case Toggle toggle:
                    toggle.onValueChanged.RemoveListener(ToggleValueChanged);
                break;
            case Slider slider:
                    slider.onValueChanged.RemoveListener(SliderValueChanged);
                break;
            case Dropdown dropdown:
                    dropdown.onValueChanged.RemoveListener(DropdownValueChanged);
                break;
            case TMP_Dropdown dropdown:
                dropdown.onValueChanged.RemoveListener(DropdownValueChanged);
                break;
            case InputField input:
                input.onEndEdit.RemoveListener(InputFieldValueChanged);
                break;
            case TMP_InputField inputPro:
                inputPro.onEndEdit.RemoveListener(InputFieldValueChanged);
                break;
        }
    }

    #region IBindableObjFeature
    public override string[] GetKeys()
    {
        return component is Text || component is TMP_Text ? txtKeys : new string[] { key };
    }
    public override string GetAttachedObject()
    {
        return this.gameObject.name;
    }
    public override string GetBindedComponent()
    {
        return component.name;
    }
    public override Type GetRequiredType()
    {
        switch (component)
        {
            case Text txt:
            case TMP_Text txtPro:
                return typeof(string);
            case Image img:
                return typeof(Sprite);
            case RawImage imgRaw:
                return typeof(Texture);
            case Toggle toggle:
                return typeof(bool);
            case Slider slider:
                return typeof(float);
            case Dropdown dropdown:
            case TMP_Dropdown dropdownPro:
                switch(bindingOption)
                {
                    case DropDownBindingOption.dropdown_options:
                        return typeof(string[]);
                    case DropDownBindingOption.index:
                        return typeof(int);
                    case DropDownBindingOption.name:
                        return typeof(string);
                }
                break;
            case InputField input:
            case TMP_InputField inputPro:
                return typeof(string);
        }
        return null;
    }
    public override void UpdateDataBinding(DataBinder binder)
    {
        if(component == null)
            component = GetSupportedComponents(gameObject)[index];

        binderSource = binder;

        if (component is null)
        {
            Debug.LogWarning($"Wrong component settings on object \"{this.name}\".", this);
            return;
        }
        if (binder.ContainsKey(key) == false && !(component is Text || component is TMP_Text))
        {
            Debug.LogWarning($"Key \"{key}\" on object \"{this.name}\" does not exist in DataBinder", this);
            return;
        }

        switch (component)
        {
            case Text txt:
            case TMP_Text txtPro:
                {
                    updater = UpdateTextBinding;
                }
                break;
            case Image img:
            case RawImage imgRaw:
                {
                    updater = UpdateImageBinding;
                }
                break;
            case Toggle toggle:
                {
                    updater = UpdateToggleBinding;
                }
                break;
            case Slider slider:
                {
                    updater = UpdateSliderBinding;
                }
                break;
            case Dropdown dropdown:
            case TMP_Dropdown dropdownPro:
                {
                    updater = UpdateDropdownBinding;
                }
                break;
            case InputField input:
            case TMP_InputField inputPro:
                {
                    updater = UpdateInputFieldBinding;
                }
                break;
        }

        if(updater != null)
            updater(binder);
    }
    #endregion

    #region UpdateBinding
    /// <summary>
    /// Updates data from Text component's text.
    /// key included in the text in format "{key}" will be replaced with value pair.
    /// </summary>
    private void UpdateTextBinding(DataBinder binder)
    {
        string pattern = @"\{[^}]*}"; //ex. "This is {key}" => "{key}";
        string replacedText = Regex.Replace(txtString, pattern, KeyExtractor, RegexOptions.None, TimeSpan.FromSeconds(0.25f));

        if (component is Text)
        {
            var txt = component as Text;
            txt.text = replacedText;       
        }
        else if (component is TMP_Text)
        {
            var txt = component as TMP_Text;
            txt.text = replacedText;
        }

        string KeyExtractor(Match match)
        {
            string matchValue = match.Value.Trim('{', '}');
            if (binder.ContainsKey(matchValue))// && binder.GetValueType(matchValue) == typeof(string))
                return binder[matchValue].ToString();

            return match.Value;
        }
    }
    
    private void UpdateImageBinding(DataBinder binder)
    {
        if (component is Image)
        {
            var image = component as Image;
            if (binder[key] is Sprite)
                image.sprite = binder[key] as Sprite;
            else
                Debug.LogWarning($"Value for \"{key}\" does not contain a Sprite", this);
        }
        if(component is RawImage)
        {
            var image = component as RawImage;
            if (binder[key] is Texture)
                image.texture = binder[key] as Texture;
            else
                Debug.LogWarning($"Value for \"{key}\" does not contain a Texture", this);
        }
    }

    private void UpdateToggleBinding(DataBinder binder)
    {
        var toggle = component as Toggle;

        if (binder[key] is bool)
            toggle.isOn = (bool)binder[key];
        else
            Debug.LogWarning($"Value for \"{key}\" does not contain a boolean value", this);
    }

    private void UpdateSliderBinding(DataBinder binder)
    {
        var slider = component as Slider;

        if (binder[key] is float)
            slider.value = (float)binder[key];
        else
            Debug.LogWarning($"Value for \"{key}\" does not contain a float value", this);
    }
    private void UpdateDropdownBinding(DataBinder binder)
    {
        if (component is Dropdown)
        {
            var dropdown = component as Dropdown;
            if ((DropDownBindingOption)bindingOption == DropDownBindingOption.dropdown_options)
            {
                dropdown.ClearOptions();

                if (binder[key] is string[])
                {
                    string[] valueArray = binder[key] as string[];
                    var newList = new Dropdown.OptionDataList();
                    foreach (var bindedValue in valueArray)
                    {
                        newList.options.Add(new Dropdown.OptionData(bindedValue));
                    }
                    dropdown.options = newList.options;
                }
                else
                    Debug.LogWarning($"Value for \"{key}\" does not contain a string array value", this);
            }
            else if ((DropDownBindingOption)bindingOption == DropDownBindingOption.index)
            {
                if (binder[key] is int)
                {
                    dropdown.value = (int)binder[key];
                }
                else
                    Debug.LogWarning($"Value for \"{key}\" does not contain an int value", this);
            }
            else if((DropDownBindingOption)bindingOption == DropDownBindingOption.name)
            {
                if(binder[key] is string)
                {
                    int idx = dropdown.options.FindIndex(x => x.text == binder[key] as string);
                    if (idx >= 0)
                        dropdown.value = idx;
                    else
                        Debug.LogWarning($"Dropdown does not have option named \"{binder[key] as string}\"");
                }
                else
                    Debug.LogWarning($"Value for \"{key}\" does not contain a string value", this);
            }
        }
        if (component is TMP_Dropdown)
        {
            var dropdown = component as TMP_Dropdown;
            if ((DropDownBindingOption)bindingOption == DropDownBindingOption.dropdown_options)
            {
                dropdown.ClearOptions();

                if (binder[key] is string[])
                {
                    string[] valueArray = binder[key] as string[];
                    var newList = new TMP_Dropdown.OptionDataList();
                    foreach (var bindedValue in valueArray)
                    {
                        newList.options.Add(new TMP_Dropdown.OptionData(bindedValue));
                    }
                    dropdown.options = newList.options;
                }
                else
                    Debug.LogWarning($"Value for \"{key}\" does not contain a string array value", this);
            }
            else if ((DropDownBindingOption)bindingOption == DropDownBindingOption.index)
            {
                if (binder[key] is int)
                {
                    dropdown.value = (int)binder[key];
                }
                else
                    Debug.LogWarning($"Value for \"{key}\" does not contain an int value", this);
            }
            else if ((DropDownBindingOption)bindingOption == DropDownBindingOption.name)
            {
                if (binder[key] is string)
                {
                    int idx = dropdown.options.FindIndex(x => x.text == binder[key] as string);
                    if (idx >= 0)
                        dropdown.value = idx;
                    else
                        Debug.LogWarning($"Dropdown does not have option named \"{binder[key] as string}\"", this);
                }
                else
                    Debug.LogWarning($"Value for \"{key}\" does not contain a string value", this);
            }
        }
    }

    private void UpdateInputFieldBinding(DataBinder binder)
    {
        if (component is InputField)
        {
            var txt = component as InputField;
            if(binder[key] is string)
            {
                txt.text = binder[key].ToString();
            }
            else
                Debug.LogWarning($"Value for \"{key}\" does not contain a string value", this);
        }
        else if (component is TMP_InputField)
        {
            var txt = component as TMP_InputField;
            if (binder[key] is string)
            {
                txt.text = binder[key].ToString();
            }
            else
                Debug.LogWarning($"Value for \"{key}\" does not contain a string value", this);
        }
    }
    #endregion

    #region BindedValueChangedListener
    private void ToggleValueChanged(bool isOn)
    {
        binderSource[key] = isOn;
    }
    private void SliderValueChanged(float value)
    {
        binderSource[key] = value;
    }
    private void DropdownValueChanged(int value)
    {
        if(bindingOption == DropDownBindingOption.index)
            binderSource[key] = value;
        else
        {
            if(component is Dropdown)
            {
                var dropdown = component as Dropdown;
                binderSource[key] = dropdown.options[value].text;
            }
            else if(component is TMP_Dropdown)
            {
                var dropdown = component as TMP_Dropdown;
                binderSource[key] = dropdown.options[value].text;
            }
        }
    }
    private void InputFieldValueChanged(string text)
    {
        binderSource[key] = text;
    }
    #endregion

    public static Component[] GetSupportedComponents(GameObject obj)
    {
        return obj.GetComponents<Component>().Where(x => MatchTypes(x)).ToArray();
    }
    /// <summary>
    /// Supported types of component
    /// </summary>
    public static bool MatchTypes(Component obj)
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