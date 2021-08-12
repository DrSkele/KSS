using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum DropDownBindingOption { dropdown_options, index }
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
    private Component component;
    /// <summary>
    /// Invoked when value changed in <see cref="DataBinder"/>
    /// </summary>
    private DataBindUpdater updater;
    public delegate void DataBindUpdater(DataBinder binder);
    private DataBinder binderSource;

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

    private void Start()
    {
        if(doUpdateOnValueChanged)
        {
            component ??= GetComponents<Component>()[index];

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
        }
    }

    #region IBindableObjFeature
    public override string[] GetKeys()
    {
        List<string> keys = new List<string>();
        if (component is Text)
        {
            var txt = component as Text;
            string pattern = @"\{[^}]*}"; //ex. "{key}" => "Value";
            var matches = Regex.Matches(txt.text, pattern, RegexOptions.None, TimeSpan.FromSeconds(0.25f));
            for (int i = 0; i < matches.Count; i++)
            {
                keys.Add(matches[i].Value + ((i != matches.Count - 1) ? ", " : ""));
            }
            return keys.ToArray();
        }
        else if (component is TMP_Text)
        {
            var txt = component as TMP_Text;
            string pattern = @"\{[^}]*}"; //ex. "{key}" => "Value"
            
            var matches = Regex.Matches(txt.text, pattern, RegexOptions.None, TimeSpan.FromSeconds(0.25f));
            for (int i = 0; i < matches.Count; i++)
            {
                keys.Add(matches[i].Value + ((i != matches.Count - 1) ? ", " : ""));
            }
            return keys.ToArray();
        }
        return new string[] { key };
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
                switch(bindingOption)
                {
                    case DropDownBindingOption.dropdown_options:
                        return typeof(string[]);
                    case DropDownBindingOption.index:
                        return typeof(int);
                }
                break;
        }
        return null;
    }
    public override void UpdateDataBinding(DataBinder binder)
    {
        component ??= GetComponents<UIBehaviour>()[index];
        binderSource = binder;

        if (component is null)
        {
            Debug.LogError($"Wrong component settings on object \"{this.name}\".", this);
            return;
        }
        if (binder.ContainsKey(key) == false && (component is Text || component is TMP_Text) == false)
        {
            Debug.LogError($"Key \"{key}\" on object \"{this.name}\" does not exist in DataBinder", this);
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
                {
                    updater = UpdateDropdownBinding;
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
        if (component is Text)
        {
            var txt = component as Text;
            string pattern = @"\{[^}]*}"; //ex. "{key}" => "Value"
            txt.text = Regex.Replace(txt.text, pattern, KeyExtractor, RegexOptions.None, TimeSpan.FromSeconds(0.25f));
        }
        else if (component is TMP_Text)
        {
            var txt = component as TMP_Text;
            string pattern = @"\{[^}]*}"; //ex. "{key}" => "Value"
            txt.text = Regex.Replace(txt.text, pattern, KeyExtractor, RegexOptions.None, TimeSpan.FromSeconds(0.25f));
        }

        string KeyExtractor(Match match)
        {
            string matchValue = match.Value.Trim('{', '}');
            if (binder.ContainsKey(matchValue) && binder.GetValueType(matchValue) == typeof(string))
                return binder[matchValue] as string;

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
                Debug.LogError($"Value for \"{key}\" does not contain a Sprite", this);
        }
        if(component is RawImage)
        {
            var image = component as RawImage;
            if (binder[key] is Texture)
                image.texture = binder[key] as Texture;
            else
                Debug.LogError($"Value for \"{key}\" does not contain a Texture", this);
        }
    }

    private void UpdateToggleBinding(DataBinder binder)
    {
        var toggle = component as Toggle;

        if (binder[key] is bool)
            toggle.isOn = (bool)binder[key];
        else
            Debug.LogError($"Value for \"{key}\" does not contain a boolean value", this);
    }

    private void UpdateSliderBinding(DataBinder binder)
    {
        var slider = component as Slider;

        if (binder[key] is float)
            slider.value = (float)binder[key];
        else
            Debug.LogError($"Value for \"{key}\" does not contain a float value", this);
    }
    private void UpdateDropdownBinding(DataBinder binder)
    {
        var dropdown = component as Dropdown;
        if((DropDownBindingOption)bindingOption == DropDownBindingOption.dropdown_options)
        {
            dropdown.ClearOptions();

            if (binder[key] is string[])
            {
                string[] valueArray = binder[key] as string[];
                foreach (var bindedValue in valueArray)
                {
                    dropdown.options.Add(new Dropdown.OptionData(bindedValue));
                }
            }
            else
                Debug.LogError($"Value for \"{key}\" does not contain a string array value", this);
        }
        else if((DropDownBindingOption)bindingOption == DropDownBindingOption.index)
        {
            if(binder[key] is int)
            {
                dropdown.value = (int)binder[key];
            }
            else
                Debug.LogError($"Value for \"{key}\" does not contain an int value", this);
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
        binderSource[key] = value;
    }
    #endregion
}