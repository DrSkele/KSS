using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KSS.DataBind
{
    [RequireComponent(typeof(Selectable))]
    public class BindableInteractor : BindableComponent
    {
        Selectable _selectable;
        Selectable selectableComponent
        {
            get
            {
                if (_selectable == null)
                {
                    _selectable = GetComponent<Selectable>();
                }
                return _selectable;
            }
        }
        [SerializeField] string _key;
        public string key
        {
            get
            {
                if (useNameAsKey)
                    return this.gameObject.name;
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

        public override Component GetBindedComponent()
        {
            return selectableComponent;
        }

        public override string Key
        {
            get => key; set => key = value;
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
                    selectableComponent.interactable = (bool)binder[key];
                }
                else
                    Debug.LogWarning($"Key \"{key}\" on object {this.name} does not contain a bool value", this);
            }
            else
                Debug.LogWarning($"Key \"{key}\" on object {this.name} does not exist in DataBinder", this);
        }
    }
}
