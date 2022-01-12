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

    public static bool Contains(this RectTransform rectT, Vector2 position)
    {
        Vector3[] corners = new Vector3[4];
        rectT.GetWorldCorners(corners);
        Vector2 holderWorldPos = corners[0];

        return new Rect(holderWorldPos, rectT.rect.size).Contains(position);
    }
}
