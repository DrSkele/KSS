using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSS;
using UnityEngine.EventSystems;

public class ReorderableContainer : ReorderableListItem
{
    [SerializeField] bool minimizeOnDrag;
    [SerializeField] Vector2 minimizedSize;

    ReorderableList childList;

    Vector2 originalSize;

    protected override void Start()
    {
        base.Start();
        childList = GetComponentInChildren<ReorderableList>();
        childList.OnItemEnterEvent.AddListener(ExpandRect);
        childList.OnItemExitEvent.AddListener(ReduceRect);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable)
            return;

        if (!currentList)
        {
            Debug.LogError("Has no ReorderableList in parent object");
            return;
        }

        //pass drag event to scroll
        ExecuteEvents.Execute(currentList.gameObject, eventData, ExecuteEvents.beginDragHandler);

        isValid = CheckHandle(eventData.position);
        if (isValid)
        {
            if (minimizeOnDrag)
            {
                originalSize = rectT.sizeDelta;
                rectT.sizeDelta = new Vector2(minimizedSize.x == 0 ? originalSize.x : minimizedSize.x,
                    minimizedSize.y == 0 ? originalSize.y : minimizedSize.y);
            }

            //Make dummy item in list
            currentList.CreateDummyItem(rectT);
            currentList.OnItemBeginDragEvent.Invoke(new ReorderableListEventData
            {
                item = this,
                list = currentList,
                index = rectT.GetSiblingIndex()
            });
            currentList.OverlapItem(rectT);
            currentList.StartScrollOnDrag();
            canvasGroup.blocksRaycasts = false;
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if(minimizeOnDrag)
        {
            rectT.sizeDelta = originalSize;
        }
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
