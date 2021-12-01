using System;
using UnityEngine;

namespace KSS.DataBind
{
    /// <summary>
    /// Bindable SetActive Controller
    /// </summary>
    public class BindableActivator : AlwaysBindedObj
    {
        public string _key;
        public string key
        {
            get
            {
                if (useNameAsKey)
                    return obj.name;
                if (_key == null)
                    return string.Empty;
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public bool useNameAsKey = true;
        GameObject obj => this.gameObject;

        public override string[] GetKeys()
        {
            return new string[] { key };
        }
        public override string GetAttachedObject()
        {
            return this.gameObject.name;
        }
        public override string GetBindedComponent()
        {
            return "Gameobject";
        }
        public override Type GetRequiredType()
        {
            return typeof(bool);
        }
        public override void UpdateDataBinding(DataBinder binder)
        {
            if (binder.ContainsKey(key))
            {
                if (binder.GetValueType(key) == typeof(bool))
                {
                    obj.SetActive((bool)binder[key]);
                }
                else
                    Debug.LogWarning($"Key \"{key}\" on object {this.name} does not contain a bool value", this);
            }
            else
                Debug.LogWarning($"Key \"{key}\" on object {this.name} does not exist in DataBinder", this);
        }
    }
}
