using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Object with DataBinding
/// </summary>
public interface IBindableObj
{
    /// <summary>
    /// Key of this Obj
    /// </summary>
    string GetKey();
    string GetAttachedObject();
    string GetBindedComponent();
    Type GetKeyType();
    void UpdateDataBinding(DataBinder binder);
}
/// <summary>
/// Value binded to the key.
/// </summary>
public class BindedValue
{
    private object _obj;
    /// <summary>
    /// Actual value binded to the key.
    /// </summary>
    public object obj
    {
        get => _obj;
        set
        {
            _obj = value;
            type = value.GetType();
            action?.Invoke(value);
        }
    }
    /// <summary>
    /// Type of the value.
    /// </summary>
    public Type type;
    /// <summary>
    /// Callback for value changed event.
    /// </summary>
    public UnityEvent<object> action;

    public BindedValue() { }
    public BindedValue(object obj)
    {
        this.obj = obj;
        type = obj.GetType();
        action = new UnityEvent<object>();
    }
}
/// <summary>
/// Data Manager for DataBinding.
/// Must be placed higher in hierarchy than <see cref="IBindableObj"/> to work.
/// </summary>
public class DataBinder : MonoBehaviour
{
    /// <summary>
    /// Dictionary for databinding.
    /// </summary>
    private Dictionary<string, BindedValue> bindedDatas = null;
    private IBindableObj[] bindables = null;
    /// <summary>
    /// Gets / Sets value of the key.
    /// Updates binded value everytime on Set.
    /// </summary>
    public object this[string key]
    {
        get => bindedDatas[key].obj;
        set
        {
            if(bindables == null)
            {
                bindables = GetComponentsInChildren<IBindableObj>(true);
            }

            if (bindedDatas == null)
            {
                bindedDatas = new Dictionary<string, BindedValue>();
            }

            if (value == null || bindables == null)
                return;

            if (ContainsKey(key))
                bindedDatas[key].obj = value;
            else
                bindedDatas[key] = new BindedValue(value);

            UpdateBindedValue(key);
        }
    }
    /// <summary>
    /// Adds <see cref="DataBinder"/> component to specified object.
    /// There is no pre-defined keys, so There is no need to attach this DataBinder component in Editor.
    /// Instead, Call this method in runtime to make one.
    /// </summary>
    public static DataBinder Init(GameObject obj)
    {
        return obj.AddComponent<DataBinder>();
    }
    /// <summary>
    /// Check whether provided key exists.
    /// </summary>
    public bool ContainsKey(string key)
    {
        return bindedDatas.ContainsKey(key);
    }
    /// <summary>
    /// Get type of value corresponding to the key.
    /// If there is no key defined, returns null.
    /// </summary>
    public Type GetType(string key)
    {
        if(ContainsKey(key))
            return bindedDatas[key].type;
        return null;
    }
    /// <summary>
    /// Get callback for value change event of the key.
    /// If there is no key defined, returns null.
    /// </summary>
    public UnityEvent<object> GetKeyEvent(string key)
    {
        if(ContainsKey(key))
            return bindedDatas[key].action;
        return null;
    }
    /// <summary>
    /// Updates binded value of <see cref="IBindableObj"/> in child.
    /// </summary>
    private void UpdateBindedValue(string key)
    {
        foreach (var bindable in bindables)
        {
            if (string.IsNullOrEmpty(bindable.GetKey()) || bindable.GetKey() == key)
                bindable.UpdateDataBinding(this);
        }
    }
}
