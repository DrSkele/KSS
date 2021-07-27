using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBindableObj
{
    string GetKey();
    void UpdateDataBinding(DataBinder binder);
}

public class BindedValue
{
    private object _obj;
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
    public Type type;
    public Action<object> action;

    public BindedValue() { }
    public BindedValue(object obj)
    {
        this.obj = obj;
        type = obj.GetType();
    }
}

public class DataBinder : MonoBehaviour
{
    private Dictionary<string, BindedValue> bindedDatas = null;
    private IBindableObj[] bindables = null;
    public object this[string key]
    {
        get => bindedDatas[key].obj;
        set
        {
            if(bindables == null)
            {
                bindables = GetComponentsInChildren<IBindableObj>();
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

            BindChanged(key);
        }
    }
    public static DataBinder Init(GameObject obj)
    {
        return obj.AddComponent<DataBinder>();
    }

    public bool ContainsKey(string key)
    {
        return bindedDatas.ContainsKey(key);
    }

    public Type GetType(string key)
    {
        return bindedDatas[key].type;
    }

    public Action<object> GetKeyAction(string key)
    {
        return bindedDatas[key].action;
    }

    public void BindChanged(string key)
    {
        foreach (var bindable in bindables)
        {
            if (string.IsNullOrEmpty(bindable.GetKey()) || bindable.GetKey() == key)
                bindable.UpdateDataBinding(this);
        }
    }
}
