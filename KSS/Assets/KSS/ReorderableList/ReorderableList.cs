using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KSS
{
    [RequireComponent(typeof(LayoutGroup)), DisallowMultipleComponent]
    public class ReorderableList : MonoBehaviour
    {
        RectTransform itemHolder;
        RectTransform overlap;

        LayoutGroup layoutGroup;
        RectTransform dummy;
        public bool IsVertical => layoutGroup is VerticalLayoutGroup || layoutGroup is GridLayoutGroup;
        public bool IsHorizontal => layoutGroup is HorizontalLayoutGroup || layoutGroup is GridLayoutGroup;

        private void Start()
        {
            itemHolder = GetComponent<RectTransform>();
            overlap = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            layoutGroup = GetComponent<LayoutGroup>();
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
        public bool ItemHolderContains(Vector2 position)
        {
            Vector3[] corners = new Vector3[4];
            itemHolder.GetWorldCorners(corners);
            Vector2 holderWorldPos = corners[0];

            return new Rect(holderWorldPos, itemHolder.rect.size).Contains(position);
        }
    }
}