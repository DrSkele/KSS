using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
/// <summary>
/// Attach this script to object with animator.
/// When using other component's events (ex. Toggle.OnValueChanged, Button.OnClick, ... etc),
/// Select animator parameter you want on this script and put this script to other component's event.
/// </summary>
public class AnimationEventController : MonoBehaviour
{
    [HideInInspector] 
    public Animator animator;
    [HideInInspector]
    public int index;
    
    AnimatorControllerParameter parameter;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        parameter = animator.parameters[index];
    }

    public void SetBool(bool isOn)
    {
        Debug.Log(index);
        if (parameter.type == AnimatorControllerParameterType.Bool)
            animator.SetBool(parameter.name, isOn);
        else
            Debug.LogError($"Wrong parameter : {parameter.name}({parameter.type}) selected on {this.gameObject.name}");
    }

    public void SetInt(int value)
    {
        animator.SetInteger(parameter.name, value);
    }

    public void SetFloat(float value)
    {
        animator.SetFloat(parameter.name, value);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimationEventController))]
public class AnimationEventControllerEditor : Editor
{
    AnimationEventController controller = null;

    private void OnEnable()
    {
        controller = (AnimationEventController)target;
        controller.animator = controller.GetComponent<Animator>();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        {
            serializedObject.Update();

            GUIContent title = new GUIContent("Parameters");

            var animatorController = controller.animator.runtimeAnimatorController as AnimatorController;

            Debug.Log(animatorController.parameters.Count());
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
#endif
