using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSS.MultiToggle;

public class MultiToggleExample : MonoBehaviour
{
    [SerializeField] MultiToggle multiToggle;

    private void Awake()
    {
        multiToggle.OnValueChanged.AddListener((index, isOn) => Debug.Log($"{index} : {isOn}"));
    }
    private void Start()
    {
        multiToggle.IsOn(4, true);

    }
}
