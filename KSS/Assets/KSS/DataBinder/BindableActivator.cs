using System;
using UnityEngine;

public class BindableActivator : MonoBehaviour, IBindableObj
{
    public string _key;
    public string key
    {
        get
        {
            if (_key == null)
                return string.Empty;
            return _key;
        }
        set
        {
            _key = value;
        }
    }
    public GameObject obj => this.gameObject;

    public string GetKey()
    {
        return key;
    }
    public string GetAttachedObject()
    {
        return this.gameObject.name;
    }
    public string GetBindedComponent()
    {
        return "Gameobject";
    }
    public Type GetKeyType()
    {
        return typeof(bool);
    }
    public void UpdateDataBinding(DataBinder binder)
    {
        if (binder.ContainsKey(key) == false)
        {
            Debug.LogError($"Key \"{key}\" on object {this.name} does not exist in DataBinder", this);
            return;
        }
        if (binder.GetKeyType(key) == typeof(bool))
        {
            obj.SetActive((bool)binder[key]);
        }
    }
}
