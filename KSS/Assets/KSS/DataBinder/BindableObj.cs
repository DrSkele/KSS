using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


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

    private ICanvasElement component;
    private DataBindUpdater updater;
    private delegate void DataBindUpdater(DataBinder binder);

    [HideInInspector]
    public int index;

    public string GetKey()
    {
        return key;
    }
    public void UpdateDataBinding(DataBinder binder)
    {
        component ??= GetComponents<ICanvasElement>()[index];

        switch (component)
        {
            case Text txt:
            case TMP_Text txtPro:
                {
                    updater = UpdateTextBinding;
                }
                break;
            case Image img:
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
            if (binder.ContainsKey(matchValue))
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

        var image = component as Image;

        if(binder.ContainsKey(key))
        {
            if (binder[key] is Sprite)
                image.sprite = binder[key] as Sprite;
            else
                Debug.LogError($"Value for \"{key}\" does not contain Sprite");
        }
        else
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder");
        }
    }

    private void UpdateToggleBinding(DataBinder binder)
    {
        if (component is null)
        {
            Debug.LogError($"Wrong component settings on object {this.name}.");
            return;
        }

        var toggle = component as Toggle;

        if(binder.ContainsKey(key))
        {
            if (binder[key] is bool)
                toggle.isOn = (bool)binder[key];
            else
                Debug.LogError($"Value for \"{key}\" does not contain boolean value");
        }
        else
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder");
        }
    }

    private void UpdateSliderBinding(DataBinder binder)
    {
        if(component is null)
        {
            Debug.LogError($"Wrong component settings on object {this.name}.");
            return;
        }

        var slider = component as Slider;

        if(binder.ContainsKey(key))
        {
            if (binder[key] is float)
                slider.value = (float)binder[key];
            else
                Debug.LogError($"Value for \"{key}\" does not contain float value");
        }
        else
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BindableObj))]
public class BindableObjEditor: Editor
{
    BindableObj obj = null;
    SerializedProperty keys;

    private void OnEnable()
    {
        obj = (BindableObj)target;
        keys = serializedObject.FindProperty("keys");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        {
            serializedObject.Update();

            var supportedComponents = obj.GetComponents<ICanvasElement>().Where(x => MatchTypes(x));

            obj.index = EditorGUILayout.Popup("Component", obj.index, supportedComponents.Select(comp => $"({comp.GetType()})").ToArray());
            
            switch (supportedComponents.ToArray()[obj.index])
            {
                case Text txt:
                case TMP_Text txtPro:
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

    private bool MatchTypes(ICanvasElement obj)
    {
        switch(obj)
        {
            case Text txt:
            case TMP_Text txtPro:
            case Image img:
            case Toggle toggle:
            case Slider slider:
                return true;
            default:
                return false;
        }
    }
}
#endif