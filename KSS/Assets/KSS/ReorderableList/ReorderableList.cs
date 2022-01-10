using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KSS
{
    [RequireComponent(typeof(LayoutGroup)), DisallowMultipleComponent]
    public class ReorderableList : MonoBehaviour
    {
        [SerializeField] RectTransform dragArea;
        
        RectTransform itemHolder;
        RectTransform overlap;

        LayoutGroup layoutGroup;
        RectTransform dummy;
        bool isVertical;
        bool isHorizontal;
        public bool IsVertical => isVertical;
        public bool IsHorizontal => isHorizontal;

        private void Start()
        {
            itemHolder = GetComponent<RectTransform>();
            overlap = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            layoutGroup = GetComponent<LayoutGroup>();
            isVertical = layoutGroup is VerticalLayoutGroup || layoutGroup is GridLayoutGroup;
            isHorizontal = layoutGroup is HorizontalLayoutGroup || layoutGroup is GridLayoutGroup;
        }
        public void OverlapItem(RectTransform item)
        {
            item.SetParent(overlap);
            item.SetAsLastSibling();
        }
        public void CreateDummyItem(RectTransform original)
        {
            dummy = new GameObject("Dummy").AddComponent<RectTransform>();
            int itemSiblingIndex = original.GetSiblingIndex();
            dummy.sizeDelta = original.sizeDelta;
            dummy.SetParent(itemHolder);
            dummy.SetSiblingIndex(itemSiblingIndex);
        }
        public void SwapWithDummy(RectTransform item)
        {
            if (!dummy)
                return;

            int itemSiblingIndex = item.GetSiblingIndex();
            dummy.SetSiblingIndex(itemSiblingIndex);
        }

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