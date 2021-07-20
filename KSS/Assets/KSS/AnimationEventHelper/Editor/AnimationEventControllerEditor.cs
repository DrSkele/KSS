using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(AnimationEventController))]
public class AnimationEventControllerEditor : Editor
{
    AnimationEventController controller = null;

    private void OnEnable()
    {
        controller = (AnimationEventController)target;
        controller.animator = controller.GetComponent<Animator>();

        if (controller.animator == null)
        {
            Debug.LogWarning($"Cannot attach this script to {controller.gameObject.name}. Require an Animator component.");
            DestroyImmediate(controller);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        {
            serializedObject.Update();

            GUIContent title = new GUIContent("Parameters");

            var animatorController = controller.animator.runtimeAnimatorController as AnimatorController;

            var parameters = animatorController.parameters;

            controller.index = EditorGUILayout.Popup(title, controller.index, parameters.Select(param => param.name + $"({param.type})").ToArray());

            serializedObject.ApplyModifiedProperties();
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }
    }
}
