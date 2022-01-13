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
        [SerializeField] bool isDraggable = true;
        [SerializeField] bool isSwappable = true;
        [SerializeField] RectTransform handle;

        HashSet<ReorderableList> lists = new HashSet<ReorderableList>();

        ReorderableList currentList;
        RectTransform rectT;
        CanvasGroup canvasGroup;

        bool isValid = false;

        public bool IsValid => isValid;
        public ReorderableList List { get => currentList; set => currentList = value; }

        private void Start()
        {
            currentList = GetComponentInParent<ReorderableList>();
            rectT = GetComponent<RectTransform>();
            canvasGroup = gameObject.ObtainComponent<CanvasGroup>();
            if(currentList)
                lists.Add(currentList);
        }

        #region Handler
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDraggable)
                return;

            if(!currentList)
            {
                Debug.LogError("Has no ReorderableList in parent object");
                return;
            }

            isValid = CheckHandle(eventData.position);
            if (isValid)
            {
                //Make dummy item in list
                currentList.CreateDummyItem(rectT);
                currentList.OverlapItem(rectT);
                canvasGroup.blocksRaycasts = false;
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!isValid)
                return;
            if (!currentList.IsDragable(eventData.position))
                return;

            if(currentList.IsDragConstrained)
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
                currentList.OnItemRemovedEvent.Invoke(new ReorderableListEventData
                {
                    item = this,
                    list = currentList
                });
                currentList.DisposeDummy();

                //add item to current list
                newList.OnItemAddedEvent.Invoke(new ReorderableListEventData 
                {
                    item = this,
                    list = newList
                });
                newList.CreateDummyItem(rectT, 0);
                currentList = newList;
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            isValid = false;
            canvasGroup.blocksRaycasts = true;

            currentList.RemoveDummyItem(rectT);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(isSwappable)
                currentList.SwapWithDummy(rectT);
        }
        #endregion
        protected bool CheckHandle(Vector2 position)
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
    }
}