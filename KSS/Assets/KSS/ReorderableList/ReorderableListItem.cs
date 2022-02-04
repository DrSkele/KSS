using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KSS
{
    [RequireComponent(typeof(RectTransform)),RequireComponent(typeof(CanvasGroup)), DisallowMultipleComponent]
    public class ReorderableListItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        [SerializeField] RectTransform handle;
        [SerializeField] bool isDraggable = true;
        [SerializeField] bool isSwappable = true;
        [SerializeField] bool minimizeOnDrag = false;
        [SerializeField] Vector2 minimizedSize;

        //Initialized On Start
        ReorderableList currentList;
        RectTransform rectT;
        CanvasGroup canvasGroup;
        ReorderableList childList;

        //Used in runtime
        bool isValid = false;
        HashSet<ReorderableList> lists = new HashSet<ReorderableList>();
        Vector2 originalSize;

        public ReorderableList List { get => currentList; set => currentList = value; }

        private void Start()
        {
            currentList = GetComponentInParent<ReorderableList>();
            rectT = GetComponent<RectTransform>();
            canvasGroup = gameObject.ObtainComponent<CanvasGroup>();
            if(currentList)
                lists.Add(currentList);

            rectT.pivot = new Vector2(0.5f, 1);
            childList = GetComponentInChildren<ReorderableList>();
            childList?.OnItemEnterEvent.AddListener(ExpandRect);
            childList?.OnItemExitEvent.AddListener(ReduceRect);
        }

        #region Handler
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDraggable)
                return;

            if(!currentList)
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
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!currentList.IsDragable(eventData.position) || !isValid)
            {
                //pass drag event to scroll
                ExecuteEvents.Execute(currentList.gameObject, eventData, ExecuteEvents.dragHandler);

                return;
            }

            if (currentList.IsDragConstrained)
            {
                rectT.anchoredPosition +=
                    new Vector2((currentList.IsHorizontal) ? eventData.delta.x : 0,
                    (currentList.IsVertical) ? eventData.delta.y : 0);
            }
            else
                rectT.anchoredPosition += eventData.delta;

            List<RaycastResult> results = new List<RaycastResult>();
            ReorderableList newList = null;
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                newList = result.gameObject.GetComponent<ReorderableList>();
                if (newList)
                    break;
            }

            if (newList && newList != currentList)
            {
                //remove item from previous list
                currentList.OnItemExitEvent.Invoke(new ReorderableListEventData
                {
                    item = this,
                    list = currentList
                });
                currentList.DisposeDummy();

                //add item to current list
                newList.OnItemEnterEvent.Invoke(new ReorderableListEventData 
                {
                    item = this,
                    list = newList
                });
                newList.CreateDummyItem(rectT, 0);
                newList.StartScrollOnDrag();
                currentList = newList;
            }
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!currentList)
            {
                Debug.LogError("Has no ReorderableList in parent object");
                return;
            }
            //pass drag event to scroll
            ExecuteEvents.Execute(currentList.gameObject, eventData, ExecuteEvents.endDragHandler);

            if (isValid)
            {
                isValid = false;
                canvasGroup.blocksRaycasts = true;

                currentList.RemoveDummyItem(rectT);
                currentList.OnItemDropEvent.Invoke(new ReorderableListEventData
                {
                    item = this,
                    list = currentList,
                    index = rectT.GetSiblingIndex()
                });

                if (minimizeOnDrag)
                {
                    rectT.sizeDelta = originalSize;
                }
            }
        }
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if(isSwappable)
                currentList.SwapWithDummy(rectT);
        }
        #endregion
        private bool CheckHandle(Vector2 position)
        {
            if (handle)
            {
                Vector3[] corners = new Vector3[4];
                handle.GetWorldCorners(corners);
                Vector2 handleWorldPos = corners[0];
                return new Rect(handleWorldPos, handle.rect.size).Contains(position);
            }
            return true;
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
}