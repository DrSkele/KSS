using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtension
{
    public static T ObtainComponent<T>(this GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }
}
