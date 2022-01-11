using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KSS
{
    [RequireComponent(typeof(LayoutGroup)), DisallowMultipleComponent]
    public class ReorderableList : MonoBehaviour
    {
        [Tooltip("Area which the user can move item around\n(with scrollview, it's recommended to use ViewPort)")]
        [SerializeField] RectTransform dragArea;
        [Tooltip("Should items be dragged along the layout's axis?")]
        [SerializeField] bool isDragConstrained;
        
        RectTransform itemHolder;
        RectTransform overlap;// transform higher in hierarchy which can make item show above the list.

        LayoutGroup layoutGroup;
        RectTransform dummy;//dummy item generated when dragging items.
        bool isVertical;
        bool isHorizontal;
        /// <summary>
        /// Does the layout has vertical axis?
        /// </summary>
        public bool IsVertical => isVertical;
        /// <summary>
        /// Does the layout has horizontal axis?
        /// </summary>
        public bool IsHorizontal => isHorizontal;
        /// <summary>
        /// Is dragging constrained to layout's axis?
        /// </summary>
        public bool IsDragConstrained => isDragConstrained;

        private void Start()
        {
            itemHolder = GetComponent<RectTransform>();
            overlap = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            layoutGroup = GetComponent<LayoutGroup>();
            isVertical = layoutGroup is VerticalLayoutGroup || layoutGroup is GridLayoutGroup;
            isHorizontal = layoutGroup is HorizontalLayoutGroup || layoutGroup is GridLayoutGroup;
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
            dummy = new GameObject("Dummy").AddComponent<RectTransform>();
            int itemSiblingIndex = original.GetSiblingIndex();
            dummy.sizeDelta = original.sizeDelta;
            dummy.SetParent(itemHolder);
            dummy.SetSiblingIndex(itemSiblingIndex);
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

            Destroy(dummy.gameObject);
            dummy = null;
        }
        /// <summary>
        /// Check if mouse position is within dragable area.<br/>
        /// If there's no <see cref="dragArea"/> assigned, it's always true.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsDragable(Vector2 position)
        {
            if (!dragArea)
                return true;

            Vector3[] corners = new Vector3[4];
            dragArea.GetWorldCorners(corners);
            Vector2 holderWorldPos = corners[0];

            return new Rect(holderWorldPos, dragArea.rect.size).Contains(position);
        }
    }
}