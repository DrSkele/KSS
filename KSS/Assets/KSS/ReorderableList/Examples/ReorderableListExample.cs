using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSS;

public class ReorderableListExample : MonoBehaviour
{
    RectTransform rectT;
    ReorderableList list;

    private void Start()
    {
        rectT = GetComponent<RectTransform>();
        list = GetComponentInChildren<ReorderableList>();
        list.OnItemEnterEvent.AddListener(ExpandRect);
        list.OnItemExitEvent.AddListener(ReduceRect);
    }

    private void ExpandRect(ReorderableListEventData data)
    {
        var itemSize = data.item.GetComponent<RectTransform>().sizeDelta;
        float expandX = data.item.List.IsHorizontal ? itemSize.x : 0;
        float expandY = data.item.List.IsVertical ? itemSize.y : 0;
        rectT.sizeDelta = new Vector2(rectT.sizeDelta.x + expandX, rectT.sizeDelta.y + expandY);
    }
    private void ReduceRect(ReorderableListEventData data)
    {
        var itemSize = data.item.GetComponent<RectTransform>().sizeDelta;
        float reduceX = data.item.List.IsHorizontal ? itemSize.x : 0;
        float reduceY = data.item.List.IsVertical ? itemSize.y : 0;
        rectT.sizeDelta = new Vector2(rectT.sizeDelta.x - reduceX, rectT.sizeDelta.y - reduceY);
    }
}
