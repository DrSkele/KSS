using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public enum DropDownBindingOption { dropdown_options, index }
/// <summary>
/// DataBinding helper for <see cref="UIBehaviour"/> components.
/// Currently supports Text, Image, Toggle, Slider, and Dropdown.
/// </summary>
public class BindableObj : MonoBehaviour, IBindableObj
{
    public List<string> keys;
    public string key
    {
        get
        {
            if (keys == null || keys.Count == 0)
                return string.Empty;
            return keys[0];
        }
        set
        {
            if (keys == null)
                keys = new List<string>();
            if (keys.Count == 0)
            {
                keys.Add(value);
                return;
            }
            keys[0] = value;
        }
    }
    /// <summary>
    /// Binded UI component.
    /// </summary>
    private UIBehaviour component;
    /// <summary>
    /// Invoked when value changed in <see cref="DataBinder"/>
    /// </summary>
    private DataBindUpdater updater;
    private delegate void DataBindUpdater(DataBinder binder);
    private DataBinder binderSource;

    /// <summary>
    /// When true, user input changes binded value.
    /// </summary>
    public bool doUpdateOnValueChanged;
    public int bindingOption;
    /// <summary>
    /// index of binded component.
    /// </summary>
    [HideInInspector]
    public int index;

    private void Awake()
    {
        if(doUpdateOnValueChanged)
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

    public string GetKey()
    {
        return key;
    }
    public void UpdateDataBinding(DataBinder binder)
    {
        component ??= GetComponents<UIBehaviour>()[index];
        binderSource = binder;

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
    /// <summary>
    /// Updates data from Text component's text.
    /// key included in the text in format "{key}" will be replaced with value pair.
    /// </summary>
    private void UpdateTextBinding(DataBinder binder)
    {
        if (component is null)
        {
            Debug.LogError($"Wrong Component Settings On {this.name}.");
            return;
        }

        if (component is Text)
        {
            var txt = component as Text;
            string pattern = @"\{[^}]*}"; //ex. "{key}" => "Value"
            txt.text = Regex.Replace(txt.text, pattern, KeyExtractor, RegexOptions.None, TimeSpan.FromSeconds(0.25f));
        }
        if (component is TMP_Text)
        {
            var txt = component as TMP_Text;
            string pattern = @"\{[^}]*}"; //ex. "{key}" => "Value"
            txt.text = Regex.Replace(txt.text, pattern, KeyExtractor, RegexOptions.None, TimeSpan.FromSeconds(0.25f));
        }

        string KeyExtractor(Match match)
        {
            string matchValue = match.Value.Trim('{', '}');
            if (binder.ContainsKey(matchValue) && binder.GetType(key) == typeof(string))
                return binder[matchValue] as string;

            return match.Value;
        }
    }
    
    private void UpdateImageBinding(DataBinder binder)
    {
        if (component is null)
        {
            Debug.LogError($"Wrong component settings on object {this.name}.");
            return;
        }
        if (binder.ContainsKey(key) == false)
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder");
            return;
        }

        if (component is Image)
        {
            var image = component as Image;
            if (binder[key] is Sprite)
                image.sprite = binder[key] as Sprite;
            else
                Debug.LogError($"Value for \"{key}\" does not contain Sprite");
        }
        if(component is RawImage)
        {
            var image = component as RawImage;
            if (binder[key] is Texture)
                image.texture = binder[key] as Texture;
            else
                Debug.LogError($"Value for \"{key}\" does not contain Texture");
        }
    }

    private void UpdateToggleBinding(DataBinder binder)
    {
        if (component is null)
        {
            Debug.LogError($"Wrong component settings on object {this.name}.");
            return;
        }
        if (binder.ContainsKey(key) == false)
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder");
            return;
        }

        var toggle = component as Toggle;

        if (binder[key] is bool)
            toggle.isOn = (bool)binder[key];
        else
            Debug.LogError($"Value for \"{key}\" does not contain boolean value");
    }

    private void UpdateSliderBinding(DataBinder binder)
    {
        if(component is null)
        {
            Debug.LogError($"Wrong component settings on object {this.name}.");
            return;
        }
        if (binder.ContainsKey(key) == false)
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder");
            return;
        }

        var slider = component as Slider;

        if (binder[key] is float)
            slider.value = (float)binder[key];
        else
            Debug.LogError($"Value for \"{key}\" does not contain float value");
    }
    private void UpdateDropdownBinding(DataBinder binder)
    {
        if(component is null)
        {
            Debug.LogError($"Wrong component settings on object {this.name}.");
            return;
        }
        if (binder.ContainsKey(key) == false)
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder");
            return;
        }

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
                Debug.LogError($"Value for \"{key}\" does not contain string array value");
        }
        else if((DropDownBindingOption)bindingOption == DropDownBindingOption.index)
        {
            if(binder[key] is int)
            {
                dropdown.value = (int)binder[key];
            }
            else
                Debug.LogError($"Value for \"{key}\" does not contain int value");
        }
    }

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
}