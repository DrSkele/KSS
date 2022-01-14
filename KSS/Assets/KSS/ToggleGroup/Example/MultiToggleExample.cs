using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiToggleExample : MonoBehaviour
{
    [SerializeField] MultiToggle multiToggle;

    private void Start()
    {
        multiToggle.OnValueChanged.AddListener((index, isOn) => Debug.Log($"{index} : {isOn}"));
    }
}
