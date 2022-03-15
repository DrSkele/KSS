using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KSS
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class ReorderableList : MonoBehaviour
    {
        public RectTransform itemHolder;
        //[Tooltip("Area which the user can move item around.\n(with scrollview, it's recommended to use ViewPort)\nLeave this field empty if you want to move block freely.")]
        [Tooltip("Should items be dragged along the layout's axis?")]
        public bool isDragConstrained;
        public bool isDragAreaLimited;
        public bool scrollOnDrag;
        public Vector2 dragArea;
        public float dragSpeed;

        RectTransform listArea;
        RectTransform overlap;// transform higher in hierarchy which can make item show above the list.

        LayoutGroup layoutGroup;
        ScrollRect scroll;

        RectTransform dummy;//dummy item generated when dragging items.

        public ReorderableListEventHandler OnItemExitEvent = new ReorderableListEventHandler();
        public ReorderableListEventHandler OnItemEnterEvent = new ReorderableListEventHandler();
        public ReorderableListEventHandler OnItemBeginDragEvent = new ReorderableListEventHandler();
        public ReorderableListEventHandler OnItemDropEvent = new ReorderableListEventHandler();

        /// <summary>
        /// Does the layout has vertical axis?
        /// </summary>
        public bool IsVertical { get; private set; }
        /// <summary>
        /// Does the layout has horizontal axis?
        /// </summary>
        public bool IsHorizontal { get; private set; }
        /// <summary>
        /// Is dragging constrained to layout's axis?
        /// </summary>
        public bool IsDragConstrained => isDragConstrained;

        private void Start()
        {
            if (!itemHolder)
            {
                Debug.LogWarning("Item holder is not assigned.", this);
                itemHolder = GetComponent<RectTransform>();
            }
            listArea = GetComponent<RectTransform>();
            overlap = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            layoutGroup = GetComponentInChildren<LayoutGroup>();
            scroll = GetComponentInChildren<ScrollRect>();
            if (layoutGroup)
            {
                IsVertical = layoutGroup is VerticalLayoutGroup || layoutGroup is GridLayoutGroup;
                IsHorizontal = layoutGroup is HorizontalLayoutGroup || layoutGroup is GridLayoutGroup;
            }
        }
        /// <summary>
        /// Show item above list by moving the item to higher order in hierarchy.
        /// </summary>
        public void OverlapItem(RectTransform item)
        {
            item.SetParent(overlap);
            item.SetAsLastSibling();
        }
        /// <summary>
        /// Create placeholder dummy for dragging item.
        /// </summary>
        public void CreateDummyItem(RectTransform original)
        {
            CreateDummyItem(original, original.GetSiblingIndex());
        }
        public void CreateDummyItem(RectTransform original, int index)
        {
            dummy = new GameObject("Dummy").AddComponent<RectTransform>();

            dummy.sizeDelta = original.sizeDelta;
            dummy.SetParent(itemHolder);
            dummy.SetSiblingIndex(index);
        }
        /// <summary>
        /// Swap place with dummy.
        /// </summary>
        public void SwapWithDummy(RectTransform item)
        {
            if (!dummy)
                return;

            int itemSiblingIndex = item.GetSiblingIndex();
            dummy.SetSiblingIndex(itemSiblingIndex);
        }
        /// <summary>
        /// Return original item to it's place and remove dummy
        /// </summary>
        public void RemoveDummyItem(RectTransform original)
        {
            if (!dummy)
                return;

            int dummySiblingIndex = dummy.GetSiblingIndex();
            original.SetParent(itemHolder);
            original.SetSiblingIndex(dummySiblingIndex);

            DisposeDummy();
        }
        public void DisposeDummy()
        {
            Destroy(dummy.gameObject);
            dummy = null;
        }
        /// <summary>
        /// Check if mouse position is within dragable area.<br/>
        /// </summary>
        public bool IsDragable(Vector2 position)
        {
            if (!isDragAreaLimited)
                return true;

            return IsWithInList(position);
        }
        public bool IsWithInList(Vector2 position)
        {
            return listArea.Contains(position);
        }
        Coroutine coroutine;
        public void StartScrollOnDrag()
        {
            if (!scrollOnDrag)
                return;
            if (coroutine != null)
                return;
            coroutine = StartCoroutine(CO_ScrollOnDrag());
        }
        private IEnumerator CO_ScrollOnDrag()
        {
            while(Input.GetKey(KeyCode.Mouse0) || Input.touchCount > 0)
            {
                Vector2 position = Input.mousePosition;

                if(IsWithInList(position))
                    ScrollOnDrag(position);

                yield return new WaitForEndOfFrame();
            }
            coroutine = null;
        }
        private void ScrollOnDrag(Vector2 position)
        {
            if (!scroll)
                return;

            Vector3[] corners = new Vector3[4];
            listArea.GetWorldCorners(corners);
            float upperBound = corners[2].y;
            float lowerBound = corners[0].y;
            float rightBound = corners[2].x;
            float leftBound = corners[0].x;

            if (IsVertical)
            {
                //upper
                if (upperBound - dragArea.y < position.y && position.y < upperBound && scroll.verticalNormalizedPosition < 1)
                {
                    scroll.verticalNormalizedPosition += (dragSpeed * Time.deltaTime);
                }
                //lower
                else if(lowerBound < position.y && position.y < lowerBound + dragArea.y && 0 < scroll.verticalNormalizedPosition)
                {
                    scroll.verticalNormalizedPosition -= (dragSpeed * Time.deltaTime);
                }
            }
            if(IsHorizontal)
            {
                //right
                if (rightBound - dragArea.x < position.x && position.x < rightBound && scroll.horizontalNormalizedPosition < 1)
                {
                    scroll.horizontalNormalizedPosition += (dragSpeed * Time.deltaTime);
                }
                //left
                else if (leftBound < position.x && position.x < leftBound + dragArea.x && 0 < scroll.horizontalNormalizedPosition)
                {
                    scroll.horizontalNormalizedPosition -= (dragSpeed * Time.deltaTime);
                }
            }
        }
    }

    public class ReorderableListEventHandler : UnityEvent<ReorderableListEventData> { }
    public struct ReorderableListEventData
    {
        public ReorderableListItem item;
        public ReorderableList list;
        public int index;
    }
}