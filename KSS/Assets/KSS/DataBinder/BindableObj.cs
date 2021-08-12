using System;
using UnityEngine;

/// <summary>
/// Object with DataBinding
/// </summary>
public interface IBindableObj
{
    /// <summary>
    /// Get keys of this Obj. Usually there's one key, but varies with implemented obj.
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
public abstract class BindableObj : MonoBehaviour, IBindableObj
{
    public abstract string[] GetKeys();
    public abstract string GetAttachedObject();
    public abstract string GetBindedComponent();
    public abstract Type GetRequiredType();
    public abstract void UpdateDataBinding(DataBinder binder);

    private void OnEnable()
    {
        DataBinder.Instance.AddToDataBinder(this);
        UpdateDataBinding(DataBinder.Instance);
    }
    private void OnDisable()
    {
        DataBinder.Instance.RemoveFromDataBinder(this);
    }
}