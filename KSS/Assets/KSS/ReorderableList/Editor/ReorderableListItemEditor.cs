using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KSS
{
    [CustomEditor(typeof(ReorderableListItem))]
    public class ReorderableListItemEditor : Editor
    {
        ReorderableListItem item;

        private void OnEnable()
        {
            item = (ReorderableListItem)target;
        }
        public override void OnInspectorGUI()
        {
            using (var value = new EditorGUI.ChangeCheckScope())
            {
                var handle = (RectTransform)EditorGUILayout.ObjectField("Handle", item.handle, typeof(RectTransform), false);
                var draggable = EditorGUILayout.Toggle("Enable Dragging", item.isDraggable);
                var swappable = EditorGUILayout.Toggle("Enable Swapping", item.isSwappable);
                var minimize = EditorGUILayout.Toggle("Minimize of Drag", item.minimizeOnDrag);
                var minimizeSize = (minimize) ? EditorGUILayout.Vector2Field("Minimized Size", item.minimizedSize) : Vector2.zero;

                if(value.changed)
                {
                    EditorUtility.SetDirty(target);
                    Undo.RegisterCompleteObjectUndo(item, nameof(ReorderableListItem) + " undo");
                    item.handle = handle;
                    item.isDraggable = draggable;
                    item.isSwappable = swappable;
                    item.minimizeOnDrag = minimize;
                    item.minimizedSize = minimizeSize;
                }
            }
        }
    }
}