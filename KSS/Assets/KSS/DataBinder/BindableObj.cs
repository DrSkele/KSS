using System;
using UnityEngine;
namespace KSS.DataBind
{
    /// <summary>
    /// Object with DataBinding
    /// </summary>
    public abstract class BindableObj: MonoBehaviour
    {
        /// <summary>
        /// The key of this BindableObj.
        /// </summary>
        public virtual string Key { get; set; }
        /// <summary>
        /// Get component binded to.
        /// </summary>
        public abstract Component GetBindedComponent();
        /// <summary>
        /// Get type of value for the key.
        /// </summary>
        /// <returns></returns>
        public abstract Type GetRequiredType();
        /// <summary>
        /// Update object with values in binder.
        /// </summary>
        public abstract void UpdateDataBinding(DataBinder binder);
    }
    /// <summary>
    /// Basic Bindable Object.
    /// Registers itself to Databinder once it's enabled.
    /// Unregisters when disabled.
    /// </summary>
    public abstract class BindableComponent : BindableObj
    {
        private void OnEnable()
        {
            DataBinder.Instance.AddToDataBinder(Key, this);
            UpdateDataBinding(DataBinder.Instance);
        }
        private void OnDisable()
        {
            if (DataBinder.IsQuit) return;
            DataBinder.Instance?.RemoveFromDataBinder(Key, this);
        }
    }
    /// <summary>
    /// Special Bindable Object.
    /// Registered to DataBinder even if it's disabled in hierarchy.
    /// Always binded during it's lifespan.
    /// </summary>
    public abstract class AlwaysBindedObj : BindableObj
    {
        /// <summary>
        /// Registers to databinder once it's created.
        /// Binded whether it's disabled or enabled.
        /// </summary>
        public AlwaysBindedObj() : base()
        {
            DataBinder.AddToDataBinder(this);
        }
        ~AlwaysBindedObj()
        {
            if (DataBinder.IsQuit) return;
            DataBinder.RemoveFromDataBinder(this);
        }
    }
}