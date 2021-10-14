using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(AnimationEventController))]
public class AnimationEventControllerEditor : Editor
{
    AnimationEventController controller = null;

    int index;
    AnimatorControllerParameter parameter;
    private void OnEnable()
    {
        controller = (AnimationEventController)target;

        if (controller.animator == null)
        {
            Debug.LogWarning($"Cannot attach this script to {controller.gameObject.name}. Require an Animator component.");
            DestroyImmediate(controller);
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        serializedObject.Update();

        GUIContent title = new GUIContent("Parameters");

        var animatorController = controller.animator.runtimeAnimatorController as AnimatorController;

        var parameters = animatorController.parameters;

        index = EditorGUILayout.Popup(title, controller.index, parameters.Select(param => param.name + $"({param.type})").ToArray());
        parameter = controller.animator.parameters[index];

        serializedObject.ApplyModifiedProperties();
        
        if ((parameter != null && controller.parameter == null) || EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            Undo.RegisterCompleteObjectUndo(controller, nameof(AnimationEventController) + " undo");
            controller.index = index;
            controller.parameter = parameter;
        }
    }
}
