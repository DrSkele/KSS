using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace KSS
{
    [CustomEditor(typeof(ReorderableList))]
    public class ReorderableListEditor : Editor
    {
        ReorderableList list;

        private void OnEnable()
        {
            list = (ReorderableList)target;
        }
        public override void OnInspectorGUI()
        {
            using (var value = new EditorGUI.ChangeCheckScope())
            {
                var itemHolder = (RectTransform)EditorGUILayout.ObjectField("Item Holder", list.itemHolder, typeof(RectTransform), false);
                var dragConstrain = EditorGUILayout.Toggle("Constrain Drag", list.IsDragConstrained);
                var limitDrag = EditorGUILayout.Toggle("Limit Drag Area", list.isDragAreaLimited);
                var dragEnable = EditorGUILayout.Toggle("Enable Auto Scroll", list.scrollOnDrag);
                var dragArea = (dragEnable) ? EditorGUILayout.Vector2Field("Drag Area", list.dragArea) : Vector2.zero;
                var dragSpeed = (dragEnable) ? EditorGUILayout.FloatField("Drag Speed", list.dragSpeed) : 0;

                if (value.changed)
                {
                    EditorUtility.SetDirty(target);
                    Undo.RegisterCompleteObjectUndo(list, nameof(ReorderableList) + " undo");
                    list.itemHolder = itemHolder;
                    list.isDragConstrained = dragConstrain;
                    list.isDragAreaLimited = limitDrag;
                    list.scrollOnDrag = dragEnable;
                    list.dragArea = dragArea;
                    list.dragSpeed = dragSpeed;
                }
            }
        }
    }
}
