using UnityEngine;

/// <summary>
/// Attach this script to object with animator.
/// When using other component's events (ex. Toggle.OnValueChanged, Button.OnClick, ... etc),
/// Select animator parameter you want on this script and put this script to other component's event.
/// </summary>
public class AnimationEventController : MonoBehaviour
{
    private Animator _animator;
    public Animator animator
    {
        get
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();
            return _animator;
        }
    }
    [HideInInspector]
    public int index;

    [SerializeField] public AnimatorControllerParameter parameter;
    private void Awake()
    {
        if (parameter == null)
            parameter = animator.parameters[index];
    }

    public void SetBool(bool isOn)
    {
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
    public void SetTrigger()
    {
        animator.SetTrigger(parameter.name);
    }
}
