using System;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

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
public class DataBinder : Singleton<DataBinder>
{
    /// <summary>
    /// Dictionary for databinding.
    /// </summary>
    private Dictionary<string, BindedValue> bindedDatas = new Dictionary<string, BindedValue>();
    private List<IBindableObj> bindables = new List<IBindableObj>();
    /// <summary>
    /// Gets / Sets value of the key.
    /// Updates binded value everytime on Set.
    /// </summary>
    public object this[string key]
    {
        get => bindedDatas[key].obj;
        set
        {
            if (bindedDatas == null)
            {
                bindedDatas = new Dictionary<string, BindedValue>();
            }

            if (ContainsKey(key))
                bindedDatas[key].obj = value;
            else
                bindedDatas[key] = new BindedValue(value);

            UpdateBindedValue(key);
        }
    }
    /// <summary>
    /// Register object to the databinder, so it can be updated when binded value changes.
    /// </summary>
    public void AddToDataBinder(IBindableObj obj)
    {
        bindables.Add(obj);
    }
    /// <summary>
    /// Removes object from the databinder, so it won't be updated anymore.
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveFromDataBinder(IBindableObj obj, bool removeAllData = false)
    {
        bindables.Remove(obj);
        if (removeAllData)
        {
            foreach (var key in obj.GetKeys())
            {
                bindedDatas.Remove(key);
            }
        }
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
    public Type GetValueType(string key)
    {
        if(ContainsKey(key))
            return bindedDatas[key].type;
        return null;
    }
    /// <summary>
    /// Get callback for value change event of the key.
    /// If there is no key defined, makes empty value and returns event.
    /// </summary>
    public UnityEvent<object> GetKeyEvent(string key)
    {
        if(ContainsKey(key))
            return bindedDatas[key].action;
        this[key] = null;
        return bindedDatas[key].action;
    }
    /// <summary>
    /// Removes key and binded data of the key.
    /// </summary>
    public void RemoveKey(string key)
    {
        bindedDatas.Remove(key);
    }
    /// <summary>
    /// Updates binded value of <see cref="IBindableObj"/> in child.
    /// </summary>
    private void UpdateBindedValue(string key)
    {
        foreach (var bindable in bindables)
        {
            if (bindable.GetKeys().Any(x => string.IsNullOrEmpty(x)) || bindable.GetKeys().Any(x => x == key))
                bindable.UpdateDataBinding(this);
        }
    }
}
