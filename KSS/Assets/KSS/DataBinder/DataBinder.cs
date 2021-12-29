using System;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using UnityEngine;
using System.Collections;
namespace KSS.DataBind
{
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
                type = value?.GetType();
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
#if UNITY_2020_1_OR_NEWER
        public UnityEvent<object>
#else
    [System.Serializable] public class UnityEvent_object : UnityEvent<object> { }
    public UnityEvent_object
#endif
    action;

        public BindedValue() { }
        public BindedValue(object obj)
        {
            this.obj = obj;
            type = obj?.GetType();

            action =
#if UNITY_2020_1_OR_NEWER
        new UnityEvent<object>();
#else
        new UnityEvent_object();
#endif
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
        private Dictionary<string, List<IBindableObj>> bindables = new Dictionary<string, List<IBindableObj>>();
        public static List<AlwaysBindedObj> alwaysBinded = new List<AlwaysBindedObj>();

        private Queue<string> toBeUpdated = new Queue<string>();

        /// <summary>
        /// Gets / Sets value of the key.
        /// Updates binded value everytime on Set.
        /// </summary>
        public object this[string key]
        {
            get
            {
                if (ContainsKey(key) == false)
                {
                    bindedDatas.Add(key, new BindedValue(null));
                }

                return bindedDatas[key].obj;
            }
            set
            {
                if (bindedDatas == null)
                {
                    bindedDatas = new Dictionary<string, BindedValue>();
                }

                if (ContainsKey(key))
                    bindedDatas[key].obj = value;
                else
                    bindedDatas.Add(key, new BindedValue(value));

                UpdateBindedValue(key);
            }
        }
        /// <summary>
        /// Register object to the databinder, so it can be updated when binded value changes.
        /// </summary>
        public void AddToDataBinder(string key, IBindableObj obj)
        {
            if (bindables.ContainsKey(key) == false)
                bindables.Add(key, new List<IBindableObj>());
            bindables[key].Add(obj);
        }
        /// <summary>
        /// Register object to the databinder, so it can be updated when binded value changes.
        /// </summary>
        public static void AddToDataBinder(AlwaysBindedObj obj)
        {
            alwaysBinded?.Add(obj);
        }
        /// <summary>
        /// Removes object from the databinder, so it won't be updated anymore.
        /// </summary>
        public void RemoveFromDataBinder(string key, IBindableObj obj, bool removeAllData = false)
        {
            if (bindables.ContainsKey(key))
                bindables[key].Remove(obj);
            if (removeAllData)
            {
                bindedDatas.Remove(obj.Key);
            }
        }
        /// <summary>
        /// Removes object from the databinder, so it won't be updated anymore.
        /// </summary>
        public static void RemoveFromDataBinder(AlwaysBindedObj obj)
        {
            alwaysBinded?.Remove(obj);
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
            if (ContainsKey(key))
                return bindedDatas[key].type;
            return null;
        }
        /// <summary>
        /// Get callback for value change event of the key.
        /// If there is no key defined, makes empty value and returns event.
        /// </summary>
        public UnityEvent<object> GetKeyEvent(string key)
        {
            if (ContainsKey(key))
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
            if (bindedDatas.ContainsKey(key))
            {
                if (bindables.ContainsKey(key))
                {
                    foreach (var bindable in bindables[key])
                    {
                        bindable.UpdateDataBinding(this);
                    }
                }

                var binds = alwaysBinded.Where(binded => binded != null && binded.gameObject.scene.IsValid());
                foreach (var binded in binds)
                {
                    if (binded.Key == key)
                        binded.UpdateDataBinding(this);
                }
            }
        }
        public void ForceUpdate()
        {
            var binds = alwaysBinded.Where(binded => binded != null && binded.gameObject.scene.IsValid());
            foreach (var binded in binds)
            {
                if (bindedDatas.ContainsKey(binded.Key))
                    binded.UpdateDataBinding(this);
            }
        }
    }
}