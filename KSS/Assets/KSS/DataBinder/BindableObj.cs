using System;
using UnityEngine;

/// <summary>
/// Object with DataBinding
/// </summary>
public interface IBindableObj
{
    /// <summary>
    /// Get keys of this Obj. Usually there's only one key, but varies with implemented object.
    /// </summary>
    string[] GetKeys();
    /// <summary>
    /// Get name of object attached to.
    /// </summary>
    string GetAttachedObject();
    /// <summary>
    /// Get name of component binded to.
    /// </summary>
    /// <returns></returns>
    string GetBindedComponent();
    /// <summary>
    /// Get type of value for the key.
    /// </summary>
    /// <returns></returns>
    Type GetRequiredType();
    /// <summary>
    /// Update object with values in binder.
    /// </summary>
    void UpdateDataBinding(DataBinder binder);
}
/// <summary>
/// Basic Bindable Object.
/// Registers itself to Databinder once it's enabled.
/// Unregisters when disabled.
/// </summary>
public abstract class BindableObj : MonoBehaviour, IBindableObj
{
    public abstract string[] GetKeys();
    public abstract string GetAttachedObject();
    public abstract string GetBindedComponent();
    public abstract Type GetRequiredType();
    public abstract void UpdateDataBinding(DataBinder binder);

    private void OnEnable()
    {
        foreach (var key in GetKeys())
        {
            DataBinder.Instance.AddToDataBinder(key, this);
        }
        UpdateDataBinding(DataBinder.Instance);
    }
    private void OnDisable()
    {
        if (DataBinder.IsQuit) return;
        foreach (var key in GetKeys())
        {
            DataBinder.Instance.RemoveFromDataBinder(key, this);
        }
    }
}
/// <summary>
/// Special Bindable Object.
/// Registered to DataBinder even if it's disabled in hierarchy.
/// Always binded during it's lifespan.
/// </summary>
public abstract class AlwaysBindedObj : MonoBehaviour, IBindableObj
{
    public abstract string[] GetKeys();
    public abstract string GetAttachedObject();
    public abstract string GetBindedComponent();
    public abstract Type GetRequiredType();
    public abstract void UpdateDataBinding(DataBinder binder);
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