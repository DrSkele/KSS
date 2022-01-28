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
        [SerializeField] protected bool isDraggable = true;
        [SerializeField] protected bool isSwappable = true;
        [SerializeField] protected RectTransform handle;

        //Initialized On Start
        protected ReorderableList currentList;
        protected RectTransform rectT;
        protected CanvasGroup canvasGroup;

        //Used in runtime
        protected bool isValid = false;
        HashSet<ReorderableList> lists = new HashSet<ReorderableList>();

        public ReorderableList List { get => currentList; set => currentList = value; }

        protected virtual void Start()
        {
            currentList = GetComponentInParent<ReorderableList>();
            rectT = GetComponent<RectTransform>();
            canvasGroup = gameObject.ObtainComponent<CanvasGroup>();
            if(currentList)
                lists.Add(currentList);
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

            isValid = false;
            canvasGroup.blocksRaycasts = true;

            currentList.RemoveDummyItem(rectT);
            currentList.OnItemDropEvent.Invoke(new ReorderableListEventData
            {
                item = this,
                list = currentList,
                index = rectT.GetSiblingIndex()
            });
        }
        public virtual void OnPointerEnter(PointerEventData eventData)
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