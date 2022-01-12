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

        ReorderableList _list;
        RectTransform _rectT;
        CanvasGroup _canvasGroup;

        bool isValid = false;

        public bool IsValid => isValid;
        public ReorderableList List { get => _list; set => _list = value; }

        private void Start()
        {
            _list = GetComponentInParent<ReorderableList>();
            _rectT = GetComponent<RectTransform>();
            _canvasGroup = gameObject.ObtainComponent<CanvasGroup>();
            if(_list)
                lists.Add(_list);
        }

        #region Handler
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
            if (isValid)
            {
                //Make dummy item in list
                _list.CreateDummyItem(_rectT);
                _list.OverlapItem(_rectT);
                _canvasGroup.blocksRaycasts = false;
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!isValid)
                return;
            if (!_list.IsDragable(eventData.position))
                return;

            if(_list.IsDragConstrained)
            {
                _rectT.anchoredPosition +=
                    new Vector2((_list.IsHorizontal) ? eventData.delta.x : 0,
                    (_list.IsVertical) ? eventData.delta.y : 0);
            }
            else
                _rectT.anchoredPosition += eventData.delta;

            List<RaycastResult> results = new List<RaycastResult>();
            ReorderableList currentList = null;
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                currentList = result.gameObject.GetComponent<ReorderableList>();
                if (currentList)
                    break;
            }

            if (currentList && currentList != _list)
            {
                _list.DisposeDummy();
                currentList.CreateDummyItem(_rectT);
                _list = currentList;
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            isValid = false;
            _canvasGroup.blocksRaycasts = true;

            _list.RemoveDummyItem(_rectT);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(isSwappable)
                _list.SwapWithDummy(_rectT);
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