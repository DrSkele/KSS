using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBindableObj
{
    string key { get; set; }
    void UpdateDataBinding(DataBinder binder);
}

public class DataBinder : MonoBehaviour
{
    private Dictionary<string, object> bindedDatas = null;
    [SerializeReference] IBindableObj[] bindables = null;
    public object this[string key]
    {
        get => bindedDatas[key];
        set
        {
            if(bindables == null)
            {
                bindables = GetComponentsInChildren<IBindableObj>();
            }

            if (bindedDatas == null)
            {
                bindedDatas = new Dictionary<string, object>();
            }

            if (value == null || bindables == null)
                return;

            bindedDatas[key] = value;

            BindChanged(key);
        }
    }

    public bool ContainsKey(string key)
    {
        return bindedDatas.ContainsKey(key);
    }

    public bool ContainsValue(object obj)
    {
        return bindedDatas.ContainsValue(obj);
    }

    public void BindChanged(string key)
    {
        foreach (var bindable in bindables)
        {
            if (string.IsNullOrEmpty(bindable.key) || bindable.key == key)
                bindable.UpdateDataBinding(this);
        }
    }
}
