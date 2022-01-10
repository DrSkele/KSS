using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KSS
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class ReorderableListItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        [SerializeField] bool isDraggable = true;
        [SerializeField] RectTransform handle;

        ReorderableList __list;
        RectTransform __rectT;
        CanvasGroup __canvasGroup;
        ReorderableList _list => __list ??= GetComponentInParent<ReorderableList>();
        RectTransform _rectT => __rectT ??= GetComponent<RectTransform>();
        CanvasGroup _canvasGroup => __canvasGroup ??= gameObject.ObtainComponent<CanvasGroup>();

        bool isValid = false;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDraggable)
                return;

            if(!_list)
            {
                Debug.LogError("Has no ReorderableList in parent object");
                return;
            }

            isValid = CheckHandle(eventData.position);
            if(isValid)
            {
                _list.OverlapItem(_rectT);
                _canvasGroup.blocksRaycasts = false;

                //Make dummy item in list
                _list.CreateDummyItem(_rectT);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isValid)
                return;

            if (_list.IsDragable(eventData.position))
            {
                _rectT.anchoredPosition += (_list.IsDragConstrained) ? 
                    new Vector2((_list.IsHorizontal) ? eventData.delta.x : 0, (_list.IsVertical) ? eventData.delta.y : 0)
                    : eventData.delta;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isValid = false;
            _canvasGroup.blocksRaycasts = true;

            _list.RemoveDummyItem(_rectT);
        }

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

        public void OnPointerEnter(PointerEventData eventData)
        {
            _list.SwapWithDummy(_rectT);
        }
    }
}